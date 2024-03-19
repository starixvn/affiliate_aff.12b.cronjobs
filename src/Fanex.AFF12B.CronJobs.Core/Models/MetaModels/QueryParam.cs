using Fanex.AFF12B.CronJobs.Core.Constants;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class QueryParam : ThirdPartyApiParam
    {
        [Required]
        [JsonIgnore]
        public string Access_Token { get; set; } = DefaultValue.String;
    }
}
