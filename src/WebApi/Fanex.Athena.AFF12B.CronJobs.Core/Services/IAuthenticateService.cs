using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Services
{
    public interface IAuthenticateService
    {
        Tuple<bool, Token> IsAuthenticated(TokenRequest request);
    }
}
