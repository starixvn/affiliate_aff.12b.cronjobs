using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.AFF12B.CronJobs.Ext.Services.Core.Configurations;
using Fanex.AFF12B.CronJobs.Ext.Services.Core.Models.MetaModels;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Controllers
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
