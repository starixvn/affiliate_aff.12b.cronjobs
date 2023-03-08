using Fanex.AFF12B.CronJobs.Core.Apis;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Fanex.Logging;
using Fanex.Logging.Sentry.RavenClient;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using ONEbook.UM.AspNetCore;
using ONEbook.UM.Configurations;
using ONEbook.UM.Helpers;
using ONEbook.UM.Services;
using ONEbook.UM.Transport;
using Refit;
using SharpRaven;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Middlewares
{
    public static class MiddlewareExtensions
    {
        public static void UsePingCheck(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.StartsWithSegments("/health"))
                {
                    return;
                }
                await next();
            });
        }

        public static IApplicationBuilder UseErrorHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ErrorHandler>();
        }

        public static IApplicationBuilder UseJWTokenOnPage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTokenHandler>();
        }

        public static void UseStaticResource(this IApplicationBuilder app, bool isDev)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = ctx =>
                {
                    const int OneYear = 60 * 60 * 24 * 365;
                    ctx.Context.Response.Headers[Microsoft.Net.Http.Headers.HeaderNames.CacheControl] = isDev ?
                    "no-cache" : "public,max-age=" + OneYear;
                }
            });
        }

        public static void UseUnderMaintenance(this IApplicationBuilder app)
        {
            app.UseMiddleware<UnderMaintenanceHandler>();
        }

        public static void InitLogging(this IServiceCollection services, IConfiguration configuration)
        {
            LogManager
                    .SetDefaultLogCategory("Affiliate.12B.CronJobs")
                    .Use(new SentryLogging(new SentryEngineOptions
                    {
                        Dsn = new Dsn(configuration.GetAppSettingsValue<string>("SentryDsn"))
                    }));

            services.AddSingleton(Logger.Log);
        }

        public static void UseWhoAmICheck(this IApplicationBuilder app)
        {
            static async Task customResponseWriter(HttpContext context, HealthReport report)
            {
                var result = JsonConvert.SerializeObject(
                    new
                    {
                        status = report.Status.ToString(),
                        monitors = report.Entries.Select(e => new { name = e.Key, status = Enum.GetName(typeof(HealthStatus), e.Value.Status), values = e.Value.Data })
                    }, Formatting.Indented);

                context.Response.ContentType = MediaTypeNames.Application.Json;
                await context.Response.WriteAsync(result);
            }

            var healthCheckOptions = new HealthCheckOptions
            {
                AllowCachingResponses = false,
                Predicate = _ => true,
                ResponseWriter = customResponseWriter
            };

            app.UseHealthChecks("/whoami", healthCheckOptions);
            app.UseHealthChecks("/12monitor", healthCheckOptions);
            app.UseHealthChecks("/12monitor.aspx", healthCheckOptions);
        }

        public static void InitApi(this IServiceCollection services, IConfiguration configuration)
        {
            var apiSetting = configuration.GetSection("ApiSettingOptions").Get<ApiSettingOptions>();

            services.AddRefitClient<IThirdPartyProductApi>()
                .ConfigureHttpClient(c => {
                    c.BaseAddress = new Uri(apiSetting.ProductApiUrl);
                    c.Timeout = TimeSpan.FromSeconds(apiSetting.ProductApiTimeOut);
                });
        }

        public static void InitFanexData(this IServiceCollection _, IConfiguration configuration)
        {
            var env = configuration.GetAppSettingsValue<string>("Environment");
            var listConnectionName = configuration.GetSection("ConnectionStrings").GetChildren().Select(_ => _.Key);
            var connectionStrings = new Dictionary<string, ConnectionConfiguration>(listConnectionName
                .Select(connectionName => new KeyValuePair<string, ConnectionConfiguration>(connectionName,
                    new ConnectionConfiguration(connectionName, configuration.GetConnectionString(connectionName)))
                ));

            DbSettingProviderManager
              .StartNewSession()
              .WithSqlServer(resourcePath: $"~/App_Data/SpConfig/SpName.{env}.xml")
              .Use(connectionStrings)
              .Run();
        }


        public static void AddUnderMaintenance(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(new UMClientConfiguration().Initialize(configuration));
            services.AddSingleton<IUMCacheService, UMCacheService>();
            services.AddSingleton<IMemoryCache, MemoryCache>();
            services.AddSingleton<IUMRestClient, UMRestClient>();
            services.AddSingleton<IHttpContextHelper, ContextInMemoryHelper>();
            services.AddSingleton<IMaintenanceService, MaintenanceService>();
        }


        internal class ContextInMemoryHelper : IHttpContextHelper
        {
            private readonly TimeSpan ByPassUMCacheTime = TimeSpan.FromMinutes(30);
            private readonly IHttpContextAccessor httpContextAccessor;
            private readonly IMemoryCache cache;

            public ContextInMemoryHelper(IHttpContextAccessor httpContextAccessor, IMemoryCache cache)
            {
                this.httpContextAccessor = httpContextAccessor;
                this.cache = cache;
            }

            public string GetSession(string key)
                => cache.TryGetValue(BuildKey(key), out string value) ? value : string.Empty;

            public string GetValueFromQueryString(string queryKey)
                => httpContextAccessor.HttpContext?.Request.Query[queryKey].ToString();

            public void SetSession(string key, string value)
                => cache.Set(BuildKey(key), value, ByPassUMCacheTime);

            private string BuildKey(string key)
                => $"{key}:{httpContextAccessor.HttpContext?.Connection.RemoteIpAddress.ToString()}";
        }
    }
}
