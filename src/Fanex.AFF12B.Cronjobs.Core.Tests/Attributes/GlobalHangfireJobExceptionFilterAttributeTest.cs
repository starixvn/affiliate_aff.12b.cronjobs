using Fanex.AFF12B.Cronjobs.Core.Tests.Helpers;
using Fanex.AFF12B.CronJobs.Core.Attributes;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.Configurations;
using Fanex.AFF12B.CronJobs.GeneralJobs.Core.RecuringTasks;
using Fanex.Logging;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Hangfire.Storage;
using NSubstitute;
using System;
using Xunit;

namespace Fanex.AFF12B.Cronjobs.Core.Tests.Attributes
{
    public class GlobalHangfireJobExceptionFilterAttributeTest
    {
        private readonly ILogger _logger;

        public GlobalHangfireJobExceptionFilterAttributeTest()
        {
            _logger = Substitute.For<ILogger>();
        }

        [Fact]
        public void OnStateElection_WhenCallWithEmptyLogger_IsNotFailedState_ShouldNotCallLog()
        {
            //Arrange
            MockingHelper.SetUpLogEngineEnviroment();
            var state = new ScheduledState(TimeSpan.Zero);
            var electStateContext = GetElectStateContext(state);

            //Action
            var filter = new GlobalHangfireJobExceptionFilterAttribute();
            filter.OnStateElection(electStateContext);

            //Assert
            _logger.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Fact]
        public void OnStateElection_WhenCallWith_IsNotFailedState_ShouldNotCallLog()
        {
            //Arrange
            var state = new ScheduledState(TimeSpan.Zero);
            var electStateContext = GetElectStateContext(state);

            //Action
            var filter = new GlobalHangfireJobExceptionFilterAttribute(_logger);
            filter.OnStateElection(electStateContext);

            //Assert
            _logger.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<Exception>());
        }

        [Fact]
        public void OnStateElection_WhenCallWith_IsFailedState_ShouldCallLog()
        {
            //Arrange
            var state = new FailedState(new Exception("Throw exception"));
            var electStateContext = GetElectStateContext(state);

            //Action
            var filter = new GlobalHangfireJobExceptionFilterAttribute(_logger);
            filter.OnStateElection(electStateContext);

            //Assert
            _logger.Received().Error(Arg.Any<string>(), Arg.Any<Exception>());
        }

        private static ElectStateContext GetElectStateContext(IState state)
        {
            var jobStorage = Substitute.For<JobStorage>();
            var backgroundJob = new BackgroundJob("job-id", Job.FromExpression<IFetchThirdPartyProductDataTask>(job => job.FetchDataAsync("Job Test", new ThirdPartyProductSetting())), DateTime.UtcNow);
            var storageConnection = Substitute.For<IStorageConnection>();
            var writeOnlyTransaction = Substitute.For<IWriteOnlyTransaction>();
            var applyStateContext = new ApplyStateContext(jobStorage, storageConnection, writeOnlyTransaction, backgroundJob, state, string.Empty);

            return new ElectStateContext(applyStateContext);
        }
    }
}
