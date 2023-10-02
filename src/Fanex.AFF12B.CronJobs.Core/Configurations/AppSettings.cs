using Fanex.AFF12B.CronJobs.Core.Extensions;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Fanex.AFF12B.CronJobs.Core.Configurations
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

        public List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>> ProductJobs
        {
            get
            {
                var productJobs = new List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>>();
                _configuration.GetAppSettingsSection("ThirdPartyProductsSetting").Bind(productJobs);

                return productJobs;
            }
        }

        public List<ThirdPartyCronJobSetting<AdjustedDateTimeProductSetting>> ConvertedTimeZoneProductJobs
        {
            get
            {
                var productJobs = new List<ThirdPartyCronJobSetting<AdjustedDateTimeProductSetting>>();
                _configuration.GetAppSettingsSection("ConvertedTimeZoneProductsSetting").Bind(productJobs);

                return productJobs;
            }
        }
    }
}
