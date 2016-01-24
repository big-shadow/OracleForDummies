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

        public StringBuilder GetCreateTableStatement(string tablename, Dictionary<string, string> columns)
        {
            char delimiter = ' ';
            StringBuilder statement = new StringBuilder("CREATE TABLE " + tablename + " (");

            foreach(KeyValuePair<string, string> column in columns)
            {
                statement.AppendLine(delimiter + ' ' + column.Key + ' ' + column.Value);
                delimiter = ',';
            }

            statement.AppendLine(")");

            return statement;
        }
    }
}

