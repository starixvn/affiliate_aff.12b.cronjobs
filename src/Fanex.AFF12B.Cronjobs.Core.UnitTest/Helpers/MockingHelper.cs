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
    }
}
