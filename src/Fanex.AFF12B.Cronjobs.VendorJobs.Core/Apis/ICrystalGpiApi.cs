using Fanex.AFF12B.CronJobs.VendorJobs.Core.Configurations;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Apis
{
    public interface ICrystalGpiApi
    {
        [Get("/onlinecasino/integration/gettransactionhistory")]
        Task<string> GetRawDataAsync([FromQuery] CrystalGpiSetting param);
    }
}
