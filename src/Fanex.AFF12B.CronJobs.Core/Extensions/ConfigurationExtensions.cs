
using Microsoft.Extensions.Configuration;

namespace Fanex.AFF12B.CronJobs.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetAppSettingsValue<T>(this IConfiguration configuration, string key)
        {
            return configuration.GetValue<T>($"AppSettings:{key}");
        }

        public static IConfigurationSection GetAppSettingsSection(this IConfiguration configuration, string key)
        {
            return configuration.GetSection($"AppSettings:{key}");
        }

        public static string GetSettingPullCronTimeValue(this IConfiguration configuration, string productName)
        {
            return configuration.GetValue<string>($"AppSettings:VendorProductSettings:{productName}Setting:PullCronTime");
        }
    }
}
