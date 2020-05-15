using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.TagHelpers.DemoApp.Pages {
    public class IndexModel : PageModel {

        [BindProperty]
        public string MonthName { get; set; }

        public IEnumerable<string> DatalistItems => CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Where(s => !string.IsNullOrEmpty(s));

        public void OnGet() {
        }
    }
}
