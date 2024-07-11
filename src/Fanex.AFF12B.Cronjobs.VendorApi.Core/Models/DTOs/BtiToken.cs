namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Models.DTOs
{
    public class BtiToken
    {
        public string Token { get; set; }

        public int ErrorCode { get; set; }

        public bool IsValidToken => ErrorCode == 0;
    }
}
