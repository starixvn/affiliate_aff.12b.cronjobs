using Fanex.Data;
using Fanex.Logging;
using Microsoft.Extensions.Configuration;
using NSubstitute;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Helpers
{
    public static class MockingHelper
    {
        public static IConfiguration SetUpConfiguration(string key, string value)
        {
            var inMemorySettings = new Dictionary<string, string> {
                {key, value},
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();
        }

        public static void SetUpLogEngineEnviroment()
        {
            var logEngine = Substitute.For<ILogEngine>();

            LogManager.SetDefaultLogCategory("Tests")
                .Use(new List<ILogEngine>
                {
                    logEngine
                });
        }

        public static void SetUpDbManagerEnviroment()
        {
            DbSettingProviderManager
             .StartTestingSession()
             .Run();
        }

        public static void SwitchObjectDbFactoryToTestingMode(IObjectDb mockObjectDB)
        {
            ObjectDbFactory.SwitchToTestingMode();
            ObjectDbFactory.Mock((x, y) => mockObjectDB);
        }
    }
}
