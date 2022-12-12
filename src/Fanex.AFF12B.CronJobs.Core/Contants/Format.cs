
namespace Fanex.AFF12B.CronJobs.Core.Contants
{
    public static class Format
    {
        private const string DEFAULT_DATE_TIME_FORMAT = "yyyy-MM-ddTHH:mm:ss.fff";

        public static string FormatDateTime(this DateTime dateTime, string dateTimeFormat = DEFAULT_DATE_TIME_FORMAT)
        {
            return dateTime.ToString(dateTimeFormat);
        }
    }
}
