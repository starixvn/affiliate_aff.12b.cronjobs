using Fanex.AFF12B.CronJobs.Core.Models;
using System;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public interface IAuthenticateService
    {
        Tuple<bool, Token> IsAuthenticated(TokenRequest request);
    }
}
