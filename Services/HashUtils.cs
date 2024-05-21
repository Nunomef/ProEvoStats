using System.Security.Cryptography;
using System.Text;

namespace ProEvoStats_EVO7.Services
{
    public class HashUtils
    {
        // Criar um hash SHA256 de uma string para a Password
        public static string ComputeSha256Hash(string rawData)
        {   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
