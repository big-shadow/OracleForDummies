using System;
using Oracle.ManagedDataAccess.Client;

namespace OFD.Data
{
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

        private static void CloseConnection(ref OracleConnection con)
        {
            con.Close();
            con.Dispose();
        }

        public static void Execute(string sql)
        {
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
                                object myField = reader["MYFIELD"];
                                Console.WriteLine(myField);
                            }
                        }
                        finally
                        {
                            // always call Close when done reading.
                            reader.Close();
                        }
                    }
                }
            }
        }

        public static bool TableExists(string name)
        {

            return true;
        }
    }
}

