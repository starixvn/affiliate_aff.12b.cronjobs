using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    public class PullDataController(
        IAppSettings appSettings,
        IPullDataService pullDataService) : BaseController(appSettings)
    {
        [HttpGet("api/GetRawData")]
        public async Task<ActionResult> GetRawDataListAsync([FromQuery] ThirdPartyApiParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var rawDataList = await pullDataService.GetRawDataListAsync(param);

                return new JsonResult(rawDataList);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("api/GetJsonData")]
        public async Task<ActionResult> GetJsonDataListAsync([FromQuery] ThirdPartyApiParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var convertedDataList = await pullDataService.GetJsonDataListAsync<Ticket>(param);

                return new JsonResult(convertedDataList);
            }

            return BadRequest(ModelState);
        }
    }
}
