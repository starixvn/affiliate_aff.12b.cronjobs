using Fanex.Data.Repository;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Models.Commands
{
    public class ImportDataCommand : NonQueryCommand
    {
        public ImportDataCommand(int productId, string xmlData)
        {
            ProductId = productId;
            XmlData = xmlData;
        }

        private const string SP_NAME = "Task_APIProduct_ImportQueue";

        public override string GetSettingKey()
            => SP_NAME;

        public int ProductId { get; set; }

        public string XmlData { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public override bool IsValid()
            => true;
    }
}
