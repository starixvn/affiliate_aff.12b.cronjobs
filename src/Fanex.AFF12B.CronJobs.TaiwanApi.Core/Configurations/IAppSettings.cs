namespace Fanex.AFF12B.CronJobs.TaiwanApi.Core.Configurations
{
    public interface IAppSettings
    {
        int MaxMinutesRange { get; }

        bool IsLocal { get; }

        bool IsRunFetchTask { get; }
    }
}
