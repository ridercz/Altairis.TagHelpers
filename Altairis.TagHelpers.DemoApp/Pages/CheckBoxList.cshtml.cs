using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Altairis.TagHelpers.DemoApp.Pages {
    public class CheckBoxListModel : PageModel {

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public class InputModel {

            public ICollection<int> CheckboxSelectedValues { get; set; }

            public string RadioSelectedValue { get; set; }

        }

        public string Message { get; set; }

        public IEnumerable<SelectListItem> CheckboxListItems => new List<SelectListItem>(new[] {
            new SelectListItem("Item 1", "1"),
            new SelectListItem("Item 2", "2"),
            new SelectListItem("Item 3", "3"),
        });

        public IEnumerable<SelectListItem> RadioListItems => new List<SelectListItem>(new[] {
            new SelectListItem("Item A", "A"),
            new SelectListItem("Item B", "B"),
            new SelectListItem("Item C", "C"),
        });

        public void OnPost() {
            this.Message = $"Selected checkbox item IDs: {string.Join(", ", this.Input.CheckboxSelectedValues)}, selected radio item ID: {this.Input.RadioSelectedValue}";
        }
    }
}