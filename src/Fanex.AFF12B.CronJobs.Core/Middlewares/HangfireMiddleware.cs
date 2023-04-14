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
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Fanex.Logging;
using System.IO;

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
            services.AddHangfireServer(option =>
            {
                option.ServerName = "AFF 12B Cronjobs";
            });
        }

        public static void UseHangfire(this IApplicationBuilder app, IConfiguration configuration, TokenValidationParameters tokenValidationParameters)
        {
            app.UseHangfireDashboard($"/hangfire", new DashboardOptions
            {
                Authorization = new[] { new AuthorizationFilter(tokenValidationParameters) }
            });

            var isRunFetchTask = configuration.GetAppSettingsValue<bool>("IsRunFetchTask");
            var serverPath = configuration.GetAppSettingsValue<string>("PathOnServer");
            var serverAddress = configuration.GetAppSettingsValue<string>("RunFetchTaskOnServer");
            var isValidServer = IsValidServer(serverPath, serverAddress);
            if (isRunFetchTask && isValidServer)
            {
                Logger.Log.Info($"Affiliate 12B CronJobs are registered on {serverAddress} - {Dns.GetHostName()}");
                var productJobs = new List<FetchThirdPartyProductSetting>();
                configuration.GetAppSettingsSection("ThirdPartyProductsSetting").Bind(productJobs);

                foreach (var product in productJobs)
                {
                    var setting = product.CronJobSetting;
                    var jobName = $"{Dns.GetHostName()} - {Product.GetProductNameByTpTypeAndSubItem(setting.TpType, setting.SubItem)}";

                    RegisterTask<IFetchThirdPartyProductDataTask>(jobName, product.PullInfoCron, job => job.FetchDataAsync(jobName, setting));
                }
            }
        }

        private static bool IsValidServer(string serverPath, string serverAddress)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var hostAddresss = Dns.GetHostAddresses(Dns.GetHostName()); 

            return baseDirectory.Contains(serverPath) && hostAddresss.Any(add => string.Equals(serverAddress, add.ToString(), StringComparison.OrdinalIgnoreCase));
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

            private readonly TokenValidationParameters tokenValidationParameters;

            public AuthorizationFilter(TokenValidationParameters tokenValidationParameters)
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

                var shouldSetCookie = false;

                string accessToken;
                if (httpContext.Request.Query.ContainsKey("access_token"))
                {
                    shouldSetCookie = true;
                    accessToken = httpContext.Request.Query["access_token"].FirstOrDefault();
                }
                else
                {
                    accessToken = httpContext.Request.Cookies[Aff12BHangFireCookieName];
                }

                if (!string.IsNullOrEmpty(accessToken))
                {
                    try
                    {
                        var handler = new JwtSecurityTokenHandler();
                        var claims = handler.ValidateToken(accessToken, this.tokenValidationParameters, out _);
                        if (claims.Identity.IsAuthenticated)
                        {
                            if (shouldSetCookie)
                            {
                                httpContext.Response.Cookies.Append(Aff12BHangFireCookieName,
                                accessToken,
                                new CookieOptions
                                {
                                    Expires = DateTime.Now.AddMinutes(CookieExpirationMinutes),
                                    HttpOnly = true,
                                    Secure = true,
                                    SameSite = SameSiteMode.Strict
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
