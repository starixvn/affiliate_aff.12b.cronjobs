using System.ComponentModel.DataAnnotations;
using Fanex.Athena.AFF12B.CronJobs.Core.Constant;
using Fanex.Athena.AFF12B.CronJobs.Core.Models.MetaModels;

namespace Fanex.Athena.AFF12B.CronJobs.WebApi.Core.Models.MetaModels
{
    public class QueryParam : ThirdPartyApiParam
    {
        [Required]
        public string Access_Token { get; set; } = DefaultValue.String;
    }
}
