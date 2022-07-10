using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.TagHelpers.DemoApp.Pages {
    public class IndexModel : PageModel {

    [BindProperty]
    public string? MonthName { get; set; }

    public IEnumerable<string> DatalistItems => CultureInfo.CurrentCulture.DateTimeFormat.MonthNames.Where(s => !string.IsNullOrEmpty(s));

}
