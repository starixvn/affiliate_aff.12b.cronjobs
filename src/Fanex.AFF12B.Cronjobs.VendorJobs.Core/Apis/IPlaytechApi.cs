using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    public interface IPlaytechApi
    {
        [Post("/reporting/services/report/execute")]
        Task<string> GetRawDataAsync([Body(BodySerializationMethod.UrlEncoded)] PlaytechParam param);
    }
}
