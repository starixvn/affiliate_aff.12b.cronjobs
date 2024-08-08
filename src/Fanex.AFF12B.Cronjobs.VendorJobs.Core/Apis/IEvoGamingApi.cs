using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    [Headers("Authorization: Basic")]
    public interface IEvoGamingApi
    {
        [Post("/api/gamehistory/v1/casino/games")]
        Task<string> GetRawDataAsync([Body(BodySerializationMethod.UrlEncoded)] EvoGamingParam param);
    }
}
