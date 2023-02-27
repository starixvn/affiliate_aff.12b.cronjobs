using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Models;
using Fanex.AFF12B.CronJobs.Core.Models.DTO;
using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.RecuringTasks;
using Hangfire;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;

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
            services.RegisterHangfire();
            var isRunAsService = Configuration.GetAppSettingsValue<bool>("IsRunAsService");

            if (!isRunAsService)
            {    
                services.AddControllers();
            }

            if (Configuration.GetAppSettingsValue<bool>("UseSwagger"))
            {
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                //services.AddApiExplorer();
                services.AddSwaggerGen();
            }

            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            services.Configure<ApiSettingOptions>(Configuration.GetSection("ApiSettingOptions"));
            services.Configure<ApplicationOptions>(Configuration.GetSection(ApplicationOptionSettingKey));
            services.Configure<List<User>>(Configuration.GetSection("Users"));

            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = true;
            });

            ConfigAuthentication(services, GetTokenValidationParameters());

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
            var isRunAsService = Configuration.GetAppSettingsValue<bool>("IsRunAsService");
            if (!isRunAsService)
            {
                app.UseRouting();
            }

            app.UseJWTokenOnPage();
            app.UseAuthorization();

            app.UseHangfire(Configuration, GetTokenValidationParameters());
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            app.UseResponseCaching();

            if (!isRunAsService)
            {
                app.UseEndpoints(endpoints =>
                {
                    endpoints.MapDefaultControllerRoute();
                    endpoints.MapGet("/", async context =>
                    {
                        await context.Response.WriteAsync("Hello, Affiliate 12B CronJobs are running!");
                    });
                });
            }
        }

        private void DIRegister(IServiceCollection services)
        {
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

        private void ConfigAuthentication(IServiceCollection services, TokenValidationParameters tokenValidationParameters)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = tokenValidationParameters;
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
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            
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
