namespace OFD.SQLize
{
    public sealed class Token
    {
        private readonly string value;

        public static readonly Token TABLE = new Token("--table--");
        public static readonly Token PROCEDURE = new Token("--procedure--");

        private Token(string v)
        {
            this.value = v;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
