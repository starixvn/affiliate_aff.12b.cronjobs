using System;

namespace Fanex.AFF12B.CronJobs.Core.Extensions
{
    public static class StringExtension
    {
        public static bool IsMatch(this string value, string matchedValue)
        {
            return string.Equals(value, matchedValue, StringComparison.OrdinalIgnoreCase);
        }
    }
}
