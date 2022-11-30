using Fanex.Athena.AFF12B.CronJobs.Core.Constant;
using System.ComponentModel.DataAnnotations;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class QueryApiParam : ThirdPartyApiParam
    {
        [Required]
        public string Access_Token { get; set; } = DefaultValues.String;
    }
}
