using Newtonsoft.Json;
using System;
using System.Xml.Linq;

namespace Fanex.AFF12B.Cronjobs.VendorApi.Core.Helpers
{
    public static class XmlHelper
    {
        public static XDocument ParseToXDocument(string xml)
        {
            try
            {
                return XDocument.Parse(xml);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static XDocument ParseToXDocumentFromJson(string json, string rootTagName = "root")
        {
            try
            {
                var rootTag = string.IsNullOrEmpty(rootTagName) ? "root" : rootTagName;
                return JsonConvert.DeserializeXNode(json, rootTag, true);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
