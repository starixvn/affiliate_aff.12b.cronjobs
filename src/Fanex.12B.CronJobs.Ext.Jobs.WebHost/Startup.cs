using Fanex.Athena.AFF12B.CronJobs.Core.Middlewares;
using Fanex.Athena.AFF12B.CronJobs.Core.Repositories;
using Fanex.Athena.AFF12B.CronJobs.Core.Services;
using Fanex.Athena.AFF12B.CronJobs.Jobs.Core.Middlewares;
using Fanex.Athena.AFF12B.CronJobs.Jobs.Core.RecuringTasks;
using Hangfire;
using Microsoft.AspNetCore.HttpOverrides;

namespace Fanex.Athena.AFF12B.CronJobs.Jobs.WebHost
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.RegisterHangfire();

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
            app.UseHangfire(Configuration);
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });

            if (string.Equals(Configuration["UseErrorHandler"], "1"))
            {
                app.UseErrorHandler();
            }
            else
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseWhoAmICheck();
            app.UseUnderMaintenance();
            app.UseStaticResource(env.IsDevelopment());
            app.UseStatusCodePagesWithReExecute("/Error");
            app.UseResponseCaching();
        }

        private void DIRegister(IServiceCollection services)
        {
            services.InitLogging(Configuration);
            services.InitApi(Configuration);
            services.InitFanexData(Configuration);
            services.AddUnderMaintenance(Configuration);

            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<ITrackerRepository, TrackerRepository>();

            services.AddScoped<IPullDataService, PullDataService>();
            services.AddScoped<IImportDataService, ImportDataService>();
            services.AddScoped<IFetchThirdPartyProductDataTask, FetchThirdPartyProductDataTask>();
        }
    }
}
