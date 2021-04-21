using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Help;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.CommandLine.Parsing;
using System.Reflection;
using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core;
using EzPlatform.EzOnboarding.Core.Serialization;

namespace EzPlatform.EzOnboarding
{
    /// <summary>
    /// Main class to run the ezob command line app.
    /// </summary>
    public static partial class Program
    {
        public static async Task Main(string[] args)
        {
#if DEBUG
            if (args is null || args.Length == 0)
            {
                args = new[] { "run", @".\..\..\..\..\samples\internet-ezob.yml", "--user-variables", "UserName=name@email.com", "Password=your_password", "CheckoutLocation=E:\\Projects" };
            }
#endif
            var command = new RootCommand
            {
                Description = "Developer onboarding tool.",
            };

            command.AddGlobalOption(StandardOptions.NoDefaultOptions);

            command.AddCommand(CreateRunCommand());

            // Show commandline help unless a subcommand was used.
            command.Handler = CommandHandler.Create<IHelpBuilder>(help =>
            {
                help.Write(command);
                return 1;
            });

            var builder = new CommandLineBuilder(command);
            builder.UseHelp();
            builder.UseVersionOption();
            builder.UseDebugDirective();
            builder.UseParseErrorReporting();
            builder.ParseResponseFileAs(ResponseFileHandling.ParseArgsAsSpaceSeparated);

            builder.CancelOnProcessTermination();
            builder.UseExceptionHandler(HandleException);

            var parser = builder.Build();
            await parser.InvokeAsync(args);

            while (true)
            {
                Console.ReadLine();
            }
        }

        private static void HandleException(Exception exception, InvocationContext context)
        {
            context.Console.ResetTerminalForegroundColor();
            context.Console.SetTerminalForegroundColor(ConsoleColor.Red);

            if (exception is TargetInvocationException tie && tie.InnerException is object)
            {
                exception = tie.InnerException;
            }

            if (exception is OperationCanceledException)
            {
                context.Console.Error.WriteLine("Oh dear! Operation canceled.");
            }
            else if (exception is CommandException command)
            {
                context.Console.Error.WriteLine($"Drats! '{context.ParseResult.CommandResult.Command.Name}' failed:");
                context.Console.Error.WriteLine($"\t{command.Message}");

                if (command.InnerException != null)
                {
                    context.Console.Error.WriteLine();
                    context.Console.Error.WriteLine(command.InnerException.ToString());
                }
            }
            else if (exception is EzobYamlException yaml)
            {
                context.Console.Error.WriteLine($"{yaml.Message}");

                if (yaml.InnerException != null)
                {
                    context.Console.Error.WriteLine();
                    context.Console.Error.WriteLine(yaml.InnerException.ToString());
                }
            }
            else
            {
                context.Console.Error.WriteLine("An unhandled exception has occurred, how unseemly: ");
                context.Console.Error.WriteLine(exception.ToString());
            }

            context.Console.ResetTerminalForegroundColor();

            context.ResultCode = 1;
        }
    }
}
