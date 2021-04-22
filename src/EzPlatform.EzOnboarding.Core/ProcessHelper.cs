using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#pragma warning disable SA1300 // Element should begin with upper-case letter
#pragma warning disable SA1202 // Elements should be ordered by access
namespace EzPlatform.EzOnboarding.Core
{
    public static class ProcessHelper
    {
        private static readonly bool _isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        [DllImport("libc", SetLastError = true, EntryPoint = "kill")]

        private static extern int sys_kill(int pid, int sig);

        public static async Task<ProcessResult> RunAsync(
            string filename,
            string arguments,
            string? workingDirectory = null,
            bool throwOnError = true,
            IDictionary<string, string>? environmentVariables = null,
            Action<string>? outputDataReceived = null,
            Action<string>? errorDataReceived = null,
            Action<int>? onStart = null,
            Action<int>? onStop = null,
            CancellationToken cancellationToken = default)
        {
            using var process = new Process
            {
                StartInfo =
                {
                    FileName = filename,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = !_isWindows,
                    WindowStyle = ProcessWindowStyle.Hidden
                },
                EnableRaisingEvents = true
            };

            if (workingDirectory != null)
            {
                process.StartInfo.WorkingDirectory = workingDirectory;
            }

            if (environmentVariables != null)
            {
                foreach (var kvp in environmentVariables)
                {
                    process.StartInfo.Environment.Add(kvp!);
                }
            }

            var outputBuilder = new StringBuilder();
            process.OutputDataReceived += (_, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }

                if (outputDataReceived != null)
                {
                    outputDataReceived.Invoke(e.Data);
                }
                else
                {
                    outputBuilder.AppendLine(e.Data);
                }
            };

            var errorBuilder = new StringBuilder();
            process.ErrorDataReceived += (_, e) =>
            {
                if (e.Data == null)
                {
                    return;
                }

                if (errorDataReceived != null)
                {
                    errorDataReceived.Invoke(e.Data);
                }
                else
                {
                    errorBuilder.AppendLine(e.Data);
                }
            };

            var processExitEvent = new TaskCompletionSource<object>();
            process.Exited += (sender, args) =>
            {
                processExitEvent.TrySetResult(true);
            };

            process.Start();
            onStart?.Invoke(process.Id);

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            var processResult = new ProcessResult();

            await processExitEvent.Task;
            processResult.ExitCode = process.ExitCode;

            // -> Timeout, let's kill the process
            try
            {
                if (!_isWindows)
                {
                    sys_kill(process.Id, sig: 2); // SIGINT
                }
                else
                {
                    if (!process.CloseMainWindow())
                    {
                        process.Kill();
                    }
                }
            }
            catch
            {
                // ignored
            }

            processResult.StandardOutput = outputBuilder.ToString();
            processResult.StandardError = errorBuilder.ToString();

            return processResult;
        }

        public static Task<ProcessResult> RunAsync(ProcessSpec processSpec, CancellationToken cancellationToken = default, bool throwOnError = true)
        {
            return RunAsync(
                processSpec.Executable!,
                processSpec.Arguments!,
                processSpec.WorkingDirectory,
                throwOnError: throwOnError,
                processSpec.EnvironmentVariables,
                processSpec.OutputData,
                processSpec.ErrorData,
                processSpec.OnStart,
                processSpec.OnStop,
                cancellationToken);
        }

        public static void KillProcess(int pid)
        {
            try
            {
                using var process = Process.GetProcessById(pid);
                process?.Kill();
            }
            catch (ArgumentException)
            {
                // skip.
            }
            catch (InvalidOperationException)
            {
                // skip.
            }
        }
    }
}
#pragma warning restore SA1300 // Element should begin with upper-case letter
#pragma warning restore SA1202 // Elements should be ordered by access
