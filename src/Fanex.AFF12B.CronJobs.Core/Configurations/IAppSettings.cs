
using System.Collections.Generic;

namespace Fanex.AFF12B.CronJobs.Core.Configurations
{
    public interface IAppSettings
    {
        int MaxMinutesRange { get; }

        bool IsRunFetchTask { get; }

        List<FetchThirdPartyProductSetting> ProductJobs { get; }
    }
}
