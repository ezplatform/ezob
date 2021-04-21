using System.Collections.Generic;

namespace EzPlatform.EzOnboarding.Core.ConfigModel.Input
{
    public class InstallSoftwareInputs : IConfigTaskInputs
    {
        public string AssetAddress { get; set; } = default!;

        public string DownloadFilename { get; set; } = default!;

        public string DownloadLocation { get; set; } = "Downloads";

        public string InstallLocation { get; set; } = default!;

        public CommandMethod Command { get; set; } = new CommandMethod();

        public CommandMethod Validation { get; set; } = new CommandMethod();

        public bool AutoInstallation { get; set; }

        public string CommandExecutor
            => string.IsNullOrEmpty(Command.Cmd) ? Command.PwshPath : Command.CmdPath;

        public string ValidationExecutor
            => string.IsNullOrEmpty(Validation.Cmd) ? Validation.PwshPath : Validation.CmdPath;

        public string CommandProcessArgs => GetArgs(Command);

        public string ValidationProcessArgs => GetArgs(Validation);

        public void AcceptUserVariables(Dictionary<string, string> vars)
        {
            var pwshPath = vars.GetValueOrDefault(nameof(CommandMethod.PwshPath), string.Empty);
            var cmdPath = vars.GetValueOrDefault(nameof(CommandMethod.CmdPath), string.Empty);

            if (!string.IsNullOrEmpty(pwshPath))
            {
                Command.PwshPath = pwshPath;
                Validation.PwshPath = pwshPath;
            }

            if (!string.IsNullOrEmpty(cmdPath))
            {
                Command.CmdPath = cmdPath;
                Validation.CmdPath = cmdPath;
            }
        }

        private string GetArgs(CommandMethod command)
            => string.IsNullOrEmpty(command.Cmd)
                   ? $"--command \"{command.Pwsh}\""
                   : $"/c {command.Cmd}";
    }

    public class CommandMethod
    {
        public string CmdPath { get; set; } = @"C:\Windows\System32\cmd.exe";

        public string PwshPath { get; set; } = @"C:\Program Files\PowerShell\7\pwsh.exe";

        /// <summary>
        /// The command content that run by powershell core.
        /// </summary>
        public string? Pwsh { get; set; }

        /// <summary>
        /// The command content that run by cmd.
        /// </summary>
        public string? Cmd { get; set; }
    }
}
