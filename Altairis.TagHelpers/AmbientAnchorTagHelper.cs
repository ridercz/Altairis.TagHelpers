using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {

    [HtmlTargetElement("aa", Attributes = ActionAttributeName)]
    [HtmlTargetElement("aa", Attributes = ControllerAttributeName)]
    [HtmlTargetElement("aa", Attributes = AreaAttributeName)]
    [HtmlTargetElement("aa", Attributes = PageAttributeName)]
    [HtmlTargetElement("aa", Attributes = PageHandlerAttributeName)]
    [HtmlTargetElement("aa", Attributes = FragmentAttributeName)]
    [HtmlTargetElement("aa", Attributes = HostAttributeName)]
    [HtmlTargetElement("aa", Attributes = ProtocolAttributeName)]
    [HtmlTargetElement("aa", Attributes = RouteAttributeName)]
    [HtmlTargetElement("aa", Attributes = RouteValuesDictionaryName)]
    [HtmlTargetElement("aa", Attributes = RouteValuesPrefix + "*")]
    public class AmbientAnchorTagHelper : AnchorTagHelper {
        private const string ActionAttributeName = "asp-action";
        private const string ControllerAttributeName = "asp-controller";
        private const string AreaAttributeName = "asp-area";
        private const string PageAttributeName = "asp-page";
        private const string PageHandlerAttributeName = "asp-page-handler";
        private const string FragmentAttributeName = "asp-fragment";
        private const string HostAttributeName = "asp-host";
        private const string ProtocolAttributeName = "asp-protocol";
        private const string RouteAttributeName = "asp-route";
        private const string RouteValuesDictionaryName = "asp-all-route-data";
        private const string RouteValuesPrefix = "asp-route-";

        public AmbientAnchorTagHelper(IHtmlGenerator generator) : base(generator) { }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            // Make classic anchor tag
            output.TagName = "a";

            // Copy values from current route data
            foreach (var key in this.ViewContext.RouteData.Values.Keys) {
                if (!this.RouteValues.ContainsKey(key)) this.RouteValues[key] = this.ViewContext.RouteData.Values[key].ToString();
            }

            // Process standard anchor helper
            base.Process(context, output);
        }

    }
}