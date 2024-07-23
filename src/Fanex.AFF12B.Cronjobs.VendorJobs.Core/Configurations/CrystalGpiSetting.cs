namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations
{
    public class CrystalGpiSetting : VendorCronJobSetting
    {
        public string Code { get; set; }
        public byte Type { get; set; }
        public string Password { get; set; }
    }
}
