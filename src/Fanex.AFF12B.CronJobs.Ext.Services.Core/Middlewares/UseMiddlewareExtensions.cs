using Microsoft.AspNetCore.Builder;

namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Middlewares
{
    public static class UseMiddlewareExtensions
    {
        public static IApplicationBuilder UseJWTokenOnPage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTokenHandler>();
        }
    }
}
