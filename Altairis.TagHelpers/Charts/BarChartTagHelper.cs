using System.Globalization;

namespace Altairis.TagHelpers.Charts;

[HtmlTargetElement("bar-chart")]
public class BarChartTagHelper : TagHelper {

    /// <summary>
    /// Default CSS class applied to the bar chart container.
    /// </summary>
    private const string DefaultCssClass = "bar-chart";

    /// <summary>
    /// ViewData key for storing a chart instance's series data.
    /// </summary>
    internal const string BarChartDataKey = "Altairis.TagHelpers.BarChart.Data";

    /// <summary>
    /// ViewData key for storing a chart instance's title.
    /// </summary>
    internal const string BarChartTitleKey = "Altairis.TagHelpers.BarChart.Title";

    /// <summary>
    /// ViewData key for storing a chart instance's data-table configuration.
    /// </summary>
    internal const string BarChartDataTableKey = "Altairis.TagHelpers.BarChart.DataTable";

    /// <summary>
    /// Collection of bar data registered by child <c>&lt;bar&gt;</c> elements.
    /// </summary>
    private List<BarChartBarTagHelper.BarInfo> barDataList = [];

    /// <summary>
    /// ID of the title <c>&lt;figcaption&gt;</c> element, if any.
    /// </summary>
    private string? titleId;

    /// <summary>
    /// Indicates whether a data table is present for this chart.
    /// </summary>
    private bool hasDataTable = false;

    /// <summary>
    /// When true, bar segments are rendered stacked horizontally inside the same container
    /// (each bar's width represents its portion of the total). When false, each bar's width
    /// is relative to the maximum series value.
    /// </summary>
    public bool Stacked { get; set; } = false;

    /// <summary>
    /// When true, labels are shown in the chart output.
    /// </summary>
    public bool ShowLabels { get; set; } = false;

    /// <summary>
    /// The aria-label used for the chart container when no title is present.
    /// </summary>
    public string AriaLabel { get; set; } = "Bar chart";

    /// <summary>
    /// The aria-label used for the data table when no title is present.
    /// </summary>
    public string ValueTableAriaLabel { get; set; } = "Bar chart value table";

    /// <summary>
    /// The current <see cref="ViewContext"/> instance. This property is injected by the runtime
    /// and is used to share data (ViewData) between the chart tag helper and its child bar tag helpers.
    /// </summary>
    [ViewContext]
    public ViewContext ViewContext { get; set; } = null!;

    /// <summary>
    /// Asynchronously processes the <c>&lt;bar-chart&gt;</c> tag. A unique id is generated for
    /// this chart instance and stored in <see cref="ViewContext.ViewData"/> so nested
    /// <c>&lt;bar&gt;</c> elements can register their data. After child content is executed,
    /// the method reads the collected series data and builds the final HTML output.
    /// </summary>
    /// <param name="context">The <see cref="TagHelperContext"/> for this execution.</param>
    /// <param name="output">The <see cref="TagHelperOutput"/> used to modify the rendered output.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    /// <remarks>
    /// The method:
    /// - Calls <see cref="TagHelperOutput.GetChildContentAsync"/> to allow child tag helpers to
    ///   populate series data into ViewData.
    /// - Reads the series list using <see cref="BarChartDataKey"/> and renders bar elements
    ///   with appropriate CSS classes and inline styles (width, color, float).
    /// </remarks>
    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output) {
        // Get a unique ID for this bar chart instance
        var currentId = context.AllAttributes.TryGetAttribute("id", out var idAttr) ? idAttr.Value?.ToString() ?? context.UniqueId : context.UniqueId;

        // Process child content to allow child tag helpers to register their data
        await output.GetChildContentAsync();

        // Retrieve bar data
        if (!this.ViewContext.ViewData.TryGetValue(BarChartDataKey, out var obj) || obj is not List<BarChartBarTagHelper.BarInfo> barDataList) {
            output.SuppressOutput();
            return;
        }
        this.barDataList = barDataList;

        // Retrieve title if available
        var titleInfo = this.ViewContext.ViewData.TryGetValue(BarChartTitleKey, out var titleObj2) && titleObj2 is BarChartTitleTagHelper.TitleInfo ti ? ti : null;
        this.titleId = titleInfo != null ? $"{currentId}-title" : null;

        // Retrieve data-table configuration if available
        var dataTableInfo = this.ViewContext.ViewData.TryGetValue(BarChartDataTableKey, out var dtObj) && dtObj is BarChartDataTableTagHelper.DataTableInfo dti ? dti : null;
        this.hasDataTable = dataTableInfo != null;

        // Build the output tag
        output.TagName = "figure";
        if (!output.Attributes.TryGetAttribute("class", out _)) output.Attributes.SetAttribute("class", DefaultCssClass);

        // Accessibility: set role and label references
        output.Attributes.SetAttribute("role", "img");
        if (!string.IsNullOrEmpty(this.titleId)) {
            output.Attributes.SetAttribute("aria-labelledby", this.titleId);
        } else {
            output.Attributes.SetAttribute("aria-label", this.AriaLabel);
        }

        // Add title if available and is on top
        if (titleInfo != null && titleInfo.Position == BarChartItemPosition.Top) this.AddTitle(output, titleInfo);

        // Add data table if available and is on top
        if (dataTableInfo != null && dataTableInfo.Position == BarChartItemPosition.Top) this.AddDataTable(output, dataTableInfo);

        // Add bars
        this.AddBars(output);

        if (this.Stacked) {
            // Add clearfix when stacked
            var clearfix = new TagBuilder("div");
            clearfix.Attributes["style"] = "clear:left;height:0;line-height:0;";
            output.Content.AppendHtml(clearfix);

            // Add labels below the chart when requested
            if (this.ShowLabels) this.AddLabelsForStackedChart(output);
        }

        // Add data table if available and is on top
        if (dataTableInfo != null && dataTableInfo.Position == BarChartItemPosition.Bottom) this.AddDataTable(output, dataTableInfo);

        // Add title if available and is at bottom
        if (titleInfo != null && titleInfo.Position == BarChartItemPosition.Bottom) this.AddTitle(output, titleInfo);

        // Clean up ViewData
        this.ViewContext.ViewData.Remove(BarChartDataKey);
        this.ViewContext.ViewData.Remove(BarChartTitleKey);
        this.ViewContext.ViewData.Remove(BarChartDataTableKey);
    }

    /// <summary>
    /// Builds and appends a set of labels for a stacked chart.
    /// </summary>
    private void AddLabelsForStackedChart(TagHelperOutput output) {
        var labels = new TagBuilder("aside");
        labels.Attributes["aria-hidden"] = "true";
        foreach (var barData in this.barDataList) {
            var label = new TagBuilder("label");
            if (!string.IsNullOrEmpty(barData.Class)) label.AddCssClass(barData.Class);

            var squareSpan = new TagBuilder("span");
            squareSpan.InnerHtml.AppendHtml("&#x25C6;");
            if (!string.IsNullOrEmpty(barData.Color)) squareSpan.Attributes["style"] = $"color:{barData.Color};";
            label.InnerHtml.AppendHtml(squareSpan);

            label.InnerHtml.AppendHtml($"{barData.Label}: {barData.Value}");
            labels.InnerHtml.AppendHtml(label);
        }
        output.Content.AppendHtml(labels);
    }

    /// <summary>
    /// Appends a title element (<c>&lt;figcaption&gt;</c>) to the output containing the provided title content.
    /// </summary>
    private void AddTitle(TagHelperOutput output, BarChartTitleTagHelper.TitleInfo titleInfo) {
        var titleTag = new TagBuilder("figcaption");
        titleTag.Attributes["id"] = this.titleId!;
        titleTag.InnerHtml.AppendHtml(titleInfo.Content);
        output.Content.AppendHtml(titleTag);
    }

    /// <summary>
    /// Renders the bar elements for the chart based on provided bar data list and stacking mode.
    /// </summary>
    private void AddBars(TagHelperOutput output) {
        var scalingDataValue = this.Stacked ? this.barDataList.Sum(x => x.Value) : this.barDataList.Max(x => x.Value);

        var sizePercentUsed = 0d;
        foreach (var barData in this.barDataList) {
            var bar = new TagBuilder("div");
            bar.Attributes["title"] = $"{barData.Label}: {barData.Value}";
            if (!string.IsNullOrEmpty(barData.Class)) bar.AddCssClass(barData.Class);

            if (this.hasDataTable) {
                bar.Attributes["aria-hidden"] = "true";
            } else {
                bar.Attributes["role"] = "img";
                bar.Attributes["aria-label"] = $"{barData.Label}: {barData.Value}";
            }

            var sizePercent = Math.Round(scalingDataValue > 0 ? (barData.Value / scalingDataValue) * 100 : 0, 2);
            if (this.Stacked && barData == this.barDataList.Last()) sizePercent = 100 - sizePercentUsed;
            var sizePercentStr = sizePercent.ToString("0.##", CultureInfo.InvariantCulture);
            sizePercentUsed += sizePercent;

            bar.Attributes["style"] = $"width:{sizePercentStr}%;";
            if (!string.IsNullOrEmpty(barData.Color)) bar.Attributes["style"] += $"background-color:{barData.Color};";
            if (this.Stacked) bar.Attributes["style"] += "float:left;";

            if (!this.Stacked && this.ShowLabels) {
                var labelSpan = new TagBuilder("label");
                labelSpan.InnerHtml.Append($"{barData.Label}: {barData.Value}");
                bar.InnerHtml.AppendHtml(labelSpan);
            }

            output.Content.AppendHtml(bar);
        }
    }

    /// <summary>
    /// Builds and appends a simple data table representation of the bar series.
    /// </summary>
    private void AddDataTable(TagHelperOutput output, BarChartDataTableTagHelper.DataTableInfo config) {
        ArgumentNullException.ThrowIfNull(config);

        var table = new TagBuilder("table");
        if (!string.IsNullOrEmpty(this.titleId)) {
            table.Attributes["aria-labelledby"] = this.titleId;
        } else {
            table.Attributes["aria-label"] = this.ValueTableAriaLabel;
        }
        var tbody = new TagBuilder("tbody");
        table.InnerHtml.AppendHtml(tbody);

        var total = this.barDataList.Sum(x => x.Value);

        foreach (var barData in this.barDataList) {
            var tr = new TagBuilder("tr");

            var th = new TagBuilder("th");
            var squareSpan = new TagBuilder("span");
            squareSpan.InnerHtml.AppendHtml("&#x25C6;");
            squareSpan.Attributes["aria-hidden"] = "true";
            if (!string.IsNullOrEmpty(barData.Color)) squareSpan.Attributes["style"] = $"color:{barData.Color};";
            th.InnerHtml.AppendHtml(squareSpan);
            th.InnerHtml.Append(barData.Label);
            tr.InnerHtml.AppendHtml(th);

            var tdValue = new TagBuilder("td");
            var valueStr = string.IsNullOrEmpty(config.ValueFormatString) ? barData.Value.ToString() : barData.Value.ToString(config.ValueFormatString);
            tdValue.InnerHtml.Append(valueStr);
            tr.InnerHtml.AppendHtml(tdValue);

            if (!string.IsNullOrEmpty(config.PercentFormatString)) {
                var tdPercent = new TagBuilder("td");
                var percent = total > 0 ? (barData.Value / total) : 0;
                var percentStr = percent.ToString(config.PercentFormatString);
                tdPercent.InnerHtml.Append(percentStr);
                tr.InnerHtml.AppendHtml(tdPercent);
            }

            tbody.InnerHtml.AppendHtml(tr);
        }

        if (!string.IsNullOrEmpty(config.TotalText)) {
            var tfoot = new TagBuilder("tfoot");
            var trTotal = new TagBuilder("tr");
            var thTotal = new TagBuilder("th");
            thTotal.InnerHtml.Append(config.TotalText);
            trTotal.InnerHtml.AppendHtml(thTotal);
            var tdTotalValue = new TagBuilder("td");
            var totalValueStr = string.IsNullOrEmpty(config.ValueFormatString) ? total.ToString() : total.ToString(config.ValueFormatString);
            tdTotalValue.InnerHtml.Append(totalValueStr);
            trTotal.InnerHtml.AppendHtml(tdTotalValue);
            if (!string.IsNullOrEmpty(config.PercentFormatString)) {
                var tdTotalPercent = new TagBuilder("td");
                trTotal.InnerHtml.AppendHtml(tdTotalPercent);
            }
            tfoot.InnerHtml.AppendHtml(trTotal);
            table.InnerHtml.AppendHtml(tfoot);
        }

        output.Content.AppendHtml(table);
    }

}