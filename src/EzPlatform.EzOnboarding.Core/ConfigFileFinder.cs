using System.IO;

#pragma warning disable CA1021 // Avoid out parameters
namespace EzPlatform.EzOnboarding.Core
{
    public static class ConfigFileFinder
    {
        private static readonly string[] _fileFormats = { "ezob.yaml", "ezob.yml" };

        public static bool TryFindSupportedFile(string directoryPath, out string? filePath, out string? errorMessage, string[]? fileFormats = null)
        {
            fileFormats ??= _fileFormats;
            foreach (var format in fileFormats)
            {
                var files = Directory.GetFiles(directoryPath, format);

                switch (files.Length)
                {
                    case 1:
                        errorMessage = null;
                        filePath = files[0];
                        return true;
                    case 0:
                        continue;
                }

                errorMessage = $"More than one matching file was found in directory '{directoryPath}'.";
                filePath = default;
                return false;
            }

            errorMessage = $"No project project file or solution was found in directory '{directoryPath}'.";
            filePath = default;
            return false;
        }
    }
}
#pragma warning restore CA1021 // Avoid out parameters
