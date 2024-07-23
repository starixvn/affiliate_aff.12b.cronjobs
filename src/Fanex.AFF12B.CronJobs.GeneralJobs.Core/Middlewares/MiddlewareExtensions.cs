using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.RecuringTasks;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Repositories;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net;
using System.Net.Http;

namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void InitGeneralJobsApi(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(ApiOptions));
            var apiSetting = section.Get<GeneralJobsOptions>();

            HttpClientHandler defaultFuncHttpHandler() => new HttpClientHandler()
            {
                Proxy = apiSetting != null && apiSetting.IsUseKbtProxy
                  ? new WebProxy(new Uri(apiSetting.KbtProxyUrl))
                  : null,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            services.AddRefitClient<IThirdPartyProductApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(apiSetting.ProductApiUrl);
                    c.Timeout = TimeSpan.FromSeconds(apiSetting.ProductApiTimeOut);
                })
                .ConfigurePrimaryHttpMessageHandler((Func<HttpClientHandler>)defaultFuncHttpHandler);
        }

        public static void DIGeneralJobsRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.RegisterHangfire("General API");

            services.InitGeneralJobsApi(configuration);
            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<ITrackerRepository, TrackerRepository>();
            services.AddScoped<IPullDataService, PullDataService>();
            services.AddScoped<IImportDataService, ImportDataService>();
            services.AddScoped<IFetchThirdPartyProductDataTask, FetchThirdPartyProductDataTask>();
        }
    }
}
