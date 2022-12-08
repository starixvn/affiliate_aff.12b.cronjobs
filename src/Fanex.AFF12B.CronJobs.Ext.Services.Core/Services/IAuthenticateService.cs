using Fanex.AFF12B.CronJobs.Ext.Services.Core.Models;

namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Services
{
    public interface IAuthenticateService
    {
        Tuple<bool, Token> IsAuthenticated(TokenRequest request);
    }
}
