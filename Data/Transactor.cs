using System;
using Oracle.ManagedDataAccess.Client;

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
                        command.ExecuteReader();
                    }

                    con.Close();
                }
            }
            finally
            {
                result = false;
            }

            return result;
        }

        private static int GetLastUpdatedId(string tablename)
        {
            string sql = "SELECT ID FROM " + tablename + "WHERE ROWNUM <=1 ORDER BY TIME_UPDATED ASC;";
            int val = -1;

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
                                Int32.TryParse((string)reader["ID"], out val);
                            }
                        }
                        catch
                        {
                            val = -1;
                            reader.Close();
                        }
                    }
                }

                con.Close();
            }

            return val;
        }

        public static int Persist(object instance)
        {
            string table = Reflector.GetClassName(ref instance);
            string sql = string.Empty;

            // Prepare a create table statement if the table doesn't exist. Then make a trigger to write to it after updates.
            if (!Sniffer.TableExists(table, GetConnection()))
            {
                Execute(SQLBuilder.GetCreateTableStatement(table, Reflector.ResolveColumns(ref instance)));
                Execute(Reflector.GetEmbeddedResource("UpdateTrigger.txt").Replace(TokenEnum.TABLE.ToString(), table));
            }

            // If the table exists or was created sucessfully, check to see if it has already been saved once before.
            if (Sniffer.TableExists(table, GetConnection()))
            {
                // If it's been saved once update the record, otherwise insert a new one.
                if(Sniffer.RecordExists(table, Reflector.GetID(ref instance), GetConnection()))
                {
                    // TODO: Update statement.
                }
                else
                {
                    sql = SQLBuilder.GetInsertStatement(table, Reflector.ResolveInsertMappings(ref instance));
                }              
            }

            // Once updated or inserted, check if it was inserted. If yes, set the ID property.
            if (Execute(sql))
            {
                if(Reflector.GetID(ref instance) < 0)
                {
                    Reflector.SetID(ref instance, GetLastUpdatedId(table));
                }
            }

            return 1;
        }
    }
}

