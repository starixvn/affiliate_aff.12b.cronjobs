using Fanex.AFF12B.CronJobs.Ext.Services.Core.Attributes;
using Fanex.AFF12B.CronJobs.Ext.Services.Core.Configurations;
using Fanex.AFF12B.CronJobs.Ext.Services.Core.Models.MetaModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Controllers
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
