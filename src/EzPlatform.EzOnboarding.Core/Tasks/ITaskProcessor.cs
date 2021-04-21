using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core.ConfigModel;

namespace EzPlatform.EzOnboarding.Core.Tasks
{
    /// <summary>
    /// To represent an onboarding task.
    /// </summary>
    public interface ITaskProcessor
    {
        Task<TaskExecutionResult> ProcessAsync(OutputContext context, ConfigTask configTask);

        Task<bool> IsCompleted(OutputContext context, ConfigTask configTask);
    }
}
