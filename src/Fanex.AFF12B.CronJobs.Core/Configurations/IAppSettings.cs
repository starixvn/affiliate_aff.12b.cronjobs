using System.Collections.Generic;

namespace Fanex.AFF12B.CronJobs.Core.Configurations
{
    public interface IAppSettings
    {
        int MaxMinutesRange { get; }

        bool IsLocal { get; }

        bool IsRunFetchTask { get; }

        List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>> ProductJobs { get; }

        List<ThirdPartyCronJobSetting<AdjustedDateTimeProductSetting>> ConvertedTimeZoneProductJobs { get; }
    }
}
