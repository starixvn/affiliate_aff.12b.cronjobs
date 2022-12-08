using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Refit;

namespace Fanex.AFF12B.CronJobs.Core.Apis
{
    public interface IThirdPartyProductApi
    {
        [Post("/ThirdPartyGame/GetTPBettingTickets")]
        Task<string> GetTPBettingTickets([Body(BodySerializationMethod.UrlEncoded)] ThirdPartyApiParam param);
    }
}
