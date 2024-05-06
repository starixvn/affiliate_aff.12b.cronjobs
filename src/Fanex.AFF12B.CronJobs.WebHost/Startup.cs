using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.Core.RecuringTasks;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.AFF12B.CronJobs.Core.Utilities;
using Fanex.Data.SqlServer;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Fanex.AFF12B.CronJobs.WebHost
{
    public class Startup(IConfiguration configuration)
    {
        private const string ApplicationOptionSettingKey = "ApplicationOptions";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            if (configuration.GetAppSettingsValue<bool>("UseSwagger"))
            {
                // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                services.AddSwaggerGen(c =>
                {
                    var jwtSecurityScheme = new OpenApiSecurityScheme
                    {
                        BearerFormat = "JWT",
                        Name = "JWT Authentication",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.Http,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                        Description = "Put **_ONLY_** your JWT Bearer token on textbox below!",

                        Reference = new OpenApiReference
                        {
                            Id = JwtBearerDefaults.AuthenticationScheme,
                            Type = ReferenceType.SecurityScheme
                        }
                    };

                    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Aff.AFF12B.Cronjobs", Version = "v1" });
                });
            }

            services.Configure<TokenManagementOptions>(configuration.GetSection("TokenManagementOptions"));
            services.Configure<ApiOptions>(configuration.GetSection("ApiOptions"));
            services.Configure<ApplicationOptions>(configuration.GetSection(ApplicationOptionSettingKey));
            services.Configure<List<User>>(configuration.GetSection("Users"));

            services.AddFanexSql(configuration);

            services.AddResponseCaching(options =>
            {
                options.UseCaseSensitivePaths = true;
            });

            ConfigAuthentication(services);

            services.AddAuthorizationBuilder()
                .SetDefaultPolicy(new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build());

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
            if (configuration.GetAppSettingsValue<bool>("UseSwagger"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (configuration.GetAppSettingsValue<bool>("UseErrorHandler"))
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
                    builder => builder.UseHttpsRedirection());
            }

            app.UseUnderMaintenance();
            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
            app.UseRouting();
            app.UseJWTokenOnPage();

            app.UseAuthentication();
            app.UseAuthorization();

            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
            app.UseHangfire(configuration);

            app.UseResponseCaching();
        }

        private void DIRegister(IServiceCollection services)
        {
            services.RegisterHangfire();
            services.InitApi(configuration);
            services.InitLogging(configuration);
            services.AddUnderMaintenance(configuration);

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
                    x.TokenValidationParameters = TokenUtils.GetTokenValidationParameters(configuration);
                    x.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Append("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        }
    }
}
