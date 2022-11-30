using Fanex.Athena.AFF12B.CronJobs.Core.Constant;

namespace Fanex.Athena.AFF12B.CronJobs.Core.Contants
{
    public static class Products
    {
        //TpTypes
        public const int PlaytechTpType = 1;

        public const int AsiaGameTpType = 4;

        public const int GpiLotteryTpType = 7;

        public const int Gpi12CrystalLiveTpType = 7;

        public const int Gpi12CrystalSlotTpType = 7;

        public const int EbetTpType = 9;

        public const int BetsoftTpType = 10;

        //SubItems
        public const string GpiLotterySubItem = "001001";

        public const string Gpi12CrystalLiveSubItem = "001002";

        public const string Gpi12CrystalSlotSubItem = "001003";


        public static IDictionary<string, Tuple<byte, string>> Infos => new Dictionary<string, Tuple<byte, string>>
        {
            {"Playtech", new Tuple<byte, string>(PlaytechTpType, DefaultValues.String) },
            {"AsiaGame", new Tuple<byte, string>(AsiaGameTpType, DefaultValues.String) },
            {"GpiLottery", new Tuple<byte, string>(GpiLotteryTpType, GpiLotterySubItem) },
            {"Gpi12CrystalLive", new Tuple<byte, string>(Gpi12CrystalLiveTpType, Gpi12CrystalLiveSubItem) },
            {"Gpi12CrystalSlot", new Tuple<byte, string>(Gpi12CrystalSlotTpType, Gpi12CrystalSlotSubItem) },
            {"AsiaGame", new Tuple<byte, string>(AsiaGameTpType, DefaultValues.String) },
            {"Ebet", new Tuple<byte, string>(EbetTpType, DefaultValues.String) },
            {"Betsoft", new Tuple<byte, string>(BetsoftTpType, DefaultValues.String) }
        };
    }
}
