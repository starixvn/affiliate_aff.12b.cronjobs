using Fanex.AFF12B.CronJobs.Core.Constants;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace Fanex.AFF12B.CronJobs.Core.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string IPv4Regex = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        private static readonly string HostName = Dns.GetHostName();

        public static bool FromInternal(this HttpRequest request)
            => HostName.Contains(request.Host.Host, StringComparison.OrdinalIgnoreCase);

        public static string GetClientIP(this HttpRequest request)
        {
            string clientIPs = null;

            if (request.Headers.TryGetValue(HeaderKey.X_ACTUAL_IP, out var actualIPs))
            {
                clientIPs = actualIPs;
            }

            if (string.IsNullOrWhiteSpace(clientIPs) && request.Headers.TryGetValue(HeaderKey.X_FORWARDED_FOR, out var forwardedIPs))
            {
                clientIPs = forwardedIPs;
            }

            var validIp = GetValidIPAddress(clientIPs);
            if (string.IsNullOrWhiteSpace(validIp))
            {
                validIp = request.HttpContext.Connection.RemoteIpAddress.ToString();
            }

            return validIp.Equals("::1") ? "127.0.0.1" : validIp;
        }

        private static string GetValidIPAddress(string ips)
        {
            if (string.IsNullOrEmpty(ips))
            {
                return null;
            }

            var ipList = ips.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(i => i.Trim())
                        .Where(i => Regex.IsMatch(i, IPv4Regex, RegexOptions.None, TimeSpan.FromSeconds(2)));

            return ipList.FirstOrDefault();
        }
    }
}
