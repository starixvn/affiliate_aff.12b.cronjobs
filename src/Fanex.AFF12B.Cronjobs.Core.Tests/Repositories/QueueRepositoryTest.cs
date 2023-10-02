using Fanex.AFF12B.Cronjobs.Core.Tests.Helpers;
using Fanex.AFF12B.CronJobs.Core.Models.MetaModels;
using Fanex.AFF12B.CronJobs.Core.Repositories;
using Fanex.Data;
using NSubstitute;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Repositories
{
    public class QueueRepositoryTest
    {
        public QueueRepositoryTest()
        {
            MockingHelper.SetUpDbManagerEnviroment();
        }

        [Fact]
        public async Task AddQueue_WhenCall_ExecuteCorrect()
        {
            //Arrange
            var param = new QueueParam();
            var mockObjectDB = Substitute.For<IObjectDb>();
            MockingHelper.SwitchObjectDbFactoryToTestingMode(mockObjectDB);

            var repository = new QueueRepository();

            //Action
            await repository.AddQueue(param);

            //Assert
            await mockObjectDB.Received().ExecuteNonQueryAsync(param, Arg.Any<DbTransaction>(), Arg.Any<CancellationToken>());
        }
    }
}
