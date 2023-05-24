using Fanex.AFF12B.CronJobs.Core.Models.Options;
using Fanex.AFF12B.CronJobs.Core.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;
using NetTools;
using System.Net;
using System;
using System.Linq;
using Fanex.Logging;

namespace Fanex.AFF12B.CronJobs.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public sealed class IPFilterAttribute : ActionFilterAttribute
    {
        private readonly ApplicationOptions _applicationOptions;

        public IPFilterAttribute(IOptions<ApplicationOptions> applicationOptionAccessor)
        {
            _applicationOptions = applicationOptionAccessor.Value;

        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.HttpContext.Request.FromInternal())
            {
                return;
            }

            var ipAddress = context.HttpContext.Connection.RemoteIpAddress;
            var whiteListIPList = _applicationOptions.WhiteList;

            var isInWhiteListIPList = !whiteListIPList.Any() || whiteListIPList.Any(_ => IsInRangeIP(_, ipAddress));
            if (!isInWhiteListIPList)
            {
                var result = new ContentResult
                {
                    Content = $"The IP Address: {ipAddress} was blocked"
                };

                context.Result = result;
            }
        }

        private static readonly Func<string, IPAddress, bool> IsInRangeIP = (ipRangeString, ipAddress) =>
        {
            if (!IPAddressRange.TryParse(ipRangeString, out IPAddressRange ipRange))
            {
                Logger.Log.Error($"The WhiteList IP: {ipRangeString} was invalid - client IP: ${ipAddress}");

                return false;
            }

            return ipRange.Contains(ipAddress);
        };
    }
}
