using System;
using System.Linq;
using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using Spectre.Console;

namespace EzPlatform.EzOnboarding
{
    public class DashboardRenderer
    {
        private readonly OutputContext _context;
        private readonly TaskManager _taskManager;

        public DashboardRenderer(OutputContext context, TaskManager taskManager)
        {
            _context = context;
            _taskManager = taskManager;
        }

        public async Task RenderDashboard(ConfigApplication configApplication)
        {
            _context.Console.Clear(true);
            Tree stepTree = null;
            await _context.Console.Status()
                .Spinner(Spinner.Known.Star)
                .StartAsync("Checking your onboarding tasks status...", async ctx =>
                {
                    stepTree = await BuildStepTree(configApplication);
                });

            RenderWelcomeHeader();

            // Render onboarding tasks tree.
            _context.Console.Render(stepTree!);
        }

        private void RenderWelcomeHeader()
        {
            _context.Console.Render(new Rule("[green]Welcome Onboard![/]")
            {
                Alignment = Justify.Center
            });
        }

        private async Task<Tree> BuildStepTree(ConfigApplication configApplication)
        {
            int i = 0;
            foreach (var stage in configApplication.Stages)
            {
                foreach (var task in stage.Tasks)
                {
                    task.Id = (++i).ToString();
                }
            }

            var stepTree = new Tree("Onboarding Steps").Style("green on black");
            foreach (var stage in configApplication.Stages)
            {
                await CreateTaskNode(stage, stepTree);
            }

            return stepTree;
        }

        private async Task CreateTaskNode(ConfigStage stage, Tree stepTree)
        {
            if (stage == null)
            {
                throw new ArgumentNullException(nameof(stage));
            }

            if (stepTree == null)
            {
                throw new ArgumentNullException(nameof(stepTree));
            }

            var sourceCodeSetupNode = stepTree.AddNode($"[yellow]{stage.DisplayName}[/]");
            var steps = stage.Tasks;
            if (!steps.Any())
            {
                return;
            }

            foreach (var task in steps)
            {
                var isDone = await _taskManager.IsTaskCompleted(_context, task);

                var nodeText = new Markup($"[{(isDone ? "green" : "yellow")}]{task.Id}. {task.DisplayName} {(isDone ? "(✓ Finished)" : string.Empty)}[/]");
                sourceCodeSetupNode.AddNode(nodeText);
            }
        }
    }
}
