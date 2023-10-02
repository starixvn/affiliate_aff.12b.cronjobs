using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
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

        [HttpGet("api/GetTimeTracker")]
        public async Task<ActionResult> GetTimeTrackerAsync([FromQuery] TimeTrackerParam param)
        {
            if (ModelState.IsValid)
            {
                var timeTracker = await _importDataService.GetTimeTrackerAsync(param.TpType, param.SubItem);

                return Ok($"{Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} - Time Tracker: {timeTracker}");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/UpdateTimeTracker")]
        public async Task<ActionResult> UpdateTimeTrackerAsync([FromQuery] TrackerConfigParam param)
        {
            if (ModelState.IsValid)
            {
                await _importDataService.UpdateTimeTrackerAsync(param);

                return Ok($"{Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} updated time tracker successfully!");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/ImportData")]
        public async Task<ActionResult> ImportDataToDbAsync([FromQuery] QueryParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var result = await _importDataService.ImportDataToDbAsync<Ticket>(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/ImportConvertedTransactionIdData")]
        public async Task<ActionResult> ImportConvertedTransactionIdDataToDbAsync([FromQuery] QueryParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var result = await _importDataService.ImportDataToDbAsync<ConvertedTransactionIdTicket>(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }
    }
}
