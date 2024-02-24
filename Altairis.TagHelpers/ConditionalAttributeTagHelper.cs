namespace Altairis.TagHelpers;

[HtmlTargetElement("*", Attributes = "conditional-attribute-name,conditional-attribute-state")]
public class ConditionalAttributeTagHelper : TagHelper {

    public string ConditionalAttributeName { get; set; } = string.Empty;

    public string? ConditionalAttributeValue { get; set; }

    public bool ConditionalAttributeState { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        if (this.ConditionalAttributeState) output.Attributes.SetAttribute(this.ConditionalAttributeName, this.ConditionalAttributeValue ?? this.ConditionalAttributeName);
    }

}
