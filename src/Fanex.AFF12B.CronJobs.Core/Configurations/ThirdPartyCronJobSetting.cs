
namespace Fanex.AFF12B.CronJobs.Core.Configurations
{
    public class ThirdPartyCronJobSetting<T> where T : ThirdPartyProductSetting
    {
        public string PullInfoCron { get; set; }

        public T CronJobSetting { get; set; }
    }
}
