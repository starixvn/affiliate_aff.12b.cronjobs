using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public interface IAuthenticateService
    {
        Tuple<bool, Token> IsAuthenticated(TokenRequestParam param);
    }
}
