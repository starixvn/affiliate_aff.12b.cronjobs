using Fanex.AFF12B.Cronjobs.VendorApi.Core.Configurations;
using Microsoft.AspNetCore.Mvc;
using Refit;
using System.Threading.Tasks;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Apis
{
    public interface IBetsoftApi
    {
        [Get("/ptpt/playersBetHistory.do")]
        Task<string> GetRawDataAsync([FromQuery] BetsoftSetting param);

    }
}
