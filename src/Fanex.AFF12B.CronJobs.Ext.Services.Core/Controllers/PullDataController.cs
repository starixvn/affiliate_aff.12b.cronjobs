using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.AFF12B.CronJobs.Ext.Services.Core.Configurations;
using Fanex.AFF12B.CronJobs.Ext.Services.Core.Models.MetaModels;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Controllers
{
    public class PullDataController : BaseController
    {
        private readonly IPullDataService _pullDataService;

        public PullDataController(
            IAppSettings appSettings, 
            IPullDataService pullDataService) : base(appSettings)
        {
            _pullDataService = pullDataService;
        }

        [HttpGet("api/GetRawData")]
        public async Task<ActionResult> GetRawDataListAsync([FromQuery]QueryParam param)
        {
            if(ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var rawDataList = await _pullDataService.GetRawDataListAsync(param);

                return new JsonResult(rawDataList);
            }

            return BadRequest(ModelState);
        }

        [HttpGet("api/GetJsonData")]
        public async Task<ActionResult> GetJsonDataListAsync([FromQuery] QueryParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var convertedDataList = await _pullDataService.GetJsonDataListAsync(param);

                return new JsonResult(convertedDataList);
            }

            return BadRequest(ModelState);
        }
    }
}
