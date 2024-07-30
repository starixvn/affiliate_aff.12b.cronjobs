using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Repositories;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net;
using System.Net.Http;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void InitVendorJobsApi(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(ApiOptions));
            var apiSetting = section.Get<ApiOptions>();

            HttpClientHandler defaultFuncHttpHandler() => new HttpClientHandler()
            {
                Proxy = apiSetting != null && apiSetting.IsUseKbtProxy
                  ? new WebProxy(new Uri(apiSetting.KbtProxyUrl))
                  : null,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };


            services.AddRefitClient<IPlaytechApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration.GetVendorProductSettingsValue<string>("PlaytechSetting:ApiUrl"));
                    c.Timeout = TimeSpan.FromSeconds(apiSetting.ProductApiTimeOut);
                })
                .ConfigurePrimaryHttpMessageHandler((Func<HttpClientHandler>)defaultFuncHttpHandler);

            services.AddRefitClient<ICrystalGpiApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration.GetVendorProductSettingsValue<string>("CrystalGpiSetting:ApiUrl"));
                    c.Timeout = TimeSpan.FromSeconds(apiSetting.ProductApiTimeOut);
                })
                .ConfigurePrimaryHttpMessageHandler((Func<HttpClientHandler>)defaultFuncHttpHandler);

            services.AddRefitClient<IBtiApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration.GetVendorProductSettingsValue<string>("BtiSetting:ApiUrl"));
                    c.Timeout = TimeSpan.FromSeconds(apiSetting.ProductApiTimeOut);
                })
                .ConfigurePrimaryHttpMessageHandler((Func<HttpClientHandler>)defaultFuncHttpHandler);

            services.AddRefitClient<IBetsoftApi>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration.GetVendorProductSettingsValue<string>("BetsoftSetting:ApiUrl"));
                    c.Timeout = TimeSpan.FromSeconds(apiSetting.ProductApiTimeOut);
                })
                .ConfigurePrimaryHttpMessageHandler((Func<HttpClientHandler>)defaultFuncHttpHandler);
        }

        public static void DIVendorJobsRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitVendorJobsApi(configuration);

            services.AddScoped<IImportVendorDataRepository, ImportVendorDataRepository>();
            services.AddScoped<IVendorTrackerRepository, VendorTrackerRepository>();
            services.AddScoped<IVendorTrackerService, VendorTrackerService>();
            services.AddScoped<IBtiService, BtiService>();
            services.AddScoped<IBetsoftService, BetsoftService>();
        }
    }
}
