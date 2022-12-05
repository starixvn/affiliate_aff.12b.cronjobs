using Fanex.Athena.AFF12B.CronJobs.Core.Services;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Configurations;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models.MetaModels;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Controllers
{
    public class ImportDataController : BaseController
    {
        private readonly IImportDataService _importDataService;

        public ImportDataController(
            IAppSettings appSettings,
            IImportDataService importDataService) : base(appSettings)
        {
            _importDataService = importDataService;
        }

        [HttpPost("api/ImportData")]
        public async Task<ActionResult> ImportDataAndTrackTimeToDbAsync([FromQuery] QueryParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var convertedDataList = await _importDataService.ImportDataAndTrackTimeToDbAsync(param);

                return new JsonResult(convertedDataList);
            }

            return BadRequest(ModelState);
        }
    }
}
