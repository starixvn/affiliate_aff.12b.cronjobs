using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Attributes;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models.DTO;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models.Options;
using Fanex.Athena.AFF12B.CronJobs.Core.Extensions;
using Fanex.Athena.AFF12B.CronJobs.Core.Middleware;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Services;
using Fanex.Data;
using Fanex.Data.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Sentry.AspNetCore;
using System.Security.Claims;
using System.Text;
using Fanex.Athena.AFF12B.CronJobs.Core.Services;
using Fanex.Athena.AFF12B.CronJobs.Core.Configurations;
using Fanex.Athena.AFF12B.CronJobs.Core.Repositories;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.WebHost
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
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();

            services.Configure<SentryAspNetCoreOptions>(Configuration.GetSection("Sentry"));
            services.Configure<TokenManagement>(Configuration.GetSection("tokenManagement"));
            services.Configure<ApplicationOptions>(Configuration.GetSection(ApplicationOptionSettingKey));

            services.Configure<List<User>>(Configuration.GetSection("Users"));

            ConfigFanexData();

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
            //app.UsePingCheck();
            app.UseForwardedHeaders();
            if (string.Equals(Configuration["UseSwagger"], "1"))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            if (string.Equals(Configuration["UseErrorHandler"], "1"))
            {
                app.UseErrorHandler();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWhoAmICheck();
            if (!env.IsDevelopment())
            {
                app.UseHsts();

                app.UseWhen(
                    context => !context.Request.FromInternal(),
                    builder =>
                    {
                        //Fix adding UseHttpsRedirection conditionally via UseWhen fails in ASP.NET Core 6. Fixed in v6.0.9
                        if (builder.ServerFeatures is null)
                        {
                            builder.Properties["server.Features"] = app.ServerFeatures;
                        }

                        builder.UseHttpsRedirection();
                    });
            }

            app.UseStaticResource(env.IsDevelopment());
            app.UseUnderMaintenance();
            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
            app.UseJWTokenOnPage();
            app.UseAuthorization();
            app.UseResponseCaching();
        }

        private void DIRegister(IServiceCollection services)
        {
            services.InitApi(Configuration);
            services.AddUnderMaintenance(Configuration);

            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<ITrackerRepository, TrackerRepository>();

            services.AddScoped<IPFilterAttribute>();
            services.AddScoped<IAppSettings, AppSettings>();
            services.AddScoped<IPullDataService, PullDataService>();
            services.AddScoped<IImportDataService, ImportDataService>();
            services.AddScoped<IUserManagementService, UserManagementService>();
            services.AddScoped<IAuthenticateService, TokenAuthenticationService>();
        }

        private void ConfigAuthentication(IServiceCollection services)
        {
            var token = Configuration.GetSection("tokenManagement").Get<TokenManagement>();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
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
                    };
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

        private void ConfigFanexData()
        {
            var listConnectionName = Configuration.GetSection("ConnectionStrings").GetChildren().Select(_ => _.Key);
            var connectionStrings = new Dictionary<string, ConnectionConfiguration>(listConnectionName
                .Select(connectionName => new KeyValuePair<string, ConnectionConfiguration>(connectionName,
                    new ConnectionConfiguration(connectionName, Configuration.GetConnectionString(connectionName)))
                ));
            DbSettingProviderManager
              .StartNewSession()
              .WithSqlServer(resourcePath: "~/App_Data/SpConfig/SpName.xml")
              .Use(connectionStrings)
              .Run();
        }
    }
}
