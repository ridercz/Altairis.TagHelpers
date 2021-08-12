using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {
    [HtmlTargetElement("a")]
    [HtmlTargetElement("button")]
    [HtmlTargetElement("input", Attributes = "[type=submit]")]
    [HtmlTargetElement("input", Attributes = "[type=button]")]
    [HtmlTargetElement("input", Attributes = "[type=reset]")]
    public class ConfirmTagHelper : TagHelper {

        public string ConfirmMessage { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            if (!string.IsNullOrWhiteSpace(this.ConfirmMessage)) {
                output.Attributes.Add("onclick", $"return window.confirm('{this.ConfirmMessage.Replace("'", "\\'")}');");
            }
        }
    }
}
