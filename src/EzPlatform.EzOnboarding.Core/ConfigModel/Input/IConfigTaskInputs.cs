using System.Collections.Generic;

namespace EzPlatform.EzOnboarding.Core.ConfigModel.Input
{
    public interface IConfigTaskInputs
    {
        void AcceptUserVariables(Dictionary<string, string> vars);
    }
}
