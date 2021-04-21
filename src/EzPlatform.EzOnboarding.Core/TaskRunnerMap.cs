using System;
using System.Collections.Generic;
using EzPlatform.EzOnboarding.Core.ConfigModel;
using EzPlatform.EzOnboarding.Core.Tasks;

namespace EzPlatform.EzOnboarding.Core
{
    public class TaskRunnerMap
    {
        public TaskRunnerMap()
        {
            Map = new Dictionary<ConfigTaskType, Type>
            {
                { ConfigTaskType.CreateTicket, typeof(CreateTicketTaskProcessor) },
                { ConfigTaskType.InstallSoftware, typeof(InstallSoftwareTaskProcessor) },
                { ConfigTaskType.SourceCodeCheckout, typeof(SourceCodeCheckoutTaskProcessor) },
            };
        }

        public Dictionary<ConfigTaskType, Type> Map { get; }
    }
}
