namespace Altairis.TagHelpers.Charts;

[HtmlTargetElement("title", ParentTag = "bar-chart")]
public class BarChartTitleTagHelper : TagHelper {

    /// <summary>
    /// Contains the title content and the requested title position for a specific chart.
    /// </summary>
    internal record TitleInfo(string Content, BarChartItemPosition Position);

    /// <summary>
    /// Gets or sets the requested position for the title. Defaults to <see cref="BarChartItemPosition.Top"/>.
    /// </summary>
    public BarChartItemPosition Position { get; set; } = BarChartItemPosition.Top;

    /// <summary>
    /// The current view context, provided by the framework.
    /// </summary>
    /// <remarks>
    /// This property is populated by the runtime because it is annotated with
    /// the <see cref="ViewContextAttribute"/>. The tag helper uses <see cref="ViewContext.ViewData"/>
    /// to communicate with the parent `BarChartTagHelper`.
    /// </remarks>
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    /// <summary>
    /// Asynchronously processes the `<title>` child tag of a `<bar-chart>` tag.
    /// </summary>
    /// <exception cref="Exception">
    /// Thrown when the tag helper is used outside of a `BarChartTagHelper` context (no current chart id available in <see cref="ViewContext.ViewData"/>).
    /// </exception>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
        // Store the title content in ViewData
        var content = await output.GetChildContentAsync();
        this.ViewContext.ViewData[BarChartTagHelper.BarChartTitleKey] = new TitleInfo(content.GetContent(), this.Position);

        // Suppress output as this tag does not render anything itself
        output.SuppressOutput();
    }

}
