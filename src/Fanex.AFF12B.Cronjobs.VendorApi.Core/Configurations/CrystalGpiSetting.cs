namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Configurations
{
    public class CrystalGpiSetting : VendorCronJobSetting
    {
        public string Code { get; set; }
        public byte Type { get; set; }
        public string Password { get; set; }
    }
}
