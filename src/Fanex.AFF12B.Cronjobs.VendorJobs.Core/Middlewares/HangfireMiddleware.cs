using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks;
using Fanex.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void RegisterVendorJobs(this IApplicationBuilder _, IConfiguration configuration)
        {
            var isRunFetchTask = HangfireHelpers.IsRunFetchTask(configuration);
            if (isRunFetchTask)
            {
                Logger.Log.Info($"AFF 12B Vendor API Cronjobs are registered on - {Dns.GetHostName()}");

                //Jobs
                RegisterTask<FetchBtiProductDataTask>(configuration, "Bti");
                RegisterTask<FetchBetsoftProductDataTask>(configuration, "Betsoft");
                RegisterTask<FetchCrystalGpiProductDataTask>(configuration, "CrystalGpi");
            }
        }

        private static void RegisterTask<T>(IConfiguration configuration, string productName)
            where T : IFetchVendorProductDataTask
        {
            var jobSetting = configuration.GetSettingJobSettingSection(productName);
            if (jobSetting.IsRunJob)
            {
                var name = typeof(T).Name;
                HangfireHelpers.RegisterTask<T>(typeof(T).Name, jobSetting.PullInfoCron, job => job.FetchDataAsync());
            }
        }
    }
}
