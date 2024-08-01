namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations
{
    public class CrystalGpiSetting : VendorCronJobSetting
    {
        public string Game { get; set; }
        public string Status { get; set; }
        public string VendorName { get; set; }
        public string Password { get; set; }
        public int PageSize { get; set; }
    }
}
