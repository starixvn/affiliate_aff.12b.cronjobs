using System.Net;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Helpers
{
    public static class ServerHelper
    {
        private static readonly string SERVER_NAME = Dns.GetHostName();
        public static string ServerName => SERVER_NAME;
    }
}
