using Microsoft.Extensions.Configuration;

namespace Fanex.AFF12B.CronJobs.VendorApi.Core.Extensions
{
    public static class ConfigurationExtensions
    {
        public static T GetVendorProductSettingsValue<T>(this IConfiguration configuration, string key)
        {
            return configuration.GetValue<T>($"VendorProductSettings:{key}");
        }

        public static IConfigurationSection GetVendorProductSettingsSection(this IConfiguration configuration, string key)
        {
            return configuration.GetSection($"VendorProductSettings:{key}");
        }
    }
}
