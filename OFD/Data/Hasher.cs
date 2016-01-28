using System.Security.Cryptography;
using System.Text;

namespace OFD
{
    public class Hasher
    {
        public static string Hash(string identifier)
        {
            if (identifier.Length > 30)
            {
                byte[] data;

                using (MD5 md5Hash = MD5.Create())
                {
                    data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(identifier.ToUpperInvariant()));
                }

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return (identifier.Substring(0, 24) + "_" + sBuilder.ToString().Substring(0, 5)).ToUpperInvariant();
            }

            return identifier;
        }
    }
}
