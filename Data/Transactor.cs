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

                //                con.ConnectionString = @"Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=127.0.0.1)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=orcl)));
                //User Id = system; Password = 1234;";

                //con.ConnectionString = @"Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = 127.0.0.1)(PORT = 1521))(CONNECT_DATA = (SERVICE_NAME = orcl)));
                //uid = system; pwd = 1234;";

                //SERVER = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = MyHost)(PORT = MyPort))(CONNECT_DATA = (SERVICE_NAME = MyOracleSID)));
                //uid = myUsername; pwd = myPassword;

                con.Open(); 
                return con;
            }
            catch (Exception ex)
            {
                string thing = ex.Message;
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
                                string myField = (string)reader["MYFIELD"];
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
    }
}

