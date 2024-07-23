namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations
{
    public class VendorCronJobSetting : VendorProductSetting
    {
        public string ApiUrl { get; set; }
        public string PullInfoCron { get; set; }
        public bool IsImportToTestTable { get; set; }
    }
}
