using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.Core.RecuringTasks;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.AFF12B.CronJobs.Core.Services;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.WebHost
{
    public class Startup
    {
        private const string ApplicationOptionSettingKey = "ApplicationOptions";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            if (Configuration.GetAppSettingsValue<bool>("UseSwagger"))
            {
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                services.AddSwaggerGen();
            }

            services.Configure<TokenManagementOptions>(Configuration.GetSection("TokenManagementOptions"));
            services.Configure<ApiOptions>(Configuration.GetSection("ApiOptions"));
            services.Configure<ApplicationOptions>(Configuration.GetSection(ApplicationOptionSettingKey));
            services.Configure<List<User>>(Configuration.GetSection("Users"));

            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = true;
            });

            ConfigAuthentication(services);

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();
            });

            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.All;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });

            services.AddHealthChecks()
                .AddCheck<WhoAmIHealthCheck>("WhoAmIHealthCheck");

            services.AddHttpContextAccessor();
            services.AddHttpsRedirection(options =>
            {
                const int httpsPort = 443;
                options.HttpsPort = httpsPort;
            });

            DIRegister(services);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UsePingCheck();
            app.UseForwardedHeaders();
            if (Configuration.GetAppSettingsValue<bool>("UseSwagger"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (Configuration.GetAppSettingsValue<bool>("UseErrorHandler"))
            {
                app.UseErrorHandler();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWhoAmICheck();
            if (env.IsDevelopment())
            {
                app.UseHsts();

                app.UseWhen(
                    context => !context.Request.FromInternal(),
                    builder =>
                    {
                        builder.Properties["server.Features"] = app.ServerFeatures;
                        builder.UseHttpsRedirection();
                    });
            }

            app.UseStaticResource(env.IsDevelopment());
            app.UseUnderMaintenance();
            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
            app.UseRouting();
            app.UseJWTokenOnPage();
            app.UseAuthorization();

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            app.UseHangfire(Configuration, GetTokenValidationParameters());

            app.UseResponseCaching();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello, Affiliate 12B CronJobs are running!");
                });
            });
        }

        private void DIRegister(IServiceCollection services)
        {
            services.RegisterHangfire();
            services.InitLogging(Configuration);
            services.InitApi(Configuration);
            services.InitFanexData(Configuration);
            services.AddUnderMaintenance(Configuration);

            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<ITrackerRepository, TrackerRepository>();

            services.AddScoped<IPFilterAttribute>();
            services.AddScoped<IAppSettings, AppSettings>();
            services.AddScoped<IPullDataService, PullDataService>();
            services.AddScoped<IImportDataService, ImportDataService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
            services.AddScoped<IFetchThirdPartyProductDataTask, FetchThirdPartyProductDataTask>();
        }

        private void ConfigAuthentication(IServiceCollection services)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = GetTokenValidationParameters();
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }

        private TokenValidationParameters GetTokenValidationParameters()
        {
            var token = Configuration.GetSection("TokenManagementOptions").Get<TokenManagementOptions>();

            return token != null ? new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                TokenDecryptionKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(token.Secret)),
                ValidIssuer = token.Issuer,
                ValidAudience = token.Audience,
                ValidateIssuer = true,
                ValidateAudience = true,
                RoleClaimType = ClaimsIdentity.DefaultNameClaimType,
                RequireExpirationTime = true,
                ValidateLifetime = true,
            } : new TokenValidationParameters();
        }
    }
}
