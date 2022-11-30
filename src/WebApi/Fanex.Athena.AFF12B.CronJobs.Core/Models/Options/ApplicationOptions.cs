namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models.Options
{
    public class ApplicationOptions
    {
        public IList<string> WhiteList { get; set; }
        public string LogCategory { get; set; }
        public string SentryDsn { get; set; }
    }
}
