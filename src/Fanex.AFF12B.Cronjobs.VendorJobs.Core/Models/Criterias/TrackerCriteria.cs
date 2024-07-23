using Fanex.Data.Repository;
namespace Fanex.AFF12B.CronJobs.GeneralJobs.Core.Models.MetaModels
{
    public class TrackerCriteria : CriteriaBase
    {
        private const string SP_NAME = "Task_APIProduct_CallTimeByProductSel";

        public override string GetSettingKey()
            => SP_NAME;
        public int ProductId { get; set; }
        public override bool IsValid()
            => true;
    }
}
