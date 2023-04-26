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
using System.Text.RegularExpressions;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    public class ImportDataController : BaseController
    {
        private const string IpRegex = "((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(\\.|$)){4}";

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
                var result = await _importDataService.ImportDataToDbAsync(param);

                return new JsonResult(result);
            }

            return BadRequest(ModelState);
        }

        [HttpPost("api/RegisterCronJobs")]
        public ActionResult RegisterCronJobs()
        {
            var productJobs = AppSettings.ProductJobs;

            foreach (var productJob in productJobs)
            {
                var (jobName, productJobSetting) = GetInfoProductJob(productJob); ;

                RegisterTask<IFetchThirdPartyProductDataTask>(jobName, productJob.PullInfoCron, job => job.FetchDataAsync(jobName, productJobSetting));
            }

            return Ok($"Register Cron Jobs on {Dns.GetHostName()} - {GetIPAddress()} successfully!");
        }

        [HttpPost("api/DisposeCronJobs")]
        public ActionResult DisposeCronJobs()
        {
            var productJobs = AppSettings.ProductJobs;

            foreach (var productJob in productJobs)
            {
                var (jobName, _) = GetInfoProductJob(productJob);

                DisposeTask(jobName);
            }

            return Ok($"Dispose Cron Jobs on {Dns.GetHostName()} - {GetIPAddress()} successfully!");
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

            foreach (var ip in ipHostInfo.AddressList)
            {
                var ipAddress = ip.ToString();
                if (Regex.IsMatch(ipAddress, IpRegex))
                {
                    return ipAddress;
                }
            }

            return ipHostInfo.AddressList[0].ToString();
        }

        private (string, ThirdPartyCronJobSetting) GetInfoProductJob(FetchThirdPartyProductSetting productSetting)
        {
            var productJobSetting = productSetting.CronJobSetting;
            var jobName = Product.GetProductNameByTpTypeAndSubItem(productJobSetting.TpType, productJobSetting.SubItem);

            return (jobName, productJobSetting);
        }
    }
}
