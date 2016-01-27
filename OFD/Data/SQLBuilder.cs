using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace OFD.Data
{
    /// <summary>
    /// This class builds both DDL and DML statements in the PL-SQL programming language using common conventions. 
    /// </summary>
    public static class SQLBuilder
    {
        public static string Hash(string identifier)
        {
            if(identifier.Length > 30)
            {
                byte[] data;

                using (MD5 md5Hash = MD5.Create())
                {
                    data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(identifier));
                }

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return identifier.Substring(0, 24) + "_" + sBuilder.ToString().Substring(0, 5).ToUpperInvariant();
            }

            return identifier;
        }

        public static string GetCreateTableStatement(string tablename, Dictionary<string, string> columns)
        {
            string delimiter = "";
            StringBuilder statement = new StringBuilder("CREATE TABLE " + Hash(tablename) + " (");

            foreach (KeyValuePair<string, string> column in columns)
            {
                string identifier = Hash(column.Key);

                if (column.Key.Equals("id"))
                {
                    statement.AppendLine(delimiter + identifier + ' ' + column.Value + " GENERATED ALWAYS AS IDENTITY NOT NULL");
                }
                else
                {
                    statement.AppendLine(delimiter + identifier + ' ' + column.Value);
                }

                delimiter = ", ";
            }

            statement.AppendLine(", time_inserted DATE DEFAULT SYSDATE NOT NULL");
            statement.AppendLine(", time_updated DATE DEFAULT SYSDATE NOT NULL");
            statement.AppendLine(")");

            return statement.ToString();
        }

        public static string GetInsertStatement(string tablename, Dictionary<string, string> columns)
        {
            string delimiter = string.Empty;

            StringBuilder statement = new StringBuilder("INSERT INTO " + Hash(tablename) + " (");
            StringBuilder values = new StringBuilder(") VALUES (");

            foreach (KeyValuePair<string, string> column in columns)
            {
                // Skip the ID because it's auto-incrementing.
                if (column.Key.Equals("id"))
                {
                    continue;
                }
                string identifier = Hash(column.Key);

                statement.Append(delimiter + identifier);
                values.Append(delimiter + column.Value);

                delimiter = ", ";
            }

            values.Append(")");

            return statement.ToString() + values.ToString();
        }

        public static string GetUpdateStatement(string tablename, Dictionary<string, string> columns)
        {
            string delimiter = string.Empty;

            StringBuilder statement = new StringBuilder("UPDATE " + Hash(tablename) + " SET ");

            foreach (KeyValuePair<string, string> column in columns)
            {
                // Skip the ID.
                if (column.Key.Equals("id"))
                {
                    continue;
                }

                string identifier = Hash(column.Key);

                statement.Append(delimiter + identifier + " = " + column.Value);
                delimiter = ", ";
            }

            statement.Append(" WHERE ID = " + columns["id"]);

            return statement.ToString();
        }
    }
}

