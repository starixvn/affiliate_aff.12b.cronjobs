using Fanex.AFF12B.CronJobs.Core.Models.DTO;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Apis
{
    public interface IThirdPartyProductApi
    {
        [Post("/ThirdPartyGame/GetTPBettingTickets")]
        Task<ThirdPartyProductRaw> GetTPBettingTickets([Body(BodySerializationMethod.UrlEncoded)] ThirdPartyApiParam param);
    }
}
