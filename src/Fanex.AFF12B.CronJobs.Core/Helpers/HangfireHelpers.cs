using Fanex.AFF12B.CronJobs.Core.Extensions;
using Hangfire;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Helpers
{
    public static class HangfireHelpers
    {
        public static bool IsRunFetchTask(IConfiguration configuration, string key = null)
        {
            var isRunFetchTask = configuration.GetAppSettingsValue<bool>("IsRunFetchTask");
            var serverAddress = configuration.GetAppSettingsValue<string>(key ?? "RunFetchTaskOnServer");
            var hostAddresss = Dns.GetHostAddresses(Dns.GetHostName());
            var isValidServer = Array.Exists(hostAddresss, add => serverAddress.IsMatch(add.ToString()));

            return isRunFetchTask && isValidServer;
        }

        public static void RegisterTask<T>(string serviceId, string cronExpression, Expression<Func<T, Task>> methodCall)
        {
            if (!string.IsNullOrWhiteSpace(cronExpression))
            {
                RecurringJob.AddOrUpdate<T>(
                            serviceId,
                            methodCall,
                            cronExpression);
            }
        }
    }
}
