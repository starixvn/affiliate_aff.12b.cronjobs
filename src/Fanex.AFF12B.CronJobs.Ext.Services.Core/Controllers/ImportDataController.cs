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
        public async Task<ActionResult> ImportDataToDbAsync([FromQuery] QueryParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var result = await _importDataService.ImportDataToDbAsync(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/UpdateTimeTracker")]
        public async Task<ActionResult> UpdateTimeTrackerAsync([FromQuery] TrackerConfigParam param)
        {
            if (ModelState.IsValid)
            {
                await _importDataService.UpdateTimeTrackerAsync(param);

                return Ok("Update time tracker successfully!");
            }

            return BadRequest(ModelState);
        }
    }
}
