using System;
using EzPlatform.EzOnboarding.Core.ConfigModel.Input;
using YamlDotNet.RepresentationModel;

namespace EzPlatform.EzOnboarding.Core.Serialization
{
    public static class CreateTicketInputsParser
    {
        public static IConfigTaskInputs HandleInputsMapping(YamlMappingNode yamlMappingNode)
        {
            var inputs = new CreateTicketInputs();

            foreach (var child in yamlMappingNode.Children)
            {
                var key = YamlParser.GetScalarValue(child.Key);

                switch (key)
                {
                    case "url":
                        inputs.Url = new Uri(YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant());
                        break;
                    case "content":
                        inputs.Content = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    default:
                        throw new EzobYamlException(child.Key.Start, string.Format(CoreStrings.UnrecognizedKey, key));
                }
            }

            return inputs;
        }
    }
}
