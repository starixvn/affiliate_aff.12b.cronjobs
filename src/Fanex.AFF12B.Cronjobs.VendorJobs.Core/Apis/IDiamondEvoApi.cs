using Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    [Headers("Authorization: Basic c2t5bGluZWIxMjAwMDAwMTowODFiMmIyMWJlYWZjMGVhYmRiYzQ0ZDk2ZDFlMmRlOGI5NDNjOWUw")]
    public interface IDiamondEvoApi
    {
        [Get("/api/gamehistory/v1/casino/games")]
        Task<string> GetRawDataAsync(DiamondEvoParam param);
    }
}
