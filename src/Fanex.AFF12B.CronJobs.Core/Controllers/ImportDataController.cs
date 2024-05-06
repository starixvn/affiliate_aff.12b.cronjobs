using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.AFF12B.CronJobs.Core.Models.DTOs;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    public class ImportDataController(
        IAppSettings appSettings,
        IImportDataService importDataService) : BaseController(appSettings)
    {
        [HttpGet("api/GetTimeTracker")]
        public async Task<ActionResult> GetTimeTrackerAsync([FromQuery] BaseParam param)
        {
            if (ModelState.IsValid)
            {
                var timeTracker = await importDataService.GetTimeTrackerAsync(param.TpType, param.SubItem);

                return Ok($"{Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} - Time Tracker: {timeTracker}");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/UpdateTimeTracker")]
        public async Task<ActionResult> UpdateTimeTrackerAsync([FromQuery] TrackerParam param)
        {
            if (ModelState.IsValid)
            {
                await importDataService.UpdateTimeTrackerAsync(param);

                return Ok($"{Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} updated time tracker successfully!");
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/ImportData")]
        public async Task<ActionResult> ImportDataToDbAsync([FromQuery] ThirdPartyApiParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var result = await importDataService.ImportDataToDbAsync<Ticket>(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/ImportConvertedTransactionIdData")]
        public async Task<ActionResult> ImportConvertedTransactionIdDataToDbAsync([FromQuery] ThirdPartyApiParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var result = await importDataService.ImportDataToDbAsync<ConvertedTransactionIdTicket>(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/ImportAdjustedDateTimeTicketData")]
        public async Task<ActionResult> ImportAdjustedDateTimeTicketDataToDbAsync([FromQuery] ThirdPartyApiParam param)
        {
            if (ModelState.IsValid)
            {
                SetMaxMinutesRange(param);
                var result = await importDataService.ImportDataToDbAsync<AdjustedDateTimeTicket>(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }
    }
}
