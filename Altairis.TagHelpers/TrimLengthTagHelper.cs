using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {
    [HtmlTargetElement(ParentTag = null, Attributes = "trim-length")]
    public class TrimLengthTagHelper : TagHelper {

        public int TrimLength { get; set; }

        public string EllipsisText { get; set; } = "&hellip;";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
            await base.ProcessAsync(context, output);

            var content = output.Content.IsModified ? output.Content.GetContent() : (await output.GetChildContentAsync()).GetContent();
            content = WebUtility.HtmlDecode(content).Trim();

            if (content.Length > this.TrimLength) {
                if (context.AllAttributes["title"] == null) output.Attributes.Add("title", content);
                output.Content.SetHtmlContent(WebUtility.HtmlEncode(content.Substring(0, this.TrimLength).TrimEnd(' ', '.', ',', '-', '(', ')')) + this.EllipsisText);
            }
        }

    }
}
