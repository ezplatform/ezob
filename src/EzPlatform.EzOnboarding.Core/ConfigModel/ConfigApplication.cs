using System.Collections.Generic;
using System.IO;
using YamlDotNet.Serialization;

namespace EzPlatform.EzOnboarding.Core.ConfigModel
{
    public class ConfigApplication
    {
        [YamlIgnore]
        public FileInfo Source { get; set; } = default!;

        public string? Name { get; set; }

        public List<ConfigStage> Stages { get; set; } = new List<ConfigStage>();
    }
}
