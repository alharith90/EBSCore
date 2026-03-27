using System;
using System.Text;

namespace MinAlakherTools
{
    public class MinAlakherEncryption
    {
        public string Encrypt(string plainText, string password)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                return string.Empty;
            }

            var payload = $"{password}:{plainText}";
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
        }

        public string Decrypt(string cipherText, string password)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                return string.Empty;
            }

            var payload = Encoding.UTF8.GetString(Convert.FromBase64String(cipherText));
            var prefix = $"{password}:";
            return payload.StartsWith(prefix, StringComparison.Ordinal)
                ? payload.Substring(prefix.Length)
                : string.Empty;
        }
    }
}
