using Fanex.AFF12B.CronJobs.Core.Middlewares;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.AFF12B.CronJobs.Ext.Jobs.Core.Middlewares;
using Fanex.AFF12B.CronJobs.Ext.Jobs.Core.RecuringTasks;
using Hangfire;

namespace Fanex.AFF12B.CronJobs.Ext.Jobs.WebHost
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
            services.RegisterHangfire();

            services.AddHealthChecks()
                .AddCheck<WhoAmIHealthCheck>("WhoAmIHealthCheck");
            services.AddHttpContextAccessor();

            DIRegister(services);
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UsePingCheck();
            app.UseWhoAmICheck();

            app.UseHangfire(Configuration);
            GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
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
