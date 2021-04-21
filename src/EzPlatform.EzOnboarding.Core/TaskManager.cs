using System;
using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using EzPlatform.EzOnboarding.Core.Tasks;

namespace EzPlatform.EzOnboarding.Core
{
    public class TaskManager : ITaskManager
    {
        private readonly TaskRunnerMap _taskRunnerMap;

        public TaskManager()
        {
            _taskRunnerMap = new TaskRunnerMap();
        }

        public Task<TaskExecutionResult> RunTaskAsync(OutputContext context, ConfigTask task)
        {
            var taskRunner = GetTaskInstanceByType(task);
            return taskRunner.ProcessAsync(context, task);
        }

        public Task<bool> IsTaskCompleted(OutputContext context, ConfigTask task)
        {
            var taskRunner = GetTaskInstanceByType(task);
            return taskRunner.IsCompleted(context, task);
        }

        private ITaskProcessor GetTaskInstanceByType(ConfigTask task)
        {
            var taskRunnerType = _taskRunnerMap.Map[task.Task];
            return (Activator.CreateInstance(taskRunnerType) as ITaskProcessor)!;
        }
    }
}
