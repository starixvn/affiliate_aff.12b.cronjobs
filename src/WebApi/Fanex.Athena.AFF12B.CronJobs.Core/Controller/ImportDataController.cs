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
    public class ImportDataController
    {
        private readonly IImportDataService _importDataService;

        public ImportDataController(IImportDataService importDataService)
        {
            _importDataService = importDataService;
        }

        [HttpPost("api/ImportData")]
        public async Task<ActionResult> ImportDataAndTrackTimeToDbAsync([FromQuery] QueryApiParam param)
        {
            var convertedDataList = await _importDataService.ImportDataAndTrackTimeToDbAsync(param);

            return new JsonResult(convertedDataList);
        }
    }
}
