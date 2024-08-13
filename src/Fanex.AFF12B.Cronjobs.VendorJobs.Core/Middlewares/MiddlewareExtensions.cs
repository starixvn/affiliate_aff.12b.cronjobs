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
            var apiSetting = configuration.GetSection(nameof(ApiOptions)).Get<ApiOptions>();

            HttpClientHandler defaultFuncHttpHandler() => new HttpClientHandler()
            {
                Proxy = apiSetting != null && apiSetting.IsUseKbtProxy
                  ? new WebProxy(new Uri(apiSetting.KbtProxyUrl))
                  : null,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };

            services.RegisterVendorApi<IPlaytechApi>(configuration, defaultFuncHttpHandler, "Playtech", apiSetting.ProductApiTimeOut);
            services.RegisterVendorApi<ICrystalGpiApi>(configuration, defaultFuncHttpHandler, "CrystalGpi", apiSetting.ProductApiTimeOut);
            services.RegisterVendorApi<IBtiApi>(configuration, defaultFuncHttpHandler, "Bti", apiSetting.ProductApiTimeOut);
            services.RegisterVendorApi<IBetsoftApi>(configuration, defaultFuncHttpHandler, "Betsoft", apiSetting.ProductApiTimeOut);
            services.RegisterVendorApi<IBetRadarApi>(configuration, defaultFuncHttpHandler, "BetRadar", apiSetting.ProductApiTimeOut);
            services.RegisterVendorApi<IDiamondEvoApi>(configuration, defaultFuncHttpHandler, "DiamondEvo", apiSetting.ProductApiTimeOut);
            services.RegisterVendorApi<IPragmaticPlayApi>(configuration, defaultFuncHttpHandler, "PragmaticPlay", apiSetting.ProductApiTimeOut);
        }

        private static void RegisterVendorApi<T>(this IServiceCollection services, IConfiguration configuration, Func<HttpClientHandler> defaultFuncHttpHandler, string productName, double timeOut = 150) where T : class
        {
            services.AddRefitClient<T>()
                .ConfigureHttpClient(c =>
                {
                    c.BaseAddress = new Uri(configuration.GetVendorProductApiUrlValue(productName));
                    c.Timeout = TimeSpan.FromSeconds(timeOut);
                })
                .ConfigurePrimaryHttpMessageHandler(defaultFuncHttpHandler);
        }

        public static void DIVendorJobsRegister(this IServiceCollection services, IConfiguration configuration)
        {
            services.InitVendorJobsApi(configuration);

            services.AddScoped<IImportVendorDataRepository, ImportVendorDataRepository>();
            services.AddScoped<IVendorTrackerRepository, VendorTrackerRepository>();

            services.AddScoped<IVendorTrackerService, VendorTrackerService>();
            services.AddScoped<IBtiService, BtiService>();
            services.AddScoped<IBetsoftService, BetsoftService>();
            services.AddScoped<IPlaytechService, PlaytechService>();
            services.AddScoped<ICrystalGpiService, CrystalGpiService>();
            services.AddScoped<IBetRadarService, BetRadarService>();
            services.AddScoped<IDiamondEvoService, DiamondEvoService>();
            services.AddScoped<IPragmaticPlayService, PragmaticPlayService>();
        }
    }
}
