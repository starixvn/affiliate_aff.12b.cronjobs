using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Services
{
    public interface IImportDataService
    {
        Task<string> ImportDataAndTrackTimeToDbAsync(ThirdPartyApiParam param);
    }
}
