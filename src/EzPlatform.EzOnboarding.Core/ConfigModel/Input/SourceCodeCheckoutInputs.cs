using System.Collections.Generic;

namespace EzPlatform.EzOnboarding.Core.ConfigModel.Input
{
    public class SourceCodeCheckoutInputs : IConfigTaskInputs
    {
        public string UserName { get; set; } = default!;

        public string Password { get; set; } = default!;

        public string CheckoutLocation { get; set; } = default!;

        public List<string> Sources { get; set; } = new List<string>();

        public void AcceptUserVariables(Dictionary<string, string> vars)
        {
            UserName = vars.GetValueOrDefault(nameof(UserName), UserName);
            Password = vars.GetValueOrDefault(nameof(Password), Password);
            CheckoutLocation = vars.GetValueOrDefault(nameof(CheckoutLocation), CheckoutLocation);
        }
    }
}
