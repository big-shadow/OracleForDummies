using OFD.Caching;
using OFD.Properties;

namespace OFD.SQLize
{
    public static class Template
    {
        public static string StoredProcedure(string table)
        {
            string sql = Cache.GetResource("Procedure");

            sql.Replace(Token.TABLE.ToString(), table);
            sql.Replace(Token.PROCEDURE.ToString(), Hasher.Hash(string.Format(Resources.ProcName, table)));

            return sql;
        }

        public static string Trigger(string table)
        {
            string sql = Cache.GetResource("UpdateTrigger");

            sql.Replace(Token.TABLE.ToString(), table);

            return sql;
        }

        public static string Drop(string table)
        {
            string sql = Cache.GetResource("DropTable");

            sql.Replace(Token.TABLE.ToString(), table);

            return sql;
        }
    }
}
