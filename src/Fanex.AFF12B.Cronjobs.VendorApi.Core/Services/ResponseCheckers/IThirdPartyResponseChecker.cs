using System.Xml.Linq;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Services.ResponseCheckers
{
    public interface IThirdPartyResponseChecker
    {
        void CheckXml(XDocument xml);
    }
}
