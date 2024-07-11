using Fanex.AFF12B.CronJobs.Core.Helpers;
using System.Xml.Linq;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Services.ResponseCheckers
{
    public class BTiResponseChecker
    {
        public static void CheckXml(XDocument xml)
        {
            var root = xml.Root;
            var code = root.Element("errorCode") ?? throw ErrorMessageHelper.FormatException($"BTi Product - Xml response is not correct format {xml}");

            const string successCode = "0";
            var value = code.Value;

            if (value != successCode)
            {
                throw ErrorMessageHelper.FormatException($"BTi Product - Xml response code indicate request not success {xml}");
            }
        }
    }
}
