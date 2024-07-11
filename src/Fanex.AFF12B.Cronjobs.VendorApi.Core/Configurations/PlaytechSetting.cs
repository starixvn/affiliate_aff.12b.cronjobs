namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Configurations
{
    public class PlaytechSetting : VendorCronJobSetting
    {
        public string ApiKey { get; set; }
        public string FileExtension { get; set; }
        public string FormatExtension { get; set; }
        public string CasinoId { get; set; }
        public string ReportId { get; set; }
        public string ReportBy { get; set; }
    }
}
