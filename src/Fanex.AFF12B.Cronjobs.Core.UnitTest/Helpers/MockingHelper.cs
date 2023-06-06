using Fanex.Data;
using Fanex.Logging;
using NSubstitute;
using System.Collections.Generic;

namespace Fanex.AFF12B.Cronjobs.Core.UnitTest.Helpers
{
    public static class MockingHelper
    {
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
