using System;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations
{
    public class VendorSetting
    {
        public int ProductId { get; set; }
        public DateTime StartTime { get; set; }
        public string FormatDateTime { get; set; }
        public int MinutesAdjustment { get; set; }
        public int MaxMinutesRange { get; set; }
        public int MillisecondsDeplay { get; set; }
    }
}
