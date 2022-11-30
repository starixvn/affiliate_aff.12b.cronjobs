using Microsoft.Extensions.Configuration;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Configurations
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;

        public AppSettings(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public int MaxTimeRange => _configuration.GetValue<int>("AppSettings:MaxTimeRange");
        public int MaxItemsInRange => _configuration.GetValue<int>("AppSettings:MaxItemsInRange");
    }
}
