using Microsoft.AspNetCore.Http;
using System;
using System.Net;

namespace Fanex.AFF12B.CronJobs.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        private static readonly string HostName = Dns.GetHostName();

        public static bool FromInternal(this HttpRequest request)
            => HostName.Contains(request.Host.Host, StringComparison.OrdinalIgnoreCase);
    }
}
