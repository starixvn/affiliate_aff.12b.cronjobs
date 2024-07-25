using System.Collections.Generic;

namespace Fanex.AFF12B.CronJobs.Core.Constants
{
    public static class Product
    {
        private static Dictionary<string, string> ProductByGeneralJobs => new Dictionary<string, string>
        {
            //{ tpType + subItem, productName }
            { "1", "12Emerald - Playtech" },
            { "4", "12Pearl - AG" },
            { "9", "12Opal - eBet" },
            { "10", "12Amber - Betsoft" },
            { "7001002", "12Crystal - GPI Live" },
            { "7001003", "12Crystal - GPI Slot" },
            { "12", "Exchange" },
            { "7001001", "GPI Lottery & Arcade" },
            { "16", "VR Lottery" },
            { "18", "12Topaz" },
            { "19", "KY Gaming" },
            { "15", "BTi" },
            { "14", "IM Esports" },
            { "17", "12Sapphire - Skywind" },
            { "22", "12Jasper - N2 Live" },
            { "21", "12Sunstone - EA" },
            { "24", "12Zircon - BG" },
            { "25", "12Garnet - AVP Mini" },
            { "26", "12Garnet - AVP Slot" },
            { "27", "12Diamond" },
        };

        public static Dictionary<string, string> ProductOnDb => new Dictionary<string, string>
        {
            { "9", "12Emerald - Playtech" },
            { "11", "12Amber - Betsoft" },
            { "12", "12Crystal - GPI" },
            { "23", "12Topaz" },
            { "25", "BTi" },
        };

        public static string GetProductNameByTpTypeAndSubItem(byte tpType, string subItem)
        {
            var productName = "Unknown";
            var key = tpType + subItem;

            if (ProductByGeneralJobs.ContainsKey(key))
            {
                ProductByGeneralJobs.TryGetValue(key, out productName);
            }

            return $"{productName} Job(TpType: '{tpType}', SubItem: '{subItem}')";
        }

        public static string GetProductNameById(string productId)
        {
            var productName = "Unknown";
            if (ProductOnDb.ContainsKey(productId))
            {
                ProductOnDb.TryGetValue(productId, out productName);
            }

            return $"{productName} Job(ProductId: '{productId}')";
        }
    }
}
