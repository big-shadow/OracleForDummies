using System;
using Oracle.ManagedDataAccess.Client;
using OFD.Properties;
using OFD.Reflection;
using OFD.Caching;

namespace OFD.Data
{
    /// <summary>
    /// This class exists only to persist and retreive model records. It serves as a buffer and depends on Reflector, Sniffer, and SQLBuilder.
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

        private static bool Execute(string sql)
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

        private static int GetLastUpdatedId(string tablename)
        {
            string sql = "SELECT MAX(ID)KEEP(DENSE_RANK LAST ORDER BY TIME_UPDATED) AS ID FROM " + tablename;
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
                            throw new Exception(string.Format(Resources.NoID, tablename), ex);
                        }
                    }
                }

                con.Close();
            }

            return val;
        }

        public static void Persist(Model instance)
        {
            string table = Reflector.GetTableName(ref instance);
            string sql = string.Empty;

            // Prepare a create table statement if the table doesn't exist. Then make a trigger to write to it after updates.
            if (Sniffer.ON && !Sniffer.TableExists(table, GetConnection()))
            {
                try
                {
                    if (Execute(SQLBuilder.GetCreateTableStatement(table, Reflector.GetColumnMap(ref instance))))
                    {
                        Execute(Reflector.GetEmbeddedResource("UpdateTrigger").Replace(TokenEnum.TABLE.ToString(), table));
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
                if (Sniffer.RecordExists(table, (int)Reflector.GetProperty(ref instance, "ID"), GetConnection()))
                {
                    sql = SQLBuilder.GetUpdateStatement(table, Reflector.GetPersistenceMap(ref instance));
                }
                else
                {
                    sql = SQLBuilder.GetInsertStatement(table, Reflector.GetPersistenceMap(ref instance));
                }
            }

            // Once updated or inserted, check the ID property and then set it accordingly.
            try
            {
                if (Execute(sql))
                {
                    if ((int)Reflector.GetProperty(ref instance, "ID") == 0)
                    {
                        Reflector.SetProperty(ref instance, "ID", GetLastUpdatedId(table));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoInsert, table), ex);
            }

        }

        public static void GetWhereCondition(Model instance, string condition)
        {
            string table = Reflector.GetTableName(ref instance);
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
                                        Reflector.SetProperty(ref instance, Cache.Get(instance).IdentityCache[name], reader[name]);
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

        public static void Drop(Model instance)
        {
            string table = Reflector.GetTableName(ref instance);

            try
            {
                if (Sniffer.ON && Sniffer.TableExists(table, GetConnection()))
                {
                    Execute(Reflector.GetEmbeddedResource("DropTable").Replace(TokenEnum.TABLE.ToString(), table));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoDrop, table), ex);
            }
        }
    }
}

