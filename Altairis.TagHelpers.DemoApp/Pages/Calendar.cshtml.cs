using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.TagHelpers.DemoApp.Pages {
    public class CalendarModel : PageModel {
        public ICollection<CalendarEvent> Events { get; set; } = new List<CalendarEvent>();

        public ICollection<DateTime> SelectedDays { get; set; } = new List<DateTime>();

        public void OnGet() {
            // Full day events
            this.Events.Add(new CalendarEvent { DateBegin = new DateTime(2020, 4, 3), IsFullDay = true, Name = "Full Day Event", CssClass = "red", Description = "Toto je popis" });
            this.Events.Add(new CalendarEvent { DateBegin = new DateTime(2020, 4, 10), IsFullDay = true, Name = "Full Day Event #1", CssClass = "blue" });
            this.Events.Add(new CalendarEvent { DateBegin = new DateTime(2020, 4, 10), IsFullDay = true, Name = "Full Day Event #2" });
            this.Events.Add(new CalendarEvent { DateBegin = new DateTime(2020, 4, 12), DateEnd = new DateTime(2020, 4, 15), IsFullDay = true, Name = "Multi day full day" });

            // Non-FDE
            this.Events.Add(new CalendarEvent { DateBegin = new DateTime(2020, 4, 7, 8, 0, 0), DateEnd = new DateTime(2020, 4, 7, 10, 0, 0), Name = "Short event", Href = "https://www.altair.blog" });
            this.Events.Add(new CalendarEvent { DateBegin = new DateTime(2020, 4, 8, 8, 0, 0), DateEnd = new DateTime(2020, 4, 9, 16, 0, 0), Name = "Multiday event", BackgroundColor = "#060", ForegroundColor = "#ff0" });
            this.Events.Add(new CalendarEvent { DateBegin = new DateTime(2020, 4, 21, 8, 0, 0), DateEnd = new DateTime(2020, 4, 24, 16, 0, 0), Name = "Long multiday event" });

            // Selected days
            this.SelectedDays.Add(new DateTime(2020, 4, 10));
            this.SelectedDays.Add(new DateTime(2020, 4, 11));
            this.SelectedDays.Add(new DateTime(2020, 4, 12));
        }

    }
}
