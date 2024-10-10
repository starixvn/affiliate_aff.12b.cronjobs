using Refit;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.MetaModels
{
    public class BetsoftParam
    {
        [AliasAs("startDate")]
        public string StartDate { get; set; }
        [AliasAs("endDate")]
        public string EndDate { get; set; }
        [AliasAs("bankId")]
        public string BankId { get; set; }
        [AliasAs("hash")]
        public string Hash { get; set; }
    }
}
