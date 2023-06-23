using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.Logging;
using Hangfire.Common;
using Hangfire.States;
using Newtonsoft.Json;
using System;
using System.Text;

namespace Fanex.AFF12B.CronJobs.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class GlobalHangfireJobExceptionFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        private readonly ILogger _logger;

        public GlobalHangfireJobExceptionFilterAttribute() : this(null)
        {
        }

        public GlobalHangfireJobExceptionFilterAttribute(ILogger logger)
        {
            _logger = logger ?? Logger.Log;
        }

        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is FailedState failedState)
            {
                var exception = failedState.Exception;
                _logger.Error(GenerateJobException(context), exception);
            }
        }

        private static string GenerateJobException(ElectStateContext context)
        {
            var jobName = context.BackgroundJob.Job.Args[0].ToString();
            var failedState = context.CandidateState as FailedState;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Job Name: {jobName} - {context.BackgroundJob.Job.Type.Name}");
            stringBuilder.AppendLine($"Job Id: {context.BackgroundJob.Id}");
            stringBuilder.AppendLine($"Arguments: {JsonConvert.SerializeObject(context.BackgroundJob.Job.Args)}");
            stringBuilder.AppendLine($"Created Time: {context.BackgroundJob.CreatedAt}");
            stringBuilder.AppendLine(ErrorMessageHelper.GenerateMessage(failedState.Exception));

            return stringBuilder.ToString();
        }
    }
}
