using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {

    [HtmlTargetElement("input", Attributes = "items")]
    public class DatalistTagHelper : TagHelper {

        public IEnumerable<string> Items { get; set; } = new HashSet<string>();

        public string DatalistId { get; set; }

        // public override int Order => int.MaxValue;

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            var datalistId = this.GetDatalistId(context, output);
            output.Attributes.Add("list", datalistId);

            var dataList = new TagBuilder("datalist");
            dataList.Attributes.Add("id", datalistId);
            dataList.InnerHtml.AppendLine();

            foreach (var item in this.Items) {
                var option = new TagBuilder("option") {
                    TagRenderMode = TagRenderMode.SelfClosing
                };
                option.Attributes.Add("value", item);
                dataList.InnerHtml.AppendLine(option);
            }

            output.PostElement.AppendLine().AppendLine(dataList);
        }

        private string GetDatalistId(TagHelperContext context, TagHelperOutput output) {
            if (!string.IsNullOrEmpty(this.DatalistId)) return this.DatalistId;

            var hasId = output.Attributes.TryGetAttribute("id", out var idAttr);
            return (hasId ? idAttr.Value : context.UniqueId) + "-datalist";
        }
    }
}
