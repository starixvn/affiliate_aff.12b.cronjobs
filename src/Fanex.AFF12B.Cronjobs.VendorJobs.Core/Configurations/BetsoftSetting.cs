namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations
{
    public class BetsoftSetting : VendorCronJobSetting
    {
        public string Password { get; set; }

        public string[] BankIds { get; set; }
    }
}
