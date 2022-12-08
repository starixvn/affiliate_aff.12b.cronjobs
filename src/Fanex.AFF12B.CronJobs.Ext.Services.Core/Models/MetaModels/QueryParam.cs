using System.ComponentModel.DataAnnotations;
using Fanex.AFF12B.CronJobs.Core.Constant;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;

namespace Fanex.AFF12B.CronJobs.Ext.Services.Core.Models.MetaModels
{
    public class QueryParam : ThirdPartyApiParam
    {
        [Required]
        public string Access_Token { get; set; } = DefaultValue.String;
    }
}
