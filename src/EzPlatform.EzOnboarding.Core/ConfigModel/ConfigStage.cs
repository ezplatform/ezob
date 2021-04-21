using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EzPlatform.EzOnboarding.Core.ConfigModel
{
    public class ConfigStage
    {
        private const string ErrorMessage = "A stage name must consist of lower case alphanumeric characters or '-'," +
                                            " start with an alphabetic character, and end with an alphanumeric character" +
                                            " (e.g. 'my-name',  or 'abc-123', regex used for validation is '[a-z]([-a-z0-9]*[a-z0-9])?').";

        private const string MaxLengthErrorMessage = "Name cannot be more that 63 characters long.";

        [Required]
        [RegularExpression("[a-z]([-a-z0-9]*[a-z0-9])?", ErrorMessage = ErrorMessage)]
        [MaxLength(63, ErrorMessage = MaxLengthErrorMessage)]
        public string Stage { get; set; } = default!;

        public string? DisplayName { get; set; }

        public string? DependsOn { get; set; }

        public List<ConfigTask> Tasks { get; set; } = new List<ConfigTask>();
    }
}
