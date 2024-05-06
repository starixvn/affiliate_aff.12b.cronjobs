using Fanex.AFF12B.CronJobs.Core.Extensions;
using Hangfire.Dashboard;
using Microsoft.IdentityModel.Tokens;

namespace Fanex.AFF12B.CronJobs.Core.Filters
{
    public class DashboardAuthorizationFilter(TokenValidationParameters tokenValidationParameters) : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.IsAuthorize(tokenValidationParameters);
        }
    }
}
