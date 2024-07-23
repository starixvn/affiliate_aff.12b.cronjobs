using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    public interface IBtiApi
    {
        [Post("/authorize_v2")]
        Task<BtiToken> GetTokenAsync([Body(BodySerializationMethod.Serialized)] BtiTokenParam param);

        [Post("/get-history-bets-paging")]
        Task<string> GetRawDataAsync([Query] string token, [Body(BodySerializationMethod.Serialized)] BaseParam param);
    }
}
