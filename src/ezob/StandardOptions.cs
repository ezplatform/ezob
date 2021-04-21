using System.CommandLine;
using EzPlatform.EzOnboarding.Core;

namespace ezob
{
    public static class StandardOptions
    {
        public static Option NoDefaultOptions
        {
            get
            {
                return new Option(new[] { "--no-default" })
                {
                    Description = "Disable default options from environment variables",
                    IsRequired = false,
                    Argument = new Argument<bool>(),
                };
            }
        }

        public static Option Verbosity
        {
            get
            {
                return new Option(new[] { "-v", "--verbosity" }, "Output verbosity")
                {
                    Argument = new Argument<Verbosity>("one of: quiet|info|debug", () => EzPlatform.EzOnboarding.Core.Verbosity.Info)
                    {
                        Arity = ArgumentArity.ExactlyOne,
                    },
                    IsRequired = false,
                };
            }
        }
    }
}
