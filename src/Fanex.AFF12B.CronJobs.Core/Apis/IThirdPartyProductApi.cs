using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Refit;

namespace Fanex.AFF12B.CronJobs.Core.Apis
{
    public interface IThirdPartyProductApi
    {
        [Post("/TPGTickets/GetTPGBetTrans")]
        Task<ThirdPartyProductRaw> GetTPGBetTrans([Body(BodySerializationMethod.UrlEncoded)] ThirdPartyApiParam param);
    }
}
