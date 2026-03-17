namespace Altairis.TagHelpers.Charts;

[HtmlTargetElement("bar", ParentTag = "bar-chart")]
public class BarChartBarTagHelper : TagHelper {

    /// <summary>
    /// Internal immutable type that stores a single bar's data.
    /// </summary>
    /// <param name="Label">Human-readable label for the bar.</param>
    /// <param name="Color">Optional CSS color string used when rendering the bar.</param>
    /// <param name="Class">Optional CSS class to apply to the bar element.</param>
    /// <param name="Value">Numeric value represented by the bar. Must be non-negative.</param>
    internal record BarInfo(string Label, string? Color, string? Class, double Value);

    /// <summary>
    /// The label for the bar.
    /// </summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>
    /// Optional color for the bar. Can be any valid CSS color string (named color, hex, rgb, etc.).
    /// </summary>
    public string? Color { get; set; }

    /// <summary>
    /// Optional CSS class or classes to apply to the rendered bar element.
    /// </summary>
    public string? Class { get; set; }

    /// <summary>
    /// The numeric value of the bar. This value must be non-negative.
    /// </summary>
    /// <remarks>
    /// The parent `BarChartTagHelper` will use this value when computing scale and rendering.
    /// </remarks>
    public double Value { get; set; }

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
    /// Processes the `<bar>` tag by registering this bar's information with the parent chart.
    /// </summary>
    /// <param name="context">Context for the current tag processing.</param>
    /// <param name="output">Output stream to modify the generated HTML for this tag.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown when <see cref="Value"/> is negative.
    /// </exception>
    public override void Process(TagHelperContext context, TagHelperOutput output) {
        // Check if value is non-negative
        if (this.Value < 0) throw new ArgumentOutOfRangeException(nameof(this.Value), "Value must be non-negative.");

        // Retrieve or initialize the data list
        if (!this.ViewContext.ViewData.TryGetValue(BarChartTagHelper.BarChartDataKey, out var obj) || obj is not List<BarInfo> list) {
            list = [];
            this.ViewContext.ViewData[BarChartTagHelper.BarChartDataKey] = list;
        }

        // Add the new series to the list
        list.Add(new BarInfo(this.Label, this.Color, this.Class, this.Value));

        // Suppress output as this tag does not render anything itself
        output.SuppressOutput();
    }

}