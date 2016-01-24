using System;
using Oracle.ManagedDataAccess.Client;

namespace OFD.Data
{
    /// <summary>
    /// This class contains only boolean members. It exists only to make database related assertions. 
    /// </summary>
    class Sniffer
    {
        public static bool TableExists(string name, OracleConnection con)
        {
            string sql = "SELECT COUNT(*) AS c FROM " + name;
            string count = string.Empty;
            long output;

            using (OracleCommand command = new OracleCommand(sql, con))
            {
                try
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                count = reader["c"].ToString();
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
                catch
                {
                    // Do nothing. It's too soon to dispose.
                }
            }

            con.Close();
            con.Dispose();

            if (Int64.TryParse(count, out output))
            {
                return true;
            }

            return false;
        }

        public static bool RecordExists(string name, int id, OracleConnection con)
        {
            string sql = "SELECT COUNT(*) AS c FROM " + name + " WHERE ID = " + id;
            string count = string.Empty;
            int output;

            using (OracleCommand command = new OracleCommand(sql, con))
            {
                try
                {
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        try
                        {
                            while (reader.Read())
                            {
                                count = reader["c"].ToString();
                            }
                        }
                        finally
                        {
                            reader.Close();
                        }
                    }
                }
                catch
                {
                    // Do nothing. It's too soon to dispose.
                }
            }

            con.Close();
            con.Dispose();

            if (Int32.TryParse(count, out output))
            {
                if(output > 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
