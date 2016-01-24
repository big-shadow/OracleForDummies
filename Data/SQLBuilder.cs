using System.Collections.Generic;
using System.Text;

namespace OFD.Data
{
    /// <summary>
    /// This class builds both DDL and DML statements in the PL-SQL programming language using common conventions. 
    /// </summary>
    public class SQLBuilder
    {
        public SQLBuilder()
        {
            
        }

        public static string GetCreateTableStatement(string tablename, Dictionary<string, string> columns)
        {
            char delimiter = ' ';
            StringBuilder statement = new StringBuilder("CREATE TABLE " + tablename + " (");

            foreach(KeyValuePair<string, string> column in columns)
            {
                statement.AppendLine(delimiter + ' ' + column.Key + ' ' + column.Value);
                delimiter = ',';
            }

            statement.AppendLine(")");

            return statement.ToString();
        }

        public static string GetInsertStatement(string tablename, Dictionary<string, string> colval)
        {
            string delimiter = string.Empty;

            StringBuilder statement = new StringBuilder("INSERT INTO " + tablename + " (");
            StringBuilder values = new StringBuilder(") VALUES (");

            foreach (KeyValuePair<string, string> column in colval)
            {
                statement.Append(delimiter + column.Key);
                values.Append(delimiter + column.Value);

                delimiter = ", ";
            }

            values.Append(")");

            return statement.ToString() + values.ToString();
        }
    }
}

