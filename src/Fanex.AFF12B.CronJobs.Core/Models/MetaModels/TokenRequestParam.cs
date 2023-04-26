using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    [JsonObject("tokenRequest")]
    public class TokenRequestParam
    {
        const int MinLength = 6;
        const int MaxLengthOfUserName = 50;
        const int MaxLengthOfPassword = 16;

        [MinLength(MinLength)]
        [MaxLength(MaxLengthOfUserName)]
        [Required]
        [JsonProperty("username")]
        public string Username { get; set; }

        [MinLength(MinLength)]
        [MaxLength(MaxLengthOfPassword)]
        [Required]
        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("isShowIpvn")]
        public bool IsShowIpvn { get; set; } = true;
    }
}
