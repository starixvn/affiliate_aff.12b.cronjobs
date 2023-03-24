using Fanex.AFF12B.CronJobs.Core.Contants;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Services;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;
using Hangfire;
using Fanex.AFF12B.CronJobs.Core.RecuringTasks;
using System.Net;

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

                return new JsonResult($"{Product.GetProductNameByTpTypeAndSubItem(param.TpType, param.SubItem)} - Time Tracker: {timeTracker}");
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
                var result = await _importDataService.ImportDataToDbAsync(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/RegisterJobs")]
        public ActionResult RegisterJobs([FromQuery] string access_token)
        {
            if (string.IsNullOrEmpty(access_token))
            {
                return BadRequest("Your don't have permisison");
            }

            var productJobs = AppSettings.ProductJobs;

            foreach (var productJob in productJobs)
            {
                var (jobName, productJobSetting) = GetInfoProductJob(productJob); ;

                RegisterTask<IFetchThirdPartyProductDataTask>(jobName, productJob.PullInfoCron, job => job.FetchDataAsync(jobName, productJobSetting));
            }

            return Ok($"Register jobs on {Dns.GetHostName()} - {GetIPAddress()} successfully!");
        }

        [HttpPost("api/DisposeJobs")]
        public ActionResult DisposeJobs([FromQuery] string access_token)
        {
            if (string.IsNullOrEmpty(access_token))
            {
                return BadRequest("Your don't have permisison");
            }

            var productJobs = AppSettings.ProductJobs;

            foreach (var productJob in productJobs)
            {
                var (jobName, _) = GetInfoProductJob(productJob);

                DisposeTask(jobName);
            }

            return Ok($"Dispose all Jobs on {Dns.GetHostName()} - {GetIPAddress()} successfully!");
        }

        private static void RegisterTask<T>(string serviceId, string cronExpression, Expression<Func<T, Task>> methodCall)
        {
            if (!string.IsNullOrWhiteSpace(cronExpression))
            {
                RecurringJob.AddOrUpdate<T>(
                            serviceId,
                            methodCall,
                            cronExpression);
            }
        }

        private static void DisposeTask(string serviceId)
        {
            if (!string.IsNullOrWhiteSpace(serviceId))
            {
                RecurringJob.RemoveIfExists(serviceId);
            }
        }

        private string GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddress = ipHostInfo.AddressList[0];

            return ipAddress.ToString();
        }

        private (string, ThirdPartyCronJobSetting) GetInfoProductJob(FetchThirdPartyProductSetting productSetting)
        {
            var productJobSetting = productSetting.CronJobSetting;
            var jobName = Product.GetProductNameByTpTypeAndSubItem(productJobSetting.TpType, productJobSetting.SubItem);

            return (jobName, productJobSetting);
        }
    }
}
