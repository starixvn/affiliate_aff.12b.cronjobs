using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Attributes;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Configurations;
using Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models.MetaModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Controllers
{
    [Authorize]
    [ApiController]
    [ServiceFilter(typeof(IPFilterAttribute))]
    public abstract class BaseController : ControllerBase
    {
        private readonly IAppSettings _appSettings;

        public BaseController(IAppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        protected void SetMaxMinutesRange(QueryParam param)
        {
            param.MaxMinutesRange = param.MaxMinutesRange == 0 ? _appSettings.MaxMinutesRange : param.MaxMinutesRange;
        }
    }
}
