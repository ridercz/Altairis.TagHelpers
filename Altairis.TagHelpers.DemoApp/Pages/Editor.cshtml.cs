using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.TagHelpers.DemoApp.Pages {
    public class EditorModel : PageModel {

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel {

            [Display(Name = "Name")]
            public string? FullName { get; set; }

            [Display(Name = "E-mail"), EmailAddress]
            public string? EmailAddress { get; set; }

            [Display(Name = "Comment"), DataType(DataType.MultilineText)]
            public string? Comment { get; set; }

        }

    }
}
