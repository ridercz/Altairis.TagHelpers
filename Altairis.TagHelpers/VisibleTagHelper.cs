using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {

    [HtmlTargetElement(Attributes = "visible")]
    public class VisibleTagHelper : TagHelper {

        public bool Visible { get; set; } = true;

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);
            if (!this.Visible) output.SuppressOutput();
        }

    }
}
