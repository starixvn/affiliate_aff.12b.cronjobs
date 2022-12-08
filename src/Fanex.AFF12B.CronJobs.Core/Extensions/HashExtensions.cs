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
    }
}
