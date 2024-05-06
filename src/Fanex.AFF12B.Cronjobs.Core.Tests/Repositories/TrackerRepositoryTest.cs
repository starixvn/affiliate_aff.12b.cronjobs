using Fanex.AFF12B.Cronjobs.Core.Tests.Helpers;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.Data;
using NSubstitute;
using System.Data.Common;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Repositories
{
    public class TrackerRepositoryTest
    {
        public TrackerRepositoryTest()
        {
            MockingHelper.SetUpDbManagerEnviroment();
        }

        [Fact]
        public async Task AddTracker_WhenCall_ExecuteCorrect()
        {
            //Arrange
            var mockObjectDB = Substitute.For<IObjectDb>();
            MockingHelper.SwitchObjectDbFactoryToTestingMode(mockObjectDB);

            var repository = new TrackerRepository();

            //Action
            await repository.AddTracker(new TrackerParam());

            //Assert
            await mockObjectDB.Received().ExecuteNonQueryAsync(Arg.Any<TrackerParam>());
        }

        [Fact]
        public async Task GetTracker_WhenCall_AssignCorrectParrameterAndReturnCorrectResultAsync()
        {
            //Arrange
            var tpType = 1;
            var subItem = "";
            var timeTracker = new DateTime(2023, 05, 23) as DateTime?;
            var mockObjectDB = Substitute.For<IObjectDb>();
            mockObjectDB.QueryEntityAsync<DateTime?>(Arg.Any<object>(), Arg.Any<DbTransaction>(), Arg.Any<CancellationToken>()).Returns(Task.FromResult(timeTracker));
            MockingHelper.SwitchObjectDbFactoryToTestingMode(mockObjectDB);

            var repository = new TrackerRepository();

            //Action
            var result = await repository.GetTracker(1);

            //Assert
            Assert.Equal(result, timeTracker);
            mockObjectDB.Received(1).SetParameterValue(nameof(tpType), tpType);
            mockObjectDB.Received(1).SetParameterValue(nameof(subItem), subItem);
        }
    }
}
