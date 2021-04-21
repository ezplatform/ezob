using System.Collections.Generic;
using EzPlatform.EzOnboarding.Core.ConfigModel.Input;
using YamlDotNet.RepresentationModel;

namespace EzPlatform.EzOnboarding.Core.Serialization
{
    public static class SourceCodeCheckoutInputsParser
    {
        public static IConfigTaskInputs HandleInputsMapping(YamlMappingNode yamlMappingNode)
        {
            var inputs = new SourceCodeCheckoutInputs();

            foreach (var child in yamlMappingNode.Children)
            {
                var key = YamlParser.GetScalarValue(child.Key);

                switch (key)
                {
                    case "username":
                        inputs.UserName = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    case "password":
                        inputs.Password = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    case "checkoutLocation":
                        inputs.CheckoutLocation = YamlParser.GetScalarValue(key, child.Value).ToLowerInvariant();
                        break;
                    case "sources":
                        YamlParser.ThrowIfNotYamlSequence(key, child.Value);

                        HandleSourcesMapping((child.Value as YamlSequenceNode)!, inputs.Sources);
                        break;
                    default:
                        throw new EzobYamlException(child.Key.Start, string.Format(CoreStrings.UnrecognizedKey, key));
                }
            }

            return inputs;
        }

        private static void HandleSourcesMapping(YamlSequenceNode node, List<string> sources)
        {
            foreach (var child in node.Children)
            {
                var mappingNode = (YamlScalarNode)child;
                sources.Add(YamlParser.GetScalarValue(mappingNode).ToLowerInvariant());
            }
        }
    }
}
