namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations
{
    public class PlaytechSetting : VendorCronJobSetting
    {
        public string ApiKey { get; set; }
        public string FormatExtension { get; set; }
        public string Casino { get; set; }
        public string SiAccounts { get; set; }
        public string Sdin { get; set; }
        public string ReportId { get; set; }
        public string ReportCode { get; set; }
        public string Outputs { get; set; }
    }
}
