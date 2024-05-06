using System;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.Core.Models.MetaModels
{
    public class TrackerParam : BaseParam
    {
        [Required]
        public DateTime CallTime { get; set; }
    }
}
