using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core.ConfigModel;

namespace EzPlatform.EzOnboarding.Core.Tasks
{
    /// <summary>
    /// To manage the onboarding process for a new member.
    /// </summary>
    public interface ITaskManager
    {
        Task<TaskExecutionResult> RunTaskAsync(OutputContext context, ConfigTask task);
    }
}
