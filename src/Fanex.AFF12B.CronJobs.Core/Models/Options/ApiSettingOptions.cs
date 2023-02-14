namespace Fanex.AFF12B.CronJobs.Core.Models.Options
{
    public class ApiSettingOptions
    {
        public bool IsUseProxy { get; set; }

        public string ProxyUrl { get; set; }

        public string ProductApiUrl { get; set; }

        public double ProductApiTimeOut { get; set; }
    }
}
