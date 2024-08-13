using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    public interface IPragmaticPlayApi
    {
        [Get("/IntegrationService/v3/DataFeeds/gamerounds/finished")]
        Task<string> GetRawDataAsync(PragmaticPlayParam param);
    }
}
