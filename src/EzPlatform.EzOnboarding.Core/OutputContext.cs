using System;
using Spectre.Console;

namespace EzPlatform.EzOnboarding.Core
{
    public sealed class OutputContext
    {
        public OutputContext(Verbosity verbosity)
        {
            Verbosity = verbosity;
        }

        public Verbosity Verbosity { get; }

        public IAnsiConsole Console { get; } = AnsiConsole.Console;

        public void MarkupAlways(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.Markup);
        }

        public void WriteAlways(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.Write);
        }

        public void WriteAlwaysLine(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.WriteLine);
        }

        public void MarkupAlwaysLine(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.MarkupLine);
        }

        public void WriteInfo(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.Write);
        }

        public void MarkupInfo(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.Markup);
        }

        public void MarkupInfoLine(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.MarkupLine);
        }

        public void WriteInfoLine(string message)
        {
            PerformWrite(Verbosity.Info, message, Console.WriteLine);
        }

        public void WriteDebug(string message)
        {
            PerformWrite(Verbosity.Debug, message, Console.Write);
        }

        public void WriteDebugLine(string message)
        {
            PerformWrite(Verbosity.Debug, message, Console.WriteLine);
        }

        public void WriteCommandLine(string process, string args)
        {
            PerformWrite(Verbosity.Debug, $"> {process} {args}", Console.WriteLine);
        }

        private void PerformWrite(Verbosity verbosity, string message, Action<string> doWriteAction)
        {
            if (Verbosity >= verbosity)
            {
                doWriteAction(message);
            }
        }
    }
}
