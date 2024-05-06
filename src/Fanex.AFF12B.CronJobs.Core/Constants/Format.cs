namespace Fanex.AFF12B.CronJobs.Core.Constants
{
    public static class Format
    {
        private const string DEFAULT_DATE_TIME_FORMAT = "yyyy-MM-dd'T'HH:mm:ss";

        public static string DefaultFomartDateTime => DEFAULT_DATE_TIME_FORMAT;

        public static string FormatDateTime(this DateTime dateTime, string dateTimeFormat = DEFAULT_DATE_TIME_FORMAT)
        {
            return dateTime.ToString(dateTimeFormat);
        }
    }
}
