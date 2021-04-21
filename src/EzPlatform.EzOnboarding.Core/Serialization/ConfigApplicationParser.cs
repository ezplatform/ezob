using EzPlatform.EzOnboarding.Core.ConfigModel;
using YamlDotNet.RepresentationModel;

namespace EzPlatform.EzOnboarding.Core.Serialization
{
    public static class ConfigApplicationParser
    {
        public static void HandleConfigApplication(YamlMappingNode yamlMappingNode, ConfigApplication app)
        {
            foreach (var child in yamlMappingNode.Children)
            {
                var key = YamlParser.GetScalarValue(child.Key);

                switch (key)
                {
                    case "name":
                        app.Name = YamlParser.GetScalarValue(key, child.Value);
                        break;
                    case "stages":
                        YamlParser.ThrowIfNotYamlSequence(key, child.Value);
                        ConfigStageParser.HandleStageMapping((child.Value as YamlSequenceNode)!, app.Stages, app);
                        break;
                    default:
                        throw new EzobYamlException(child.Key.Start, string.Format(CoreStrings.UnrecognizedKey, key));
                }
            }
        }
    }
}
