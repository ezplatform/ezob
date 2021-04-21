using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.IO;
using EzPlatform.EzOnboarding.Core;

namespace ezob
{
    internal static class CommonArguments
    {
        public static Argument<FileInfo> PathOptional
        {
            get
            {
                return new Argument<FileInfo>((r) => TryParsePath(r, required: false), isDefault: true)
                {
                    Arity = ArgumentArity.ZeroOrOne,
                    Description = "file can be a yaml or yml",
                    Name = "path",
                };
            }
        }

        public static Argument<FileInfo> PathRequired
        {
            get
            {
                return new Argument<FileInfo>((r) => TryParsePath(r, required: true), isDefault: true)
                {
                    Arity = ArgumentArity.ZeroOrOne,
                    Description = "file can be a yaml or yml",
                    Name = "path",
                };
            }
        }

        private static FileInfo TryParsePath(ArgumentResult result, bool required)
        {
            var token = result.Tokens.Count switch
            {
                0 => ".",
                1 => result.Tokens[0].Value,
                _ => throw new InvalidOperationException("Unexpected token count."),
            };

            if (string.IsNullOrEmpty(token))
            {
                token = ".";
            }

            if (File.Exists(token))
            {
                return new FileInfo(token);
            }

            if (Directory.Exists(token))
            {
                if (ConfigFileFinder.TryFindSupportedFile(token, out var filePath, out var errorMessage))
                {
                    return new FileInfo(filePath!);
                }
                else if (required)
                {
                    result.ErrorMessage = errorMessage;
                    return default!;
                }
                else
                {
                    return default!;
                }
            }

            result.ErrorMessage = $"The file '{token}' could not be found.";
            return default!;
        }
    }
}
