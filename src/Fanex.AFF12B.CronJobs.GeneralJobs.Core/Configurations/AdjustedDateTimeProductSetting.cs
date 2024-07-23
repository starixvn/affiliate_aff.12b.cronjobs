
namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.Configurations
{
    public class AdjustedDateTimeProductSetting : ThirdPartyProductSetting
    {
        public int HoursAdjustment { get; set; }

        public string FormatDateTime { get; set; }
    }
}
