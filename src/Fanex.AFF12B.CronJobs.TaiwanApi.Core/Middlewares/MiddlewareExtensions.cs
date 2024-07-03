using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Apis;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.Options;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Net;
using System.Net.Http;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void InitApi(this IServiceCollection services, IConfiguration configuration)
        {
            var section = configuration.GetSection(nameof(ApiOptions));
            var apiSetting = section.Get<TaiwanApiOptions>();

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
    }
}
