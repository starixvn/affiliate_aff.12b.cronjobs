using Fanex.AFF12B.CronJobs.Core.Helpers;
using Fanex.AFF12B.CronJobs.Ext.Jobs.Core.Configurations;
using Fanex.Logging;
using Hangfire.Common;
using Hangfire.States;
using Newtonsoft.Json;
using System.Text;

namespace Fanex.AFF12B.CronJobs.Ext.Jobs.Core.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Interface)]
    public class GlobalHangfireJobExceptionFilterAttribute : JobFilterAttribute, IElectStateFilter
    {
        public GlobalHangfireJobExceptionFilterAttribute()
        {
        }

        public void OnStateElection(ElectStateContext context)
        {
            if (context.CandidateState is FailedState failedState)
            {
                var exception = failedState.Exception;
                Logger.Log.Error(GenerateJobException(context), exception);
            }
        }

        private static string GenerateJobException(ElectStateContext context)
        {
            var jobSetting = new ThirdPartyCronJobSetting();
            try
            {
                jobSetting = context.BackgroundJob.Job.Args[0] as ThirdPartyCronJobSetting;
            } catch { };

            var failedState = context.CandidateState as FailedState;
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"Job Name: {jobSetting.CronJobName} - {context.BackgroundJob.Job.Type.Name}");
            stringBuilder.AppendLine($"Job Id: {context.BackgroundJob.Id}");
            stringBuilder.AppendLine($"Arguments: {JsonConvert.SerializeObject(context.BackgroundJob.Job.Args)}");
            stringBuilder.AppendLine($"Created Time: {context.BackgroundJob.CreatedAt}");
            stringBuilder.AppendLine(ErrorMessageHelper.GenerateMessage(failedState.Exception));

            return stringBuilder.ToString();
        }
    }
}
