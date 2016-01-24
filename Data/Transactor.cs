using System;
using Oracle.ManagedDataAccess.Client;
using System.Text;

namespace OFD.Data
{
    public static class Transactor
    {
        private static OracleConnection GetConnection()
        { 
            try
            {
                
                var con = new OracleConnection(); 

                con.ConnectionString = @"ORCL =
                  (DESCRIPTION =
                    (ADDRESS = (PROTOCOL = TCP)(HOST = localhost)(PORT = 1521))
                    (CONNECT_DATA =
                      (SERVER = DEDICATED)
                      (SERVICE_NAME = orcl)
                    )
                  )"; 


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
                OracleCommand command = new OracleCommand(sql, con);

                OracleDataReader reader = command.ExecuteReader();
                try
                {
                    while (reader.Read())
                    {
                        
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

