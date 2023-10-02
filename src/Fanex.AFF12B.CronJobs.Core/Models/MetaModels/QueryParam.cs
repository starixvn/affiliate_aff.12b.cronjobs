using Fanex.AFF12B.CronJobs.Core.Constants;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class QueryParam : ThirdPartyApiParam
    {
        [Required]
        public string Access_Token { get; set; } = DefaultValue.String;
    }
}
