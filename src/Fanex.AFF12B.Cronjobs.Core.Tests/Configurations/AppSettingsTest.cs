using Fanex.AFF12B.Cronjobs.Core.Tests.Helpers;
using Fanex.AFF12B.CronJobs.Core.Configurations;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Configurations
{
    public class AppSettingsTest
    {
        private IConfiguration _mockConfiguration;

        public AppSettingsTest()
        {
            _mockConfiguration = Substitute.For<IConfiguration>();
        }

        [Fact]
        public void MaxMinutesRange_WhenCall_ReturnCorrect()
        {
            //Arrange
            var maxMinutesRange = 30;
            _mockConfiguration = MockingHelper.SetUpConfiguration("AppSettings:MaxMinutesRange", $"{maxMinutesRange}");

            //Action
            var result = AppSettings.MaxMinutesRange;

            //Assert
            Assert.Equal(maxMinutesRange, result);
        }

        [Fact]
        public void IsRunFetchTask_WhenCall_ReturnCorrect()
        {
            //Arrange
            var isRunFetchTask = true;
            _mockConfiguration = MockingHelper.SetUpConfiguration("AppSettings:IsRunFetchTask", $"{isRunFetchTask}");

            //Action
            var result = AppSettings.IsRunFetchTask;

            //Assert
            Assert.Equal(isRunFetchTask, result);
        }

        [Theory]
        [InlineData("LOCAL")]
        [InlineData("Development")]
        public void IsLocal_WhenCall_ReturnCorrect(string env)
        {
            //Arrange
            _mockConfiguration = MockingHelper.SetUpConfiguration("AppSettings:Environment", $"{env}");

            //Action
            var result = AppSettings.IsLocal;

            //Assert
            Assert.True(result);
        }

        [Fact]
        public void ProductJobs_WhenCall_ReturnCorrect()
        {
            //Arrange
            var productJobs = new List<ThirdPartyCronJobSetting<ThirdPartyProductSetting>> { new ThirdPartyCronJobSetting<ThirdPartyProductSetting> {
                PullInfoCron = "*/15 * * * *",
                CronJobSetting = new ThirdPartyProductSetting {
                    MinutesAdjustment = -60,
                    MaxMinutesRange = 30,
                    MillisecondsDeplay = 0,
                    TpType = 7,
                    SubItem = "001001"
                }
            } };

            _mockConfiguration = MockingHelper.SetUpConfiguration("AppSettings:ThirdPartyProductsSetting", $"{JsonConvert.SerializeObject(productJobs)}");

            //Action
            var result = AppSettings.ProductJobs;

            //Assert
            Assert.Empty(result);
        }

        private AppSettings AppSettings => new AppSettings(_mockConfiguration);
    }
}
