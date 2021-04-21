using System;
using System.Collections.Generic;
using System.IO;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using YamlDotNet.Core;
using YamlDotNet.RepresentationModel;

namespace EzPlatform.EzOnboarding.Core.Serialization
{
    public class YamlParser : IDisposable
    {
        private readonly YamlStream _yamlStream;
        private readonly FileInfo? _fileInfo;
        private readonly TextReader _reader;

        public YamlParser(string yamlContent, FileInfo? fileInfo = null)
            : this(new StringReader(yamlContent), fileInfo)
        {
        }

        public YamlParser(FileInfo fileInfo)
            : this(fileInfo.OpenText(), fileInfo)
        {
        }

        internal YamlParser(TextReader reader, FileInfo? fileInfo = null)
        {
            _reader = reader;
            _yamlStream = new YamlStream();
            _fileInfo = fileInfo;
        }

        public static string GetScalarValue(YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Scalar)
            {
                throw new EzobYamlException(node.Start, string.Format(CoreStrings.UnexpectedType, YamlNodeType.Scalar.ToString(), node.NodeType.ToString()));
            }

            return ((YamlScalarNode)node).Value!;
        }

        public static string GetScalarValue(string key, YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Scalar)
            {
                throw new EzobYamlException(node.Start, string.Format(CoreStrings.ExpectedYamlScalar, key));
            }

            return ((YamlScalarNode)node).Value!;
        }

        public static void ThrowIfNotYamlSequence(string key, YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Sequence)
            {
                throw new EzobYamlException(node.Start, string.Format(CoreStrings.ExpectedYamlScalar, key));
            }
        }

        public static void ThrowIfNotYamlMapping(YamlNode node)
        {
            if (node.NodeType != YamlNodeType.Mapping)
            {
                throw new EzobYamlException(node.Start, string.Format(CoreStrings.UnexpectedType, YamlNodeType.Mapping.ToString(), node.NodeType.ToString()));
            }
        }

        public ConfigApplication ParseConfigApplication()
        {
            try
            {
                _yamlStream.Load(_reader);
            }
            catch (YamlException ex)
            {
                throw new EzobYamlException(ex.Start, "Unable to parse ezob.yaml. See inner exception.", ex);
            }

            var app = new ConfigApplication();

            // TODO assuming first document.
            var document = _yamlStream.Documents[0];
            var node = document.RootNode;
            ThrowIfNotYamlMapping(node);

            app.Source = _fileInfo!;

            ConfigApplicationParser.HandleConfigApplication((YamlMappingNode)node, app);

            app.Name ??= CoreStrings.WelcomeOnboard;

            // TODO confirm if these are ever null.
            foreach (var stage in app.Stages)
            {
                stage.Tasks ??= new List<ConfigTask>();
            }

            return app;
        }

        public void Dispose()
        {
            _reader.Dispose();
        }
    }
}
