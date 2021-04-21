using EzPlatform.EzOnboarding.Core.ConfigModel.Input;
using YamlDotNet.RepresentationModel;

namespace EzPlatform.EzOnboarding.Core.Serialization
{
    public static class InstallSoftwareInputsParser
    {
        public static IConfigTaskInputs HandleInputsMapping(YamlMappingNode yamlMappingNode)
        {
            var inputs = new InstallSoftwareInputs();

            foreach (var child in yamlMappingNode.Children)
            {
                var key = YamlParser.GetScalarValue(child.Key);

                switch (key)
                {
                    case "autoInstallation":
                        inputs.AutoInstallation = bool.Parse(YamlParser.GetScalarValue(key, child.Value));
                        break;
                    case "assetAddress":
                        inputs.AssetAddress = YamlParser.GetScalarValue(key, child.Value);
                        break;
                    case "downloadLocation":
                        var downloadLocation = YamlParser.GetScalarValue(key, child.Value);
                        if (!string.IsNullOrEmpty(downloadLocation))
                        {
                            inputs.DownloadLocation = downloadLocation;
                        }

                        break;
                    case "downloadFilename":
                        inputs.DownloadFilename = YamlParser.GetScalarValue(key, child.Value);
                        break;
                    case "command":
                        YamlParser.ThrowIfNotYamlMapping(child.Value);

                        HandleCommandMethodMapping((child.Value as YamlMappingNode)!, inputs.Command);
                        break;
                    case "installLocation":
                        inputs.InstallLocation = YamlParser.GetScalarValue(key, child.Value);
                        break;
                    case "validation":
                        YamlParser.ThrowIfNotYamlMapping(child.Value);

                        HandleCommandMethodMapping((child.Value as YamlMappingNode)!, inputs.Validation);
                        break;
                    default:
                        throw new EzobYamlException(child.Key.Start, string.Format(CoreStrings.UnrecognizedKey, key));
                }
            }

            return inputs;
        }

        private static void HandleCommandMethodMapping(YamlMappingNode yamlSequenceNode, CommandMethod commandMethod)
        {
            foreach (var child in yamlSequenceNode.Children)
            {
                var key = YamlParser.GetScalarValue(child.Key);
                switch (key)
                {
                    case "pwsh":
                        commandMethod.Pwsh = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    case "cmd":
                        commandMethod.Cmd = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    default:
                        throw new EzobYamlException(child.Key.Start, string.Format(CoreStrings.UnrecognizedKey, key));
                }
            }
        }
    }
}
