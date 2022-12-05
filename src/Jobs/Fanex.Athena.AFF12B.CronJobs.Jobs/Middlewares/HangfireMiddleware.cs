using Fanex.Athena.AFF12B.CronJobs.Core.Extensions;
using Fanex.Athena.AFF12B.CronJobs.Jobs.Core.Configurations;
using Fanex.Athena.AFF12B.CronJobs.Jobs.Core.Filters;
using Fanex.Athena.AFF12B.CronJobs.Jobs.Core.RecuringTasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;

namespace Fanex.Athena.AFF12B.CronJobs.Jobs.Core.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void RegisterHangfire(this IServiceCollection services)
        {
            // Add Hangfire services.
            services.AddHangfire(config =>
            {
                config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                    .UseColouredConsoleLogProvider()
                    .UseSimpleAssemblyNameTypeSerializer()
                    .UseRecommendedSerializerSettings();

                var memoryOption = new MemoryStorageOptions() { FetchNextJobTimeout = TimeSpan.FromHours(2) };
                config.UseMemoryStorage(memoryOption);
                config.UseFilter(new GlobalHangfireJobExceptionFilterAttribute());
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AllowedAnonymouseAuthorizationFilter() }
            });

            var isRunFetchTask = configuration.GetAppSettingsValue<bool>("IsRunFetchTask");
            if (isRunFetchTask)
            {
                var productJobs = new List<FetchThirdPartyProductSetting>();
                configuration.GetAppSettingsSection("ThirdPartyProductsSetting").Bind(productJobs);

                foreach (var product in productJobs)
                {
                    var cronJobSetting = product.CronJobSetting;
                    RegisterTask<IFetchThirdPartyProductDataTask>(cronJobSetting.CronJobName, product.PullInfoCron, job => job.FetchDataAsync(cronJobSetting));
                }
            }
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

        public class AllowedAnonymouseAuthorizationFilter : IDashboardAuthorizationFilter
        {
            public bool Authorize(DashboardContext context)
            {
                return true;
            }
        }
    }
}
