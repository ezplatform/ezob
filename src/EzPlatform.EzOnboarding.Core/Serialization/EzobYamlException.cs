using System;
using YamlDotNet.Core;

namespace EzPlatform.EzOnboarding.Core.Serialization
{
    public class EzobYamlException : Exception
    {
        public EzobYamlException(string message)
            : base(message)
        {
        }

        public EzobYamlException(Mark start, string message)
            : this(start, message, null)
        {
        }

        public EzobYamlException(Mark start, string message, Exception? innerException)
            : base($"Error parsing ezob.yaml: ({start.Line}, {start.Column}): {message}", innerException)
        {
        }

        public EzobYamlException(string message, Exception? inner)
            : base(message, inner)
        {
        }
    }
}
