using Fanex.AFF12B.CronJobs.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Configurations
{
    public class AppSettings : IAppSettings
    {
        private readonly IConfiguration _configuration;
        public AppSettings(
            IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public int MaxMinutesRange => _configuration.GetAppSettingsValue<int>("MaxMinutesRange");

        public bool IsRunFetchTask => _configuration.GetAppSettingsValue<bool>("IsRunFetchTask");

        public bool IsLocal
        {
            get
            {
                var env = _configuration.GetAppSettingsValue<string>("Environment");

                return env.Equals("Development") || env.Equals("LOCAL");
            }
        }
    }
}
