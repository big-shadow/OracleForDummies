using OFD.Caching;
using OFD.Properties;

namespace OFD.SQLize
{
    public static class Template
    {
        public static string StoredProcedure(string table)
        {
            string sql = Cache.GetResource("Procedure");

            sql = sql.Replace(Token.TABLE.ToString(), table);
            sql = sql.Replace(Token.PROCEDURE.ToString(), Hasher.Hash(string.Format(Resources.ProcName, table)));

            return sql;
        }

        public static string Trigger(string table)
        {
            string sql = Cache.GetResource("UpdateTrigger");

            sql = sql.Replace(Token.TABLE.ToString(), table);

            return sql;
        }

        public static string Drop(string table)
        {
            string sql = Cache.GetResource("DropTable");

            sql = sql.Replace(Token.TABLE.ToString(), table);

            return sql;
        }
    }
}
