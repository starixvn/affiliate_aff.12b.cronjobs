using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.Athena.AFF12B.CronJobs.Core.Services;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Controller
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class PullDataController
    {
        private readonly IPullDataService _pullDataService;

        public PullDataController(IPullDataService pullDataService)
        {
            _pullDataService = pullDataService;
        }

        [HttpGet("api/GetRawData")]
        public async Task<ActionResult> GetRawDataListAsync([FromQuery]QueryApiParam param)
        {
            var rawDataList = await _pullDataService.GetRawDataListAsync(param);

            return new JsonResult(rawDataList);
        }

        [HttpGet("api/GetJsonData")]
        public async Task<ActionResult> GetJsonDataListAsync([FromQuery] QueryApiParam param)
        {
            var convertedDataList = await _pullDataService.GetJsonDataListAsync(param);

            return new JsonResult(convertedDataList);
        }
    }
}
