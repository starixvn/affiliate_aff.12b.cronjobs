using Fanex.AFF12B.CronJobs.Core.Extensions;
using Hangfire.Dashboard;
using Microsoft.IdentityModel.Tokens;

namespace Fanex.AFF12B.CronJobs.Core.Filters
{
    public class DashboardAuthorizationFilter : IDashboardAuthorizationFilter
    {
        private readonly TokenValidationParameters tokenValidationParameters;

        public DashboardAuthorizationFilter(TokenValidationParameters tokenValidationParameters)
        {
            this.tokenValidationParameters = tokenValidationParameters;
        }
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();

            return httpContext.IsAuthorize(this.tokenValidationParameters);
        }
    }
}
