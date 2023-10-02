using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using System;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Services
{
    public interface IImportDataService
    {
        Task<string> ImportDataToDbAsync<T>(ThirdPartyApiParam param, bool isFromRunningJob = false) where T : Ticket;

        Task<DateTime> GetTimeTrackerAsync(int tpType, string subItem = DefaultValue.String);

        Task UpdateTimeTrackerAsync(TrackerParam param);
    }
}
