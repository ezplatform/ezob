using System;
using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using EzPlatform.EzOnboarding.Core.ConfigModel.Input;
using Spectre.Console;

namespace EzPlatform.EzOnboarding.Core.Tasks
{
    public class CreateTicketTaskProcessor : BaseTaskProcessor
    {
        public override Task<TaskExecutionResult> InternalProcessAsync(OutputContext context, ConfigTask configTask)
        {
            var inputs = configTask.Inputs as CreateTicketInputs ?? throw new ArgumentNullException(nameof(configTask));
            var result = new TaskExecutionResult();

            context.Console.Render(new Rule($"[green]{configTask.DisplayName}[/]") { Alignment = Justify.Center });

            var table = new Table();
            table
                .AddColumn(new TableColumn("Title"))
                .AddColumn(new TableColumn("Content"))
                .Border(TableBorder.MinimalDoubleHead)
                .HideHeaders();

            table.Columns[0].Width(15);

            table.AddRow("[bold]Ticket request url:[/]", $"[green]{inputs.Url}[/]");
            table.AddRow("---------------", "---------------------------------------------------------------------------------------------");

            table.AddRow("[bold]Guideline:[/]", $"[yellow]{inputs.Content}[/]");

            context.Console.Render(table);

            var selectedStep = context.Console.Prompt(
                  new TextPrompt<string>("Have you finished the task?")
                      .InvalidChoiceMessage("[red]Invalid option!")
                      .AddChoice("y")
                      .AddChoice("n"));

            if (selectedStep.Equals("n", StringComparison.CurrentCultureIgnoreCase))
            {
                TaskStatusTracker.Instance.Update(configTask.Id, false);
                result.AddError("You skipped the task!");
            }
            else
            {
                TaskStatusTracker.Instance.Update(configTask.Id, true);
            }

            return Task.FromResult(result);
        }

        public override Task<bool> IsCompleted(OutputContext context, ConfigTask configTask)
        {
            var result = TaskStatusTracker.Instance.IsTaskCompleted(configTask.Id);
            return Task.FromResult(result);
        }
    }
}
