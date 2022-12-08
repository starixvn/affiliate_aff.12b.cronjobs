using Fanex.AFF12B.CronJobs.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Configurations
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
