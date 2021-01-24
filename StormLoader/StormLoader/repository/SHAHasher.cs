using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Org.BouncyCastle.Crypto.Paddings;

namespace StormLoader.repository
{
    static class SHAHasher
    {
        public static string SHA256Hash(string text)
        {
            
            using (SHA256 sha256Hash = SHA256.Create()) {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(text));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("X2"));
                }
                DbgLog.WriteLine(builder.ToString());
                return builder.ToString() ;
            }

            
        }
    }
}
