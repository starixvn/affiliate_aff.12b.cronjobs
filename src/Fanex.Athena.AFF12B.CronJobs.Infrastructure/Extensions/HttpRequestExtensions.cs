using Microsoft.AspNetCore.Http;
using System.Net;

namespace Fanex.Athena.AFF12B.CronJobs.Infrastructure.Extensions
{
    public static class HttpRequestExtensions
    {
        private static readonly string HostName = Dns.GetHostName();

        public static bool FromInternal(this HttpRequest request)
            => HostName.Contains(request?.Host.Host, StringComparison.OrdinalIgnoreCase);
    }
}
