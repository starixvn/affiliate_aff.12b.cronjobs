using Fanex.AFF12B.CronJobs.Core.Contants;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Filters;
using Fanex.AFF12B.CronJobs.Core.RecuringTasks;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace Fanex.AFF12B.CronJobs.Core.Middlewares
{
    public static class HangfireMiddleware
    {
        public static void RegisterHangfire(this IServiceCollection services)
        {
            // Add Hangfire services.
            services.AddHangfire(config =>
            {
                var memoryOption = new MemoryStorageOptions() { FetchNextJobTimeout = TimeSpan.FromHours(6) };
                config.UseMemoryStorage(memoryOption);
                config.UseFilter(new GlobalHangfireJobExceptionFilterAttribute());
            });

            // Add the processing server as IHostedService
            services.AddHangfireServer();
        }

        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilter(tokenValidationParameters) }
            });

            var isRunFetchTask = configuration.GetAppSettingsValue<bool>("IsRunFetchTask");
            if (isRunFetchTask)
            {
                var productJobs = new List<FetchThirdPartyProductSetting>();
                configuration.GetAppSettingsSection("ThirdPartyProductsSetting").Bind(productJobs);

                foreach (var product in productJobs)
                {
                    var setting = product.CronJobSetting;
                    var jobName = $"{Product.GetProductNameByTpTypeAndSubItem(setting.TpType, setting.SubItem)} Cron Job";

                    RegisterTask<IFetchThirdPartyProductDataTask>(jobName, product.PullInfoCron, job => job.FetchDataAsync(jobName, setting));
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

        public class AuthorizationFilter : IDashboardAuthorizationFilter
        {
            private static readonly int CookieExpirationMinutes = 30;
            private static readonly string Aff12BHangFireCookieName = "Aff_12B_HangFire_Cookie";

            private TokenValidationParameters tokenValidationParameters;

            public AuthorizationFilter(TokenValidationParameters tokenValidationParameters, string role = null)
            {
                this.tokenValidationParameters = tokenValidationParameters;
            }
            public bool Authorize(DashboardContext context)
            {
                var httpContext = context.GetHttpContext();
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    return true;
                }

                var accessToken = String.Empty;
                var shouldSetCookie = false;

                if (httpContext.Request.Query.ContainsKey("access_token"))
                {
                    shouldSetCookie = true;
                    accessToken = httpContext.Request.Query["access_token"].FirstOrDefault();
                } else
                {
                    accessToken = httpContext.Request.Cookies[Aff12BHangFireCookieName];
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var claims = handler.ValidateToken(accessToken, this.tokenValidationParameters, out var _);
                        if (claims.Identity.IsAuthenticated)
                        {
                            if (shouldSetCookie)
                            {
                                httpContext.Response.Cookies.Append(Aff12BHangFireCookieName,
                                accessToken,
                                new CookieOptions
                                {
                                    Expires = DateTime.Now.AddMinutes(CookieExpirationMinutes)
                                });
                            }
                            return true;
                        }
                    }
                    catch (Exception) {}
                }

                return false;
            }
        }
    }
}
