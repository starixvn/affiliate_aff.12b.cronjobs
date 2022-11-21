namespace Fanex.Athena.AFF12B.CronJobs.Core.Params
{
    public class QueueParam
    {
        public int TpType { get; set; }

        public string? SubItem { get; set; }

        public byte JobType { get; set; }

        public string XmlData { get; set; } = string.Empty;
    }
}
