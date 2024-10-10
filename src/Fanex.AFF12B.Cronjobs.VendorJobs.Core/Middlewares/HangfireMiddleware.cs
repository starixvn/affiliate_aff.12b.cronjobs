using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.RecuringTasks;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
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
                RegisterVendorTask<IBetsoftService>(configuration, "Betsoft");
            }
        }

        private static void RegisterVendorTask<T>(IConfiguration configuration, string productName)
            where T : IVendorService
        {
            var jobSetting = configuration.GetSettingJobSettingSection(productName);
            if (jobSetting.IsRunJob)
            {
                HangfireHelpers.RegisterTask<FetchVendorProductDataTask<T>>($"Fetch{productName}ProductDataTask", jobSetting.PullInfoCron, job => job.FetchDataAsync());
            }
        }
    }
}
