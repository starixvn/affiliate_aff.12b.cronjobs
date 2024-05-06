using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Core.Controllers
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public class BaseController(IAppSettings appSettings) : ControllerBase
    {
        protected void SetMaxMinutesRange(ThirdPartyApiParam param)
        {
            param.MaxMinutesRange = param.MaxMinutesRange == 0 ? appSettings.MaxMinutesRange : param.MaxMinutesRange;
        }
    }
}
