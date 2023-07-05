using Fanex.AFF12B.CronJobs.Core.Constants;
using Fanex.Data;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class TrackerConfigParam : TrackerParam
    {
        [Required]
        [SpParam(Ignored = true)]
        public string Access_Token { get; set; } = DefaultValue.String;
    }
}
