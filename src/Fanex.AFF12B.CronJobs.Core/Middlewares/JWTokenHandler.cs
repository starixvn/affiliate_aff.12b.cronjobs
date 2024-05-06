using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Fanex.AFF12B.CronJobs.Core.Middlewares
{
    public class JWTokenHandler(RequestDelegate next)
    {
        const string HeaderAuthKey = "Authorization";
        const string SuffixTokenValue = "Bearer ";

        public async Task Invoke(HttpContext context)
        {
            if (!context.Request.Headers.ContainsKey(HeaderAuthKey))
            {
                var token = GetToken(context);
                if (!string.IsNullOrEmpty(token))
                {
                    context.Request.Headers.Append(HeaderAuthKey, SuffixTokenValue + token);
                }
            }

            await next.Invoke(context);
        }

        private static string GetToken(HttpContext context)
        {
            context.Request.Query.TryGetValue("access_token", out StringValues queryData);
            return queryData.FirstOrDefault();
        }
    }
}
