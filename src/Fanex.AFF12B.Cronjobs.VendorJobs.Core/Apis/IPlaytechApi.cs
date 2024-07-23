using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    public interface IPlaytechApi
    {
        [Get("/reporting/services/report/execute")]
        Task<string> GetRawDataAsync([FromQuery] CrystalGpiSetting param);
    }
}
