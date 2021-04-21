using System;
using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core.ConfigModel;

namespace EzPlatform.EzOnboarding.Core.Tasks
{
    public abstract class BaseTaskProcessor : ITaskProcessor
    {
        public virtual async Task<TaskExecutionResult> ProcessAsync(OutputContext context, ConfigTask configTask)
        {
            try
            {
                var isCompleted = await IsCompleted(context, configTask);

                if (isCompleted)
                {
                    // Return as completed.
                    return new TaskExecutionResult();
                }

                return await InternalProcessAsync(context, configTask);
            }
            catch (Exception exception)
            {
                var failedResult = TaskExecutionResult.Failed(exception);
                context.WriteInfoLine($"Failed running onboarding task. {failedResult.GetErrorSummary()}.");
                return failedResult;
            }
        }

        public abstract Task<TaskExecutionResult> InternalProcessAsync(OutputContext context, ConfigTask configTask);

        public abstract Task<bool> IsCompleted(OutputContext context, ConfigTask configTask);
    }
}
