using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Altairis.TagHelpers.DemoApp.Pages {
    public class CheckBoxListModel : PageModel {

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel {

            public ICollection<string> SelectedValues { get; set; } = new HashSet<string>();

        }

        public string Message { get; set; }

        public IEnumerable<SelectListItem> ListValues => new List<SelectListItem>(new[] {
            new SelectListItem("Item 1", "1", this.Input.SelectedValues.Contains("1")),
            new SelectListItem("Item 2", "2", this.Input.SelectedValues.Contains("2")) { Disabled = true },
            new SelectListItem("Item 3", "3", this.Input.SelectedValues.Contains("3")),
        });

        public void OnPost() {
            this.Message = "Selected item IDs: " + string.Join(", ", this.Input.SelectedValues);
        }
    }
}