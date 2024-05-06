using System;
using System.Collections.Generic;

namespace Fanex.AFF12B.CronJobs.Core.Helpers
{
    public static class DateTimeHelper
    {
        public static IEnumerable<Tuple<DateTime, DateTime>> GetProportionTimeRange(DateTime startTime, DateTime endTime, TimeSpan span)
        {
            if (span == TimeSpan.Zero)
            {
                throw new ArgumentException("Zero time span, this make a infinite repeation to get timerange.");
            }
            var startRequestTime = startTime;
            var endRequestTime = GetSmallerTime(startRequestTime.Add(span), endTime);
            while (startRequestTime < endRequestTime)
            {
                if ((endRequestTime - startRequestTime) == span)
                {
                    yield return new Tuple<DateTime, DateTime>(startRequestTime, endRequestTime);
                }
                startRequestTime = endRequestTime;
                endRequestTime = GetSmallerTime(endRequestTime.Add(span), endTime);
            }
        }

        public static DateTime GetSmallerTime(DateTime compareTime, DateTime endTime)
        {
            return DateTime.Compare(compareTime, endTime) > 0 ? endTime : compareTime;
        }
    }
}
