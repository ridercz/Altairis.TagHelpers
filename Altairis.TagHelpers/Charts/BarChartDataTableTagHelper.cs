namespace Altairis.TagHelpers.Charts;

[HtmlTargetElement("data-table", ParentTag = "bar-chart")]
public class BarChartDataTableTagHelper : TagHelper {

    /// <summary>
    /// Contains the data-table configuration for a specific chart instance.
    /// </summary>
    /// <param name="Position">Where the data-table row should be rendered relative to the chart (top or bottom).</param>
    internal record DataTableInfo(BarChartItemPosition Position, string? TotalText, string? PercentFormatString, string ValueFormatString);

    /// <summary>
    /// Gets or sets the position where the data-table item will be rendered relative to the chart.
    /// </summary>
    /// <value>
    /// <see cref="BarChartItemPosition.Top"/> to render above the chart; <see cref="BarChartItemPosition.Bottom"/>
    /// to render below the chart. Defaults to <see cref="BarChartItemPosition.Bottom"/>.
    /// </value>
    public BarChartItemPosition Position { get; set; } = BarChartItemPosition.Bottom;

    /// <summary>
    /// Gets or sets optional display text for the total row in the data table.
    /// </summary>
    /// <value>
    /// A string to display for the total; if <c>null</c>, no explicit total text will be rendered.
    /// </value>
    public string? TotalText { get; set; }

    /// <summary>
    /// Gets or sets an optional format string used when rendering percentage values.
    /// </summary>
    /// <value>
    /// A standard or custom numeric format string for percentages, or <c>null</c> to use a default format.
    /// </value>
    public string? PercentFormatString { get; set; }

    /// <summary>
    /// Gets or sets the format string used when rendering numeric values in the data table.
    /// </summary>
    /// <value>
    /// A standard or custom numeric format string. Defaults to an empty string.
    /// </value>
    public string ValueFormatString { get; set; } = string.Empty;

    /// <summary>
    /// The current view context associated with the executing view.
    /// </summary>
    /// <remarks>
    /// Populated by the runtime because it is annotated with <see cref="ViewContextAttribute"/>.
    /// This tag helper uses <see cref="ViewContext.ViewData"/> to communicate the data-table
    /// configuration to the parent <see cref="BarChartTagHelper"/>.
    /// </remarks>
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    /// <summary>
    /// Processes the tag helper by storing the data-table configuration in <see cref="ViewContext.ViewData"/>.
    /// </summary>
    /// <remarks>
    /// This tag helper suppresses its own output because it only provides configuration data to the parent
    /// <c>bar-chart</c> tag helper. The configuration is stored under the key
    /// <see cref="BarChartTagHelper.BarChartDataTableKey"/>.
    /// </remarks>
    public override void Process(TagHelperContext context, TagHelperOutput output) {
        // Store the data-table configuration in ViewData
        this.ViewContext.ViewData[BarChartTagHelper.BarChartDataTableKey] = new DataTableInfo(this.Position, this.TotalText, this.PercentFormatString, this.ValueFormatString);

        // Suppress output - the tag does not render anything itself
        output.SuppressOutput();
    }

}
