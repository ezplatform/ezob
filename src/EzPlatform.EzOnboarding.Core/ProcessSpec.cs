using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EzPlatform.EzOnboarding.Core
{
    public class ProcessSpec
    {
        public string? Executable { get; set; }

        public string? WorkingDirectory { get; set; }

        public IDictionary<string, string> EnvironmentVariables { get; set; } = new Dictionary<string, string>();

        public string? Arguments { get; set; }

        public Action<string>? OutputData { get; set; }

        public Func<Task<int>>? Build { get; set; }

        public Action<string>? ErrorData { get; set; }

        public Action<int>? OnStart { get; set; }

        public Action<int>? OnStop { get; set; }

        public string? ShortDisplayName() => Path.GetFileNameWithoutExtension(Executable);
    }
}
