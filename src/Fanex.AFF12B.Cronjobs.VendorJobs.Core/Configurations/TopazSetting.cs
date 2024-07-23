namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations
{
    public class TopazSetting : VendorCronJobSetting
    {
        public string CompanyId { get; set; }
        public string AuthUsername { get; set; }
        public string AuthSecret { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
