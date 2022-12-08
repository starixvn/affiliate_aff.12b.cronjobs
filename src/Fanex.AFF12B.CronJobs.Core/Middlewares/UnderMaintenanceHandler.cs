using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using ONEbook.UM.Services;
using System.Net;

namespace Fanex.AFF12B.CronJobs.Core.Middlewares
{
    public class UnderMaintenanceHandler
    {
        private const string AjaxRequestMsgFormat = "System is currently unavailable due to scheduled maintenance. This downtime will last from {0} to {1} (HK Time GMT +8)";

        private const string HtmlRequestMsgFormat = "<div style='text-align:center;padding-top:10%'>System is currently unavailable due to scheduled maintenance." +
            "<br /> This downtime will last from <b>{0}</b> &rarr; <b>{1}</b> (HK Time GMT +8). Thanks for your patience!</div>";

        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        private readonly RequestDelegate _next;
        private readonly IMaintenanceService _maintenanceService;

        public UnderMaintenanceHandler(RequestDelegate next, IMaintenanceService maintenanceService)
        {
            _next = next;
            _maintenanceService = maintenanceService;
        }

        private static bool IsAjaxRequest(HttpRequest request)
            => request.Headers.TryGetValue(RequestedWithHeader, out StringValues headerValue)
               && string.Equals(headerValue.FirstOrDefault(), XmlHttpRequest, StringComparison.OrdinalIgnoreCase);

        public async Task Invoke(HttpContext context)
        {
            var isUM = await _maintenanceService.IsUnderMaintenanceAsync();
            if (isUM)
            {
                var umTime = await _maintenanceService.GetUnderMaintenanceTimeAsync();
                string msg;
                string contentType;
                if (IsAjaxRequest(context.Request))
                {
                    contentType = "application/json";
                    msg = JsonConvert.SerializeObject(new
                    {
                        msg = string.Format(AjaxRequestMsgFormat, umTime.From, umTime.To),
                        isSuccess = false
                    });
                }
                else
                {
                    contentType = "text/html";
                    msg = string.Format(HtmlRequestMsgFormat, umTime.From, umTime.To);
                }

                context.Response.ContentType = contentType;
                context.Response.StatusCode = (int)HttpStatusCode.ServiceUnavailable;
                await context.Response.WriteAsync(msg);
            }
            else
            {
                await _next.Invoke(context);
            }
        }
    }
}
