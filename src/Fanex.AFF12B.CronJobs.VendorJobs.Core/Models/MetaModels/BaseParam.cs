using System;
using System.ComponentModel.DataAnnotations;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels
{
    public class BaseParam
    {
        [Required]
        public DateTime From { get; set; }
        [Required]
        public DateTime To { get; set; }
    }
}
