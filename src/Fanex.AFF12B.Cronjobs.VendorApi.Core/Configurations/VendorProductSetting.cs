using System;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Configurations
{
    public class VendorProductSetting
    {
        public int ProductId { get; set; }
        public DateTime StartTime { get; set; }
        public string FormatDateTime { get; set; }
        public int MinutesAdjustment { get; set; }
        public int MaxMinutesRange { get; set; }
        public int MillisecondsDeplay { get; set; }
    }
}
