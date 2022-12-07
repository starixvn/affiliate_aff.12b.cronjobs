using Microsoft.AspNetCore.Builder;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Middlewares
{
    public static class UseMiddlewareExtensions
    {
        public static IApplicationBuilder UseJWTokenOnPage(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JWTokenHandler>();
        }
    }
}
