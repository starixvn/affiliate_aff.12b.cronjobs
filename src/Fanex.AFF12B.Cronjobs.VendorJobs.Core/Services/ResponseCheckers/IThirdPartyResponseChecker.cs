using System.Xml.Linq;

namespace Fanex.AFF12B.CronJobs.VendorJobs.Core.Services.ResponseCheckers
{
    public interface IThirdPartyResponseChecker
    {
        void CheckXml(XDocument xml);
    }
}
