using Fanex.AFF12B.CronJobs.Core.Extensions;
using Microsoft.Extensions.Configuration;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetVendorProductSettingsValue<T>(this IConfiguration configuration, string key)
        {
            return configuration.GetAppSettingsValue<T>($"VendorProductSettings:{key}");
        }

        public static IConfigurationSection GetVendorProductSettingsSection(this IConfiguration configuration, string key)
        {
            return configuration.GetAppSettingsSection($"VendorProductSettings:{key}");
        }

        public static string GetSettingPullCronTimeValue(this IConfiguration configuration, string productName)
        {
            return configuration.GetValue<string>($"AppSettings:VendorProductSettings:{productName}Setting:PullInfoCron");
        }
    }
}
