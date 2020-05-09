using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Altairis.Services.DateProvider;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {
    [HtmlTargetElement("calendar")]
    public class CalendarTagHelper : TagHelper {
        private readonly CultureInfo culture = CultureInfo.CurrentCulture;
        private readonly IDateProvider dateProvider;
        private DateTime realDateBegin;
        private DateTime realDateEnd;

        // Constructor

        public CalendarTagHelper(IDateProvider dateProvider = null) {
            this.dateProvider = dateProvider ?? new LocalDateProvider();
        }

        // Configuration properties

        public DateTime DateBegin { get; set; }

        public DateTime DateEnd { get; set; }

        public DayNameStyle DayNameStyle { get; set; } = DayNameStyle.Full;

        public IEnumerable<CalendarEvent> Events { get; set; } = new HashSet<CalendarEvent>();

        public IEnumerable<DateTime> SelectedDays { get; set; } = new HashSet<DateTime>();

        public string GeneralDateFormat { get; set; } = "d.";

        public string NewMonthDateFormat { get; set; } = "d. MMMM";

        // Main process method

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            // Get real begin and end date
            this.realDateBegin = this.FindWeekBegin(this.DateBegin);
            this.realDateEnd = this.FindWeekEnd(this.DateEnd);

            // Setup output options
            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.AppendLine();

            // Add header with day names
            if (this.DayNameStyle != DayNameStyle.None) {
                output.Content.AppendHtml(this.GenerateHeader());
                output.Content.AppendLine();
            }

            // Add weeks
            var d = this.realDateBegin;
            while (d < this.realDateEnd) {
                output.Content.AppendLine();
                output.Content.AppendHtml(this.GenerateWeek(d));
                d = d.AddDays(7);
            }
        }

        // Helper HTML content generating methods

        private IHtmlContent GenerateHeader() {
            var headerBuilder = new TagBuilder("header");
            for (var i = 0; i < 7; i++) {
                var d = this.realDateBegin.AddDays(i);
                var dayName = this.DayNameStyle switch
                {
                    DayNameStyle.Shortest => this.culture.DateTimeFormat.GetShortestDayName(d.DayOfWeek),
                    DayNameStyle.Abbreviated => this.culture.DateTimeFormat.GetAbbreviatedDayName(d.DayOfWeek),
                    _ => this.culture.DateTimeFormat.GetDayName(d.DayOfWeek)
                };

                var dayBuilder = new TagBuilder("div");
                dayBuilder.InnerHtml.Append(dayName);
                headerBuilder.InnerHtml.AppendLine();
                headerBuilder.InnerHtml.AppendHtml(dayBuilder);
            }
            headerBuilder.InnerHtml.AppendLine();
            return headerBuilder;
        }

        private IHtmlContent GenerateWeek(DateTime firstDay) {
            var weekBuilder = new TagBuilder("section");
            weekBuilder.Attributes.Add("class", "week");
            weekBuilder.Attributes.Add("data-week-number", this.culture.Calendar.GetWeekOfYear(firstDay, CalendarWeekRule.FirstDay, this.culture.DateTimeFormat.FirstDayOfWeek).ToString());


            for (var i = 0; i < 7; i++) {
                var d = firstDay.AddDays(i);
                weekBuilder.InnerHtml.AppendLine();
                weekBuilder.InnerHtml.AppendHtml(this.GenerateDay(d));
            }

            return weekBuilder;
        }

        private TagBuilder GenerateDay(DateTime day) {
            // Add day container with CSS clases
            var dayBuilder = new TagBuilder("article");
            if (day < this.DateBegin || day > this.DateEnd) dayBuilder.AddCssClass("extra");
            if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday) dayBuilder.AddCssClass("weekend");
            if (day.Date == this.dateProvider.Today) dayBuilder.AddCssClass("today");
            if (this.SelectedDays.Any(x => x.Date == day.Date)) dayBuilder.AddCssClass("selected");

            // Add date
            var dayHeaderBuilder = new TagBuilder("header");
            dayHeaderBuilder.InnerHtml.AppendHtml(day.ToString(day.Day == 1 ? this.NewMonthDateFormat : this.GeneralDateFormat));
            dayBuilder.InnerHtml.AppendLine();
            dayBuilder.InnerHtml.AppendHtml(dayHeaderBuilder);
            dayBuilder.InnerHtml.AppendLine();

            // Add day events
            dayBuilder.InnerHtml.AppendHtml(this.GenerateDayEvents(day));
            return dayBuilder;
        }

        private IHtmlContent GenerateDayEvents(DateTime day) {
            static bool isBetween(DateTime value, DateTime begin, DateTime end) {
                return value.Date >= begin.Date && value.Date <= end.Date;
            }

            var dayEvents = this.Events
                .Where(e => isBetween(day, e.DateBegin, e.DateEnd.HasValue ? e.DateEnd.Value : e.DateBegin))
                .OrderBy(e => e.DateBegin).ThenBy(e => e.IsFullDay);

            if (!dayEvents.Any()) return null;

            var eventListBuilder = new TagBuilder("ul");
            foreach (var e in dayEvents) {
                var eventBuilder = new TagBuilder("li");
                if (!string.IsNullOrEmpty(e.Description)) eventBuilder.Attributes.Add("title", e.Description);
                if (!string.IsNullOrEmpty(e.CssClass)) eventBuilder.AddCssClass(e.CssClass);

                if (!string.IsNullOrEmpty(e.BackgroundColor) || !string.IsNullOrEmpty(e.BackgroundColor)) {
                    var style = string.Empty;
                    if (!string.IsNullOrEmpty(e.BackgroundColor)) style += $"background-color:{e.BackgroundColor};";
                    if (!string.IsNullOrEmpty(e.ForegroundColor)) style += $"color:{e.ForegroundColor};";
                    eventBuilder.Attributes.Add("style", style);
                }

                if (e.IsFullDay) {
                    eventBuilder.AddCssClass("full-day");
                } else {
                    if (day.Date == e.DateBegin.Date) {
                        var timeBeginBuilder = new TagBuilder("time");
                        timeBeginBuilder.Attributes.Add("datetime", e.DateBegin.ToString("s"));
                        timeBeginBuilder.AddCssClass("begin");
                        timeBeginBuilder.InnerHtml.AppendHtml(e.DateBegin.ToShortTimeString());
                        eventBuilder.InnerHtml.AppendHtml(timeBeginBuilder);
                    }
                    if (e.DateEnd.HasValue) {
                        if (day.Date == e.DateEnd.Value.Date) {
                            var timeBeginBuilder = new TagBuilder("time");
                            timeBeginBuilder.Attributes.Add("datetime", e.DateEnd.Value.ToString("s"));
                            timeBeginBuilder.AddCssClass("end");
                            timeBeginBuilder.InnerHtml.AppendHtml(e.DateEnd.Value.ToShortTimeString());
                            eventBuilder.InnerHtml.AppendHtml(timeBeginBuilder);
                        } else if (day.Date != e.DateBegin.Date) {
                            var timeBeginBuilder = new TagBuilder("span");
                            timeBeginBuilder.AddCssClass("middle");
                            eventBuilder.InnerHtml.AppendHtml(timeBeginBuilder);
                        }
                    }
                }

                var liContent = new TagBuilder(string.IsNullOrEmpty(e.Href) ? "span" : "a");
                if (!string.IsNullOrEmpty(e.Href)) liContent.Attributes.Add("href", e.Href);
                liContent.InnerHtml.AppendHtml(e.Name);
                eventBuilder.InnerHtml.AppendHtml(liContent);
                eventListBuilder.InnerHtml.AppendHtml(eventBuilder);
            }

            return eventListBuilder;
        }

        // Helper methods

        private DateTime FindWeekBegin(DateTime value) {
            while (value.DayOfWeek != this.culture.DateTimeFormat.FirstDayOfWeek) value = value.AddDays(-1);
            return value;
        }

        private DateTime FindWeekEnd(DateTime value) => this.FindWeekBegin(value).AddDays(6);

    }

    public class CalendarEvent {

        public bool IsFullDay { get; set; }

        public DateTime DateBegin { get; set; }

        public DateTime? DateEnd { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Href { get; set; }

        public string CssClass { get; set; }

        public string ForegroundColor { get; set; }

        public string BackgroundColor { get; set; }

    }

    public enum DayNameStyle {
        None = 0,
        Full = 1,
        Abbreviated = 2,
        Shortest = 3
    }

}
