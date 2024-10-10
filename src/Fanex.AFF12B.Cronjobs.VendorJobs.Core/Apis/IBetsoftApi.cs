using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    public interface IBetsoftApi
    {
        [Get("/ptpt/playersBetHistory.do")]
        Task<string> GetRawDataAsync(BetsoftParam param);
    }
}
