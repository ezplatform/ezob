using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace EzPlatform.EzOnboarding.Core
{
    public sealed class TaskStatusTracker
    {
        private const string StatusFileName = "result.json";

        private static TaskStatusTracker? _instance;

        private readonly List<TaskResult> _taskResults;

        private TaskStatusTracker()
        {
            _taskResults = LoadFromStatusFile();
        }

        public static TaskStatusTracker Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TaskStatusTracker();
                }

                return _instance;
            }
        }

        public IReadOnlyList<TaskResult> TaskResults
        {
            get
            {
                return _taskResults;
            }
        }

        public void Update(string id, bool isCompleted)
        {
            var taskResult = _taskResults.FirstOrDefault(x => x.Id == id);

            // Create new taskResult if there is no existing one.
            if (taskResult == null)
            {
                taskResult = new TaskResult
                {
                    Id = id
                };
                _taskResults.Add(taskResult);
            }

            taskResult.IsCompleted = isCompleted;
            WriteJsonToFile();
        }

        public bool IsTaskCompleted(string id)
        {
            var taskResult = _taskResults.FirstOrDefault(x => x.Id == id);
            return taskResult?.IsCompleted ?? false;
        }

        private void WriteJsonToFile()
        {
            var jsonData = JsonSerializer.Serialize(_taskResults);
            System.IO.File.WriteAllText(StatusFileName, jsonData);
        }

        private List<TaskResult> LoadFromStatusFile()
        {
            CreateIfNotExist();
            var fileContent = File.ReadAllText(StatusFileName);

            // Assign [] for empty file.
            fileContent = string.IsNullOrEmpty(fileContent) ? "[]" : fileContent;
            return JsonSerializer.Deserialize<List<TaskResult>>(fileContent)!;
        }

        private void CreateIfNotExist()
        {
            // Create if file is not existed.
            if (!File.Exists(StatusFileName))
            {
                File.Create(StatusFileName).Dispose();
            }
        }
    }

    public class TaskResult
    {
        public string Id { get; set; } = default!;

        public bool IsCompleted { get; set; }
    }
}
