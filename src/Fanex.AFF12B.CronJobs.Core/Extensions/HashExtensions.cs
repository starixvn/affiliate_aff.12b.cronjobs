using System;
using System.Security.Cryptography;
using System.Text;

namespace Fanex.AFF12B.CronJobs.Core.Extensions
{
    public static class HashExtensions
    {
        public static string Sha256(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            var result = Convert.ToBase64String(hash);

            return result;
        }

        public static string Md5(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return string.Empty;
            }

            var hash = new StringBuilder();

            using var sha = MD5.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }

            return hash.ToString();
        }
    }
}
