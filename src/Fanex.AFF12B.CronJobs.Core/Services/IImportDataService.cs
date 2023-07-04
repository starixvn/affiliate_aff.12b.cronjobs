using Fanex.AFF12B.CronJobs.Core.Constant;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public interface IImportDataService
    {
        Task<string> ImportDataToDbAsync<T>(ThirdPartyApiParam param, bool isFromRunningJob = false);

        Task<DateTime> GetTimeTrackerAsync(int tpType, string subItem = DefaultValue.String);

        Task UpdateTimeTrackerAsync(TrackerParam param);
    }
}
