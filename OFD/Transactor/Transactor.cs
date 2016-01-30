using System;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using OFD.Properties;
using OFD.Caching;
using OFD.SQLize;
using OFD.Reflect;
using System.Data;
using System.Reflection;

namespace OFD.Transact
{
    /// <summary>
    /// This class exists only to persist and retrieve model records. It serves as a buffer and depends on Reflector, Sniffer, and SQLBuilder.
    /// </summary>
    public static class Transactor
    {
        private static OracleConnection GetConnection()
        {
            string connectionString = @"Data Source =
                  (DESCRIPTION =
                    (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                    (CONNECT_DATA =
                      (SERVER = DEDICATED)
                      (SERVICE_NAME = orcl)
                    )
                  ); User Id = system; Password = 2016;";

            try
            {
                var con = new OracleConnection();
                con.ConnectionString = connectionString;
                con.Open();

                return con;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoConnection, connectionString), ex);
            }
        }

        public static bool Execute(string sql)
        {
            bool result = true;

            try
            {
                using (OracleConnection con = GetConnection())
                {
                    using (OracleCommand command = new OracleCommand(sql, con))
                    {
                        command.ExecuteNonQuery();
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(sql.Substring(0, 50), ex);
            }

            return result;
        }

        public static List<List<object>> GetGeneric(string columns, string table, string condition = null)
        {
            string sql = "SELECT " + columns + " FROM " + table;
            List<List<object>> list = new List<List<object>>();

            if (!string.IsNullOrWhiteSpace(condition))
            {
                sql += " WHERE " + condition;
            }

            using (OracleConnection con = GetConnection())
            {
                using (OracleCommand command = new OracleCommand(sql, con))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                list.Add(new List<object>());

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    list[i].Add(reader.GetValue(i));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            reader.Close();
                            throw new Exception(string.Format(Resources.NoFetchWhere, table, condition), ex);
                        }
                    }
                }

                con.Close();
            }

            return list;
        }

        private static int GetLastUpdatedId(string table)
        {
            string sql = "SELECT MAX(ID)KEEP(DENSE_RANK LAST ORDER BY TIME_UPDATED) AS ID FROM " + table;
            int val = -1;

            using (OracleConnection con = GetConnection())
            {
                using (OracleCommand command = new OracleCommand(sql, con))
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            if (reader.Read())
                            {
                                Int32.TryParse(reader["ID"].ToString(), out val);
                            }
                        }
                        catch (Exception ex)
                        {
                            reader.Close();
                            throw new Exception(string.Format(Resources.NoID, table), ex);
                        }
                    }
                }

                con.Close();
            }

            return val;
        }

        public static void Persist(Model instance)
        {
            string table = Cache.Get(instance).TableName;
            string sql = string.Empty;

            // Prepare a create table statement if the table doesn't exist. Then make a trigger to write to it after updates.
            if (Sniffer.ON && !Sniffer.TableExists(table, GetConnection()))
            {
                try
                {
                    if (Execute(SQLizer.GetCreateTableStatement(table, Cache.Get(instance).ColumnDictionary)))
                    {
                        Execute(Template.Trigger(table));
                        Execute(Template.StoredProcedure(table));
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format(Resources.NoTable, table), ex);
                }

            }

            // If the table exists or was created successfully, check to see if it has already been saved once before.
            if (Sniffer.ON && Sniffer.TableExists(table, GetConnection()))
            {
                // If it's been saved before update the record, otherwise insert a new one.
                if (Sniffer.RecordExists(table, instance.ID, GetConnection()))
                {
                    sql = SQLizer.GetUpdateStatement(table, Reflector.GetPersistenceDictionary(ref instance));
                }
                else
                {
                    sql = SQLizer.GetInsertStatement(table, Reflector.GetPersistenceDictionary(ref instance));
                }
            }

            // Once updated or inserted, check the ID property and then set it accordingly.
            try
            {
                if (Execute(sql))
                {
                    if (instance.ID == 0)
                    {
                        instance.ID = GetLastUpdatedId(table);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoInsert, table), ex);
            }

        }

        public static List<T> GetWhere<T>(string condition) where T : Model, new()
        {
            List<T> collection = new List<T>();
            Model instance = new T();

            string table = Cache.Get(instance).TableName;
            string sql = "SELECT * FROM " + table + " WHERE " + condition;

            try
            {
                using (OracleConnection con = GetConnection())
                {
                    using (OracleCommand command = new OracleCommand(sql, con))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string name = reader.GetName(i).ToUpperInvariant();

                                    if (Cache.Get(instance).IdentityCache.ContainsKey(name))
                                    {
                                        Reflector.SetPropertyValue(ref instance, Cache.Get(instance).IdentityCache[name], reader[name]);
                                    }
                                }

                                collection.Add((T)instance.Clone());
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoFetchWhere, table, condition), ex);
            }

            return collection;
        }

        public static T GetScalarWhere<T>(string condition) where T : Model, new()
        {
            T child = Reflector.GetUninitializedObject<T>();
            SetScalarWhere(child, condition);

            return child;
        }

        public static void SetScalarWhere(Model instance, string condition)
        {
            string table = Cache.Get(instance).TableName;
            string sql = "SELECT * FROM " + table + " WHERE " + condition;

            try
            {
                using (OracleConnection con = GetConnection())
                {
                    using (OracleCommand command = new OracleCommand(sql, con))
                    {
                        using (OracleDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string name = reader.GetName(i).ToUpperInvariant();

                                    if (Cache.Get(instance).IdentityCache.ContainsKey(name))
                                    {
                                        Reflector.SetPropertyValue(ref instance, Cache.Get(instance).IdentityCache[name], reader[name]);
                                    }
                                }
                            }
                        }
                    }

                    con.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoFetchWhere, table, condition), ex);
            }
        }

        public static void Drop(Type type)
        {
            string table = Cache.Get(type).TableName;

            try
            {
                if (Sniffer.ON && Sniffer.TableExists(table, GetConnection()))
                {
                    Execute(Template.Drop(table));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoDrop, table), ex);
            }
        }

        public static void DeleteWhere<T>(string condition) where T : Model, new()
        {
            string table = Cache.Get(typeof(T)).TableName;
            string sql = "DELETE FROM " + table + " WHERE " + condition;

            try
            {
                Execute(sql);
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoDelete, table, condition), ex);
            }
        }

        public static List<T> StoredProcedure<T>(string table, List<Parameter> parameters) where T : Model, new()
        {
            List<T> collection = new List<T>();
            Model instance = new T();

            using (OracleConnection cn = GetConnection())
            {
                using (OracleCommand command = new OracleCommand(null, cn))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = Hasher.Hash(string.Format(Resources.ProcName, table));

                    foreach (Parameter p in parameters)
                    {
                        command.Parameters.Add(p.Name, p.Type).Value = p.Value;
                    }

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                string name = reader.GetName(i).ToUpperInvariant();

                                if (Cache.Get(typeof(T)).IdentityCache.ContainsKey(name))
                                {
                                    Reflector.SetPropertyValue(ref instance, Cache.Get(instance).IdentityCache[name], reader[name]);
                                }
                            }

                            collection.Add((T)instance.Clone());
                        }
                    }
                }
            }

            return collection;
        }
    }
}

public struct Parameter
{
    public Parameter(string name, OracleDbType type, object value)
    {
        Name = name;
        Type = type;
        Value = value;
    }
    public string Name { get; set; }
    public OracleDbType Type { get; set; }
    public object Value { get; set; }
}

public static class Helper
{

}