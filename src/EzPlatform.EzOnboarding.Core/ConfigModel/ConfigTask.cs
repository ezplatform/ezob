using System.Collections.Generic;
using EzPlatform.EzOnboarding.Core.ConfigModel.Input;

namespace EzPlatform.EzOnboarding.Core.ConfigModel
{
    public class ConfigTask
    {
        /// <summary>
        /// Task Id is used for user's selection.
        /// </summary>
        public string Id { get; set; } = "NotSet";

        public ConfigTaskType Task { get; set; }

        public string? DisplayName { get; set; }

        public IConfigTaskInputs Inputs { get; set; } = new EmptyTaskInputs();

        public void AcceptUserVariables(Dictionary<string, string> userVariables)
        {
            Inputs.AcceptUserVariables(userVariables);
        }
    }
}
