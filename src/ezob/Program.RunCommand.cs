using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using EzPlatform.EzOnboarding.Core;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using EzPlatform.EzOnboarding.Core.ConfigModel.Input;
using Spectre.Console;

namespace ezob
{
    /// <summary>
    /// Program.RunCommand class.
    /// </summary>
    public static partial class Program
    {
        private static Command CreateRunCommand()
        {
            var command = new Command("run", "run the onboarding process")
            {
                CommonArguments.PathRequired,

                new Option<Dictionary<string, string>>(
                    new string[] { "-uvars", "--user-variables" },
                    parseArgument: argResult => argResult
                        .Tokens
                        .Select(t => t.Value.Split('='))
                        .ToDictionary(p => p[0], p => p[1]))
                {
                    Description = "User's variables.",
                    IsRequired = false
                },

                StandardOptions.Verbosity,
            };

            command.Handler = CommandHandler.Create<RunCommandArguments>(async args =>
            {
                // Workaround for https://github.com/dotnet/command-line-api/issues/723#issuecomment-593062654
                if (args.Path is null)
                {
                    throw new CommandException("No config file was found.");
                }

                var application = ConfigFactory.FromFile(args.Path);
                var outputContext = new OutputContext(args.Verbosity);
                var taskManager = new TaskManager();
                var dashboardRenderer = new DashboardRenderer(outputContext, taskManager);

                // Check and install prerequisite software.
                outputContext.Console.WriteLine("Checking prerequisites...");
                var autoInstalledTasks = application
                    .Stages
                    .SelectMany(p => p.Tasks)
                    .Where(x => x.Task == ConfigTaskType.InstallSoftware && ((InstallSoftwareInputs)x.Inputs).AutoInstallation)
                    .ToList();

                foreach (var task in autoInstalledTasks)
                {
                    var taskRunResult = await taskManager.RunTaskAsync(outputContext, task);
                }

                // Get to dashboard lifecycle.
                while (true)
                {
                    await dashboardRenderer.RenderDashboard(application);

                    var tasks = application.Stages.SelectMany(p => p.Tasks).ToList();
                    var taskCount = tasks.Count;

                    var choiceOptions = $"1-{taskCount}/exit";
                    var selectedStep = outputContext.Console.Prompt(
                        new TextPrompt<string>($"Select your onboarding step ({choiceOptions}):")
                            .Validate(choice =>
                            {
                                if ("exit".Equals("exit"))
                                {
                                    return ValidationResult.Success();
                                }

                                var isValid = int.TryParse(choice, out int selectedChoice);
                                if (!isValid || selectedChoice < 1 || selectedChoice > taskCount)
                                {
                                    return ValidationResult.Error("[red]Invalid option![/]");
                                }

                                return ValidationResult.Success();
                            }));

                    if (selectedStep.Equals("exit"))
                    {
                        break;
                    }

                    var selectedTask = tasks.First(p => p.Id == selectedStep);
                    selectedTask.AcceptUserVariables(args.UserVariables);

                    var taskRunResult = await taskManager.RunTaskAsync(outputContext, selectedTask);
                    if (taskRunResult.IsSuccess)
                    {
                        outputContext.MarkupInfoLine($"[green]{selectedTask.DisplayName} was completed successfully.[/]");
                    }
                    else
                    {
                        outputContext.MarkupInfoLine($"[red]{selectedTask.DisplayName} was failed.[/]");
                    }

                    outputContext.MarkupInfoLine($"[yellow]Task finished. Press any key to continue.[/]");
                    Console.ReadLine();
                }
            });

            return command;
        }

        private class RunCommandArguments
        {
            public FileInfo Path { get; set; } = default!;

            public Verbosity Verbosity { get; set; } = Verbosity.Info;

            public Dictionary<string, string> UserVariables { get; set; } = default!;
        }
    }
}
