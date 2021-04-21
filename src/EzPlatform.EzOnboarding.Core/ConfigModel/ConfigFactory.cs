using System.IO;
using EzPlatform.EzOnboarding.Core.Serialization;

namespace EzPlatform.EzOnboarding.Core.ConfigModel
{
    public static class ConfigFactory
    {
        public static ConfigApplication FromFile(FileInfo file)
        {
            var extension = file.Extension.ToLowerInvariant();
            switch (extension)
            {
                case ".yaml":
                case ".yml":
                    return FromYaml(file);

                default:
                    throw new CommandException($"File '{file.FullName}' is not a supported format.");
            }
        }

        private static ConfigApplication FromYaml(FileInfo file)
        {
            using var parser = new YamlParser(file);
            return parser.ParseConfigApplication();
        }
    }
}
