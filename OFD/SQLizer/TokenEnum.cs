namespace OFD.SQLize
{
    public sealed class TokenEnum
    {
        private readonly string value;

        public static readonly TokenEnum TABLE = new TokenEnum("--table--");

        private TokenEnum(string v)
        {
            this.value = v;
        }

        public override string ToString()
        {
            return value;
        }
    }
}
