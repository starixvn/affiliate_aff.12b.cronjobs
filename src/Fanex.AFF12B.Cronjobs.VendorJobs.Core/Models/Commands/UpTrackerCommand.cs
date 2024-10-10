using Fanex.Data.Repository;
using System;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.Commands
{
    public class UpTrackerCommand : NonQueryCommand
    {
        private const string SP_NAME = "Task_APIProduct_CallTimeIns";

        public override string GetSettingKey()
            => SP_NAME;

        public int ProductId { get; set; }

        public DateTime? CallTime { get; set; }

        public override bool IsValid()
            => true;
    }
}
