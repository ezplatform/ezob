using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using EzPlatform.EzOnboarding.Core.ConfigModel.Input;

namespace EzPlatform.EzOnboarding.Core.Tasks
{
    public class InstallSoftwareTaskProcessor : BaseTaskProcessor
    {
        public override async Task<TaskExecutionResult> InternalProcessAsync(OutputContext context, ConfigTask configTask)
        {
            var inputs = configTask.Inputs as InstallSoftwareInputs ?? throw new ArgumentNullException(nameof(configTask));

            var result = new TaskExecutionResult();

            var downloadPath = Path.Combine(Directory.GetCurrentDirectory(), "Downloads");
            if (!Directory.Exists(downloadPath))
            {
                Directory.CreateDirectory(downloadPath);
            }

            // Download file to local file or copy to download folder
            var softwareAsset = inputs.AssetAddress;
            if (!string.IsNullOrEmpty(softwareAsset))
            {
                if (IsValidUrl(softwareAsset))
                {
                    using (var client = new WebClient())
                    {
                        client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                        context.WriteInfoLine($"Downloading {inputs.DownloadFilename}...");
                        var downloadFilePath = Path.Combine(downloadPath, inputs.DownloadFilename);
                        await client.DownloadFileTaskAsync(new Uri(softwareAsset), downloadFilePath);
                    }
                }
                else
                {
                    File.Copy(inputs.AssetAddress, inputs.DownloadLocation);
                }
            }

            // Get command list from Powershell or Cmd string.
            var commands = string.IsNullOrEmpty(inputs.Command.Cmd) ?
                inputs.Command.Pwsh!.TrimEnd('\n').Split("\n") :
                inputs.Command.Cmd!.TrimEnd('\n').Split("\n");

            foreach (var command in commands)
            {
                var processResult = await ProcessHelper.RunAsync(
                    filename: inputs.CommandExecutor,
                    arguments: inputs.CommandProcessArgs,
                    workingDirectory: Path.Combine(Directory.GetCurrentDirectory(), inputs.DownloadLocation),
                    outputDataReceived: context.WriteInfoLine,
                    errorDataReceived: data =>
                    {
                        context.WriteInfoLine(data);
                        result.AddError(data);
                    },
                    throwOnError: false);

                if (!processResult.IsSuccess)
                {
                    result.AddError($"Failed running command \"{command}\".");
                }
            }

            return result;
        }

        public override async Task<bool> IsCompleted(OutputContext context, ConfigTask configTask)
        {
            var inputs = (configTask.Inputs as InstallSoftwareInputs)!;
            try
            {
                var processResult = await ProcessHelper.RunAsync(
                    filename: inputs.ValidationExecutor,
                    arguments: inputs.ValidationProcessArgs,
                    throwOnError: false);

                var processOutput = processResult.StandardOutput.Trim();

                return processOutput.Equals("ok", StringComparison.OrdinalIgnoreCase)
                    || !string.IsNullOrEmpty(processOutput);
            }
            catch (Exception ex)
            {
                context.WriteInfoLine("Check software status failed. Detail: " + ex.Message);

                // By default return false as not installed.
                return false;
            }
        }

        private bool IsValidUrl(string url) =>
            url.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase)
            || url.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase);
    }
}
