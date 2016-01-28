using System.Collections.Generic;
using System.Text;

namespace OFD.Data
{
    /// <summary>
    /// This class builds both DDL and DML statements in the PL-SQL programming language using common conventions. 
    /// </summary>
    public static class SQLBuilder
    {
        public static string GetCreateTableStatement(string tablename, Dictionary<string, string> columns)
        {
            string delimiter = "";
            StringBuilder statement = new StringBuilder("CREATE TABLE " + tablename + " (");

            foreach (KeyValuePair<string, string> column in columns)
            {
                string identifier = column.Key;

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

            StringBuilder statement = new StringBuilder("INSERT INTO " + tablename + " (");
            StringBuilder values = new StringBuilder(") VALUES (");

            foreach (KeyValuePair<string, string> column in columns)
            {
                // Skip the ID because it's auto-incrementing.
                if (column.Key.Equals("id"))
                {
                    continue;
                }
                string identifier = column.Key;

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

            StringBuilder statement = new StringBuilder("UPDATE " + tablename + " SET ");

            foreach (KeyValuePair<string, string> column in columns)
            {
                // Skip the ID.
                if (column.Key.Equals("id"))
                {
                    continue;
                }

                string identifier = column.Key;

                statement.Append(delimiter + identifier + " = " + column.Value);
                delimiter = ", ";
            }

            statement.Append(" WHERE ID = " + columns["id"]);

            return statement.ToString();
        }
    }
}

