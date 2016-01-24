using System;
using Oracle.ManagedDataAccess.Client;
using OFD.Properties;

namespace OFD.Data
{
    /// <summary>
    /// This class exists only to persist and retreive model records. It serves as a buffer and depends on Reflector, Sniffer, and SQLBuilder.
    /// </summary>
    public static class Transactor
    {
        private static OracleConnection GetConnection()
        {
            try
            {
                var con = new OracleConnection();

                con.ConnectionString = @"Data Source =
                  (DESCRIPTION =
                    (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                    (CONNECT_DATA =
                      (SERVER = DEDICATED)
                      (SERVICE_NAME = orcl)
                    )
                  ); User Id = system; Password = 2016;";


                con.Open();
                return con;
            }
            catch (Exception)
            {
                return null;
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
            catch(Exception ex)
            {
                throw new Exception(sql.Substring(0, 50), ex);
            }

            return result;
        }

        private static int GetLastUpdatedId(string tablename)
        {
            string sql = "SELECT ID FROM " + tablename + " WHERE ROWNUM <= 1 ORDER BY time_updated DESC";
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
                        catch(Exception ex)
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

        public static void Persist(object instance)
        {
            string table = Reflector.GetClassName(ref instance);
            string sql = string.Empty;

            // Prepare a create table statement if the table doesn't exist. Then make a trigger to write to it after updates.
            if (!Sniffer.TableExists(table, GetConnection()))
            {
                try
                {
                    if (Execute(SQLBuilder.GetCreateTableStatement(table, Reflector.ResolveColumns(ref instance))))
                    {
                        Execute(Reflector.GetEmbeddedResource("UpdateTrigger").Replace(TokenEnum.TABLE.ToString(), table));
                    }
                }
                catch(Exception ex)
                {
                    throw new Exception(string.Format(Resources.NoTable, table), ex);
                }

            }

            // If the table exists or was created sucessfully, check to see if it has already been saved once before.
            if (Sniffer.TableExists(table, GetConnection()))
            {
                // If it's been saved once update the record, otherwise insert a new one.
                if(Sniffer.RecordExists(table, Reflector.GetID(ref instance), GetConnection()))
                {
                    // TODO: Update statement.
                    sql = SQLBuilder.GetUpdateStatement(table, Reflector.ResolvePersistenceMappings(ref instance));
                }
                else
                {
                    sql = SQLBuilder.GetInsertStatement(table, Reflector.ResolvePersistenceMappings(ref instance));
                }              
            }

            // Once updated or inserted, check the ID property and then set it accordingly.
            try
            {
                if (Execute(sql))
                {
                    if (Reflector.GetID(ref instance) == 0)
                    {
                        Reflector.SetID(ref instance, GetLastUpdatedId(table));
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format(Resources.NoInsert, table), ex);
            }

        }
    }
}

