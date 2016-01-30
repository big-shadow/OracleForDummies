using System;
using Oracle.ManagedDataAccess.Client;
using OFD.Properties;

namespace OFD.Transact
{
    /// <summary>
    /// This class contains only boolean members. It exists only to make database related assertions. 
    /// </summary>
    public static class Sniffer
    {
        /// <summary>
        /// Set this to false when in a production environment for performance boosts.
        /// </summary>
        public const bool ON = true;

        public static bool TableExists(string name, OracleConnection con)
        {
            string sql = "SELECT count(*) AS c FROM " + name;
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
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("ORA-00942"))
                    {
                        throw new Exception(string.Format(Resources.NoSniff, name), ex);
                    }
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
                catch (Exception ex)
                {
                    throw new Exception(string.Format(Resources.NoSniff, name), ex);
                }
            }

            con.Close();
            con.Dispose();

            if (Int32.TryParse(count, out output))
            {
                return output > 0;
            }

            return false;
        }
    }
}