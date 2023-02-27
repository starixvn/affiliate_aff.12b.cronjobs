using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.Logging;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Middlewares
{
    public class ErrorHandler
    {
        private const int ServerErrorCode = 500;
        private readonly RequestDelegate _next;

        public ErrorHandler(RequestDelegate next)
        {
            _next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            if (context.Features[typeof(IExceptionHandlerFeature)] is IExceptionHandlerFeature error && error.Error != null)
            {
                await HandleError(context, error.Error);
            }

            try
            {
                await _next.Invoke(context);
            }
            catch (Exception exception)
            {
                await HandleError(context, exception);
            }
        }

        private async Task HandleError(HttpContext context, Exception ex)
        {
            context.Response.StatusCode = ServerErrorCode;
            context.Response.ContentType = "application/json";
            var errorMsg = ErrorMessageHelper.GenerateMessage(ex, context);
            Logger.Log.Critical(errorMsg);
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new
            {
                State = "Internal Server Error",
                Msg = "Sorry, an error occurred while processing your request."
            }));
        }
    }
}
