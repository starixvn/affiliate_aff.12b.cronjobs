
namespace Fanex.AFF12B.CronJobs.Ext.Jobs.Core.Configurations
{
    public class ThirdPartyCronJobSetting
    {
        public int MinutesAdjustment { get; set; }

        public int MaxMinutesRange { get; set; }

        public byte TpType { get; set; }

        public string SubItem { get; set; }
    }
}
