using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Filters;
using Fanex.AFF12B.CronJobs.Core.RecuringTasks;
using Fanex.Logging;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void RegisterHangfire(this IServiceCollection services)
        {
            // Add Hangfire services.
            services.AddHangfire(config =>
            {
                var memoryOption = new MemoryStorageOptions { FetchNextJobTimeout = TimeSpan.FromHours(6) };
                config.UseMemoryStorage(memoryOption);
                config.UseFilter(new GlobalHangfireJobExceptionFilterAttribute());
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer(option =>
            {
                option.ServerName = $"AFF 12B Cronjobs - {Dns.GetHostName()}";
            });
        }

        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            app.UseHangfireDashboard($"/hangfire", new DashboardOptions
            {
                Authorization = new[] { new DashboardAuthorizationFilter(tokenValidationParameters) }
            });

            var isRunFetchTask = configuration.GetAppSettingsValue<bool>("IsRunFetchTask");
            var serverAddress = configuration.GetAppSettingsValue<string>("RunFetchTaskOnServer");
            var isValidServer = IsValidServer(serverAddress);
            if (isRunFetchTask && isValidServer)
            {
                Logger.Log.Info($"Affiliate 12B CronJobs are registered on {serverAddress} - {Dns.GetHostName()}");

                //Normal Jobs
                var productJobs = new List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>>();
                configuration.GetAppSettingsSection("ThirdPartyProductsSetting").Bind(productJobs);

                foreach (var product in productJobs)
                {
                    var setting = product.CronJobSetting;
                    var jobName = GetJobName(setting);

                    RegisterTask<IFetchThirdPartyProductDataTask>(jobName, product.PullInfoCron, job => job.FetchDataAsync(jobName, setting));
                }

                //converted Transaction Id Jobs
                var transactionIdProductJobs = new List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>>();
                configuration.GetAppSettingsSection("ConvertedTransactionIdProductsSetting").Bind(transactionIdProductJobs);

                foreach (var transactionIdProduct in transactionIdProductJobs)
                {
                    var transactionIdProductSetting = transactionIdProduct.CronJobSetting;
                    var transactionIdJobName = GetJobName(transactionIdProductSetting);

                    RegisterTask<IFetchThirdPartyProductDataTask>(transactionIdJobName, transactionIdProduct.PullInfoCron,
                        job => job.FetchConvertedTransactionIdDataAsync(transactionIdJobName, transactionIdProductSetting));
                }

                //Adjusted DateTime Jobs
                var adjustedProductJobs = new List<ThirdPartyCronJobSetting<AdjustedDateTimeProductSetting>>();
                configuration.GetAppSettingsSection("AdjustedDateTimeProductsSetting").Bind(adjustedProductJobs);

                foreach (var adjustedProduct in adjustedProductJobs)
                {
                    var adjustedProductSetting = adjustedProduct.CronJobSetting;
                    var adjustedJobName = GetJobName(adjustedProductSetting);

                    RegisterTask<IFetchThirdPartyProductDataTask>(adjustedJobName, adjustedProduct.PullInfoCron,
                        job => job.FetchAdjustedDateTimeDataAsync(adjustedJobName, adjustedProductSetting));
                }
            }
        }

        private static bool IsValidServer(string serverAddress)
        {
            var hostAddresss = Dns.GetHostAddresses(Dns.GetHostName());

            return hostAddresss.Any(add => string.Equals(serverAddress, add.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        private static void RegisterTask<T>(string serviceId, string cronExpression, Expression<Func<T, Task>> methodCall)
        {
            if (!string.IsNullOrWhiteSpace(cronExpression))
            {
                RecurringJob.AddOrUpdate<T>(
                            serviceId,
                            methodCall,
                            cronExpression);
            }
        }

        private static string GetJobName(ThirdPartyProductSetting setting)
        {
            return $"{Dns.GetHostName()} - {Product.GetProductNameByTpTypeAndSubItem(setting.TpType, setting.SubItem)}";
        }
    }
}
