using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.TaiwanApi.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Apis
{
    public interface IThirdPartyProductApi
    {
        [Post("/TPGTickets/GetTPGBetTrans")]
        Task<ThirdPartyProductRaw> GetTPGBetTrans([Body(BodySerializationMethod.UrlEncoded)] ThirdPartyApiParam param);
    }
}
