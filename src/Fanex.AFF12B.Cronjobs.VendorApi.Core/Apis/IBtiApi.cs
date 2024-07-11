using Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.DTOs;
using Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Apis
{
    public interface IBtiApi
    {
        [Post("/authorize_v2")]
        Task<BtiToken> GetTokenAsync([Body(BodySerializationMethod.UrlEncoded)] BtiTokenParam param);

        [Post("/get-history-bets-paging")]
        Task<string> GetRawDataAsync([Query] string token, [Body(BodySerializationMethod.UrlEncoded)] BaseApiParam param);
    }
}
