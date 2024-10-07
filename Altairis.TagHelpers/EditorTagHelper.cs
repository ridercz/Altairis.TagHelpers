namespace Altairis.TagHelpers;

[HtmlTargetElement("editor", Attributes = "asp-for")]
public class EditorTagHelper(IHtmlHelper htmlHelper) : TagHelper {
    private readonly IHtmlHelper htmlHelper = htmlHelper ?? throw new ArgumentNullException(nameof(htmlHelper));

    [HtmlAttributeName("asp-for")]
    public ModelExpression For { get; set; } = null!;

    public string? TemplateName { get; set; }

    [ViewContext, HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; } = null!;

    public override void Process(TagHelperContext context, TagHelperOutput output) {
        base.Process(context, output);
        var awareHtmlHelper = (IViewContextAware)this.htmlHelper;
        awareHtmlHelper.Contextualize(this.ViewContext);
        output.TagName = null;
        output.Content.SetHtmlContent(this.htmlHelper.Editor(this.For.Name, this.TemplateName));
    }

}
