using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class BaseController : ControllerBase
    {
        private readonly IAppSettings _appSettings;

        public BaseController(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        protected void SetMaxMinutesRange(ThirdPartyApiParam param)
        {
            param.MaxMinutesRange = param.MaxMinutesRange == 0 ? _appSettings.MaxMinutesRange : param.MaxMinutesRange;
        }
    }
}
