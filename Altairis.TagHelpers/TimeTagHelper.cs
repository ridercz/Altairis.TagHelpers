using System;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;

namespace Altairis.TagHelpers {
    [HtmlTargetElement("time", Attributes = "value")]
    public class TimeTagHelper : TagHelper {
        private readonly TimeTagHelperOptions options;

        public TimeTagHelper(IOptions<TimeTagHelperOptions> optionsAccessor = null) {
            this.options = optionsAccessor?.Value ?? new TimeTagHelperOptions();
        }

        public DateTime? Value { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            if (!this.Value.HasValue) {
                // Value is not specified
                output.Content.SetContent(this.options.NullDateFormatter());
            }
            else {
                // Value is specified
                var dateValue = this.Value.Value;

                // Add datetime attribute if not already present
                if (context.AllAttributes["datetime"] == null) {
                    output.Attributes.Add("datetime", dateValue.ToString("s"));
                }

                // Add title attribute if not already present
                if (context.AllAttributes["title"] == null) {
                    output.Attributes.Add("title", this.options.TooltipDateFormatter(dateValue));
                }

                // Set content if not present
                if (output.Content.IsEmptyOrWhiteSpace) {
                    if (dateValue.Date == DateTime.Today) {
                        output.Content.SetContent(this.options.TodayDateFormatter(dateValue));
                    }
                    else if (dateValue.Date == DateTime.Today.AddDays(-1)) {
                        output.Content.SetContent(this.options.YesterdayDateFormatter(dateValue));
                    }
                    else if (dateValue.Date == DateTime.Today.AddDays(1)) {
                        output.Content.SetContent(this.options.TomorrowDateFormatter(dateValue));
                    }
                    else {
                        output.Content.SetContent(this.options.GeneralDateFormatter(dateValue));
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