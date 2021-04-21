using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using EzPlatform.EzOnboarding.Core.ConfigModel.Input;

namespace EzPlatform.EzOnboarding.Core.Tasks
{
    public class SourceCodeCheckoutTaskProcessor : BaseTaskProcessor
    {
        public override async Task<TaskExecutionResult> InternalProcessAsync(OutputContext context, ConfigTask configTask)
        {
            var inputs = (configTask.Inputs as SourceCodeCheckoutInputs)!;

            context.WriteInfoLine($"Storing repositories in: {inputs.CheckoutLocation}...");

            var gitUserName = Uri.EscapeDataString(inputs.UserName);
            var gitPassword = Uri.EscapeDataString(inputs.Password);

            foreach (var gitSourceUrl in inputs.Sources)
            {
                context.WriteInfoLine($"\n\nProcessing for repository: {gitSourceUrl}...");

                var currentRepoDir = GetProjectFolder(inputs.CheckoutLocation, gitSourceUrl);

                /*****
                /*  Following git rules: if the specific folder that used for cloning the source to is exist
                /*  but is not empty, then skip it. In our context, we will skip to clone those kind of repositories,
                /*  users can delete the left items in those folders and re-run the tool once again!
                *****/
                if (Directory.Exists(currentRepoDir))
                {
                    // In case the directory is exit but there is nothing in there, we shall remove it and clone again.
                    if (IsDirectoryEmpty(currentRepoDir))
                    {
                        Directory.Delete(currentRepoDir, true);
                    }
                    else
                    {
                        context.WriteInfoLine($"the current folder ${currentRepoDir} is exist but is not empty, clean it and retry later!");
                        continue;
                    }
                }

                if (!Directory.Exists(currentRepoDir))
                {
                    Directory.CreateDirectory(currentRepoDir);
                }

                context.WriteInfoLine($"\tCloning repository: {gitSourceUrl}...");

                // detect git.
                var gitCheck = await ProcessHelper.RunAsync("git", "--version", throwOnError: false);

                // Add git credential
                var credentialGitSourceUrl = gitSourceUrl.Replace("https://", $"https://{gitUserName}:{gitPassword}@");
                var cloneProcess = await ProcessHelper.RunAsync("git", $"clone {credentialGitSourceUrl} {currentRepoDir}", throwOnError: false);

                if (cloneProcess.IsSuccess)
                {
                    context.MarkupInfoLine($"[green]Clone successfully to {gitSourceUrl}[/]");
                }
                else
                {
                    context.WriteInfoLine(cloneProcess.StandardOutput);
                }
            }

            return new TaskExecutionResult();
        }

        public override Task<bool> IsCompleted(OutputContext context, ConfigTask configTask)
        {
            var inputs = (configTask.Inputs as SourceCodeCheckoutInputs)!;

            foreach (var repo in inputs.Sources)
            {
                var currentRepoDir = GetProjectFolder(inputs.CheckoutLocation, repo);

                // If any repo folder is empty, we shall consider the task as not completed.
                if (IsDirectoryEmpty(currentRepoDir))
                {
                    return Task.FromResult(false);
                }
            }

            return Task.FromResult(false);
        }

        private static string GetProjectFolder(string checkoutLocationPath, string gitChetLink)
        {
            // We will avoid to use Path.GetFileName, that won't work if the path ends in a \.
            var folderName = new DirectoryInfo(gitChetLink).Name.Replace(".git", string.Empty, StringComparison.CurrentCultureIgnoreCase);
            return Path.Combine(checkoutLocationPath, folderName);
        }

        private static bool IsDirectoryEmpty(string path)
        {
            if (!Directory.Exists(path))
            {
                return true;
            }

            return !Directory.EnumerateFiles(path).Any();
        }
    }
}
