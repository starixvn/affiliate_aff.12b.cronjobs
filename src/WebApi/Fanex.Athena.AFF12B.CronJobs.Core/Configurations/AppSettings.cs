using Fanex.Athena.AFF12B.CronJobs.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Configurations
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
    }
}
