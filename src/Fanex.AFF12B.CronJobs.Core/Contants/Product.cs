using Fanex.AFF12B.CronJobs.Core.Constant;

namespace Fanex.AFF12B.CronJobs.Core.Contants
{
    public static class Product
    {
        private static IDictionary<string, string> ProductNames => new Dictionary<string, string>
        {
            //{ tpType + subItem, productName }
            { "1", "12Emerald - Playtech" },
            { "4", "12Pearl - AG" },
            { "9", "12Opal - eBet" },
            { "10", "12Emerald - Playtech" },
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

        public static string GetProductNameByTpTypeAndSubItem(byte tpType, string subItem)
        {
            var productName = "Unknown";
            var key = tpType + subItem;

            if (ProductNames.ContainsKey(key))
            {
                ProductNames.TryGetValue(key, out productName);
            }    

            return productName;
        }
    }
}
