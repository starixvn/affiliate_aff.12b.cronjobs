namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.DTOs
{
    public class BtiToken
    {
        public string Token { get; set; }

        public int ErrorCode { get; set; }

        public bool IsValidToken => ErrorCode == 0;
    }
}
