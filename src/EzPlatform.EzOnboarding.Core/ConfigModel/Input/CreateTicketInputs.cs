using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EzPlatform.EzOnboarding.Core.ConfigModel.Input
{
    public class CreateTicketInputs : IConfigTaskInputs
    {
        [Required]
        public Uri Url { get; set; } = default!;

        [Required]
        public string Content { get; set; } = default!;

        public void AcceptUserVariables(Dictionary<string, string> vars)
        {
        }
    }
}
