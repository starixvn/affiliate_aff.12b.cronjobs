using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Filters;
using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.Core.Utilities;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Configurations;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.RecuringTasks;
using Fanex.Logging;
using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
        {
            var tokenValidationParameters = TokenUtils.GetTokenValidationParameters(configuration);

            app.UseHangfireDashboard($"/hangfire", new DashboardOptions
            {
                Authorization = new[] { new DashboardAuthorizationFilter(tokenValidationParameters) }
            });

            var isRunFetchTask = HangfireHelpers.IsRunFetchTask(configuration);
            if (isRunFetchTask)
            {
                Logger.Log.Info($"AFF 12B Taiwan API Cronjobs are registered on {Dns.GetHostName()}");

                //Normal Jobs
                var productJobs = new List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>>();
                configuration.GetAppSettingsSection("ThirdPartyProductsSetting").Bind(productJobs);

                foreach (var product in productJobs)
                {
                    var setting = product.CronJobSetting;
                    var jobName = GetJobName(setting);

                    HangfireHelpers.RegisterTask<IFetchThirdPartyProductDataTask>(jobName, product.PullInfoCron, job => job.FetchDataAsync(jobName, setting));
                }

                //converted Transaction Id Jobs
                var transactionIdProductJobs = new List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>>();
                configuration.GetAppSettingsSection("ConvertedTransactionIdProductsSetting").Bind(transactionIdProductJobs);

                foreach (var transactionIdProduct in transactionIdProductJobs)
                {
                    var transactionIdProductSetting = transactionIdProduct.CronJobSetting;
                    var transactionIdJobName = GetJobName(transactionIdProductSetting);

                    HangfireHelpers.RegisterTask<IFetchThirdPartyProductDataTask>(transactionIdJobName, transactionIdProduct.PullInfoCron,
                        job => job.FetchConvertedTransactionIdDataAsync(transactionIdJobName, transactionIdProductSetting));
                }

                //Adjusted DateTime Jobs
                var adjustedProductJobs = new List<ThirdPartyCronJobSetting<AdjustedDateTimeProductSetting>>();
                configuration.GetAppSettingsSection("AdjustedDateTimeProductsSetting").Bind(adjustedProductJobs);

                foreach (var adjustedProduct in adjustedProductJobs)
                {
                    var adjustedProductSetting = adjustedProduct.CronJobSetting;
                    var adjustedJobName = GetJobName(adjustedProductSetting);

                    HangfireHelpers.RegisterTask<IFetchThirdPartyProductDataTask>(adjustedJobName, adjustedProduct.PullInfoCron,
                        job => job.FetchAdjustedDateTimeDataAsync(adjustedJobName, adjustedProductSetting));
                }
            }
        }

        private static string GetJobName(ThirdPartyProductSetting setting)
        {
            return $"{Dns.GetHostName()} - {Product.GetProductNameByTpTypeAndSubItem(setting.TpType, setting.SubItem)}";
        }
    }
}
