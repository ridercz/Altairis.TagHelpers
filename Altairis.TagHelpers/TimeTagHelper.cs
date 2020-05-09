using System;
using Altairis.Services.DateProvider;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Altairis.TagHelpers {
    [HtmlTargetElement("time", Attributes = "value")]
    public class TimeTagHelper : TagHelper {
        private readonly TimeTagHelperOptions options;
        private readonly IDateProvider dateProvider;

        public TimeTagHelper(IOptions<TimeTagHelperOptions> optionsAccessor = null, IDateProvider dateProvider = null) {
            this.options = optionsAccessor?.Value ?? new TimeTagHelperOptions();
            this.dateProvider = dateProvider ?? new LocalDateProvider();
        }

        public DateTime? Value { get; set; }

        public string TooltipFormat { get; set; }

        public string GeneralFormat { get; set; }

        public string TodayFormat { get; set; }

        public string YesterdayFormat { get; set; }

        public string TomorrowFormat { get; set; }

        public string NullText { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            if (!this.Value.HasValue) {
                // Value is not specified
                output.Content.SetContent(this.NullText ?? this.options.NullDateFormatter());
            } else {
                string formatValue(DateTime value, string fixedFormat, Func<DateTime, string> configuredFormat) {
                    return string.IsNullOrEmpty(fixedFormat) ? configuredFormat(value) : string.Format(fixedFormat, value);
                }

                // Value is specified
                var dateValue = this.Value.Value;

                // Add datetime attribute if not already present
                if (context.AllAttributes["datetime"] == null) {
                    output.Attributes.Add("datetime", dateValue.ToString("s"));
                }

                // Add title attribute if not already present
                if (context.AllAttributes["title"] == null) {
                    output.Attributes.Add("title", formatValue(dateValue, this.TooltipFormat, this.options.TooltipDateFormatter));
                }

                // Set content if not present
                if (output.Content.IsEmptyOrWhiteSpace) {
                    if (dateValue.Date == this.dateProvider.Today) {
                        output.Content.SetContent(formatValue(dateValue, this.TodayFormat, this.options.TodayDateFormatter));
                    } else if (dateValue.Date == this.dateProvider.Today.AddDays(-1)) {
                        output.Content.SetContent(formatValue(dateValue, this.YesterdayFormat, this.options.YesterdayDateFormatter));
                    } else if (dateValue.Date == this.dateProvider.Today.AddDays(1)) {
                        output.Content.SetContent(formatValue(dateValue, this.TomorrowFormat, this.options.TomorrowDateFormatter));
                    } else {
                        output.Content.SetContent(formatValue(dateValue, this.GeneralFormat, this.options.GeneralDateFormatter));
                    }
                }
            }
        }

    }

    public class TimeTagHelperOptions {

        public Func<string> NullDateFormatter { get; set; } = () => "n/a";

        public Func<DateTime, string> TooltipDateFormatter { get; set; } = d => string.Format("{0:D}, {0:T}", d);

        public Func<DateTime, string> GeneralDateFormatter { get; set; } = d => string.Format("{0:d}, {0:t}", d);

        public Func<DateTime, string> TodayDateFormatter { get; set; } = d => string.Format("today, {0:t}", d);

        public Func<DateTime, string> YesterdayDateFormatter { get; set; } = d => string.Format("yesterday, {0:t}", d);

        public Func<DateTime, string> TomorrowDateFormatter { get; set; } = d => string.Format("tomorrow, {0:t}", d);

    }

}