using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Altairis.TagHelpers {
    public class CheckboxListTagHelper : TagHelper {

        [HtmlAttributeName("asp-for")]
        public ModelExpression For { get; set; }

        [HtmlAttributeName("asp-items")]
        public IEnumerable<SelectListItem> Items { get; set; }

        public string Class { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            output.TagName = "ul";
            if (!string.IsNullOrWhiteSpace(this.Class)) output.Attributes.Add("class", this.Class);

            var items = new List<SelectListItem>(this.Items);
            for (var i = 0; i < items.Count; i++) {
                var fieldName = this.For.Name;
                var fieldId = $"{this.For.Name.Replace('.', '_')}[{i}]";

                // Create checkbox
                var input = new TagBuilder("input") {
                    TagRenderMode = TagRenderMode.SelfClosing
                };
                input.Attributes.Add("type", "checkbox");
                input.Attributes.Add("name", fieldName);
                input.Attributes.Add("id", fieldId);
                input.Attributes.Add("value", items[i].Value);
                if (items[i].Selected) input.Attributes.Add("checked", "checked");
                if (items[i].Disabled) input.Attributes.Add("disabled", "disabled");

                // Create label
                var label = new TagBuilder("label");
                label.Attributes.Add("for", fieldId);
                label.InnerHtml.Append(items[i].Text);

                // Create list item with contents
                var li = new TagBuilder("li");
                li.InnerHtml.AppendHtml(input);
                li.InnerHtml.AppendHtml(label);
                output.Content.AppendHtml(li);
            }
        }
    }
}
