using System;

namespace EzPlatform.EzOnboarding.Core
{
    public class CommandException : Exception
    {
        public CommandException(string message)
            : base(message)
        {
        }

        public CommandException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
