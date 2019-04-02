using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Altairis.TagHelpers {

    [HtmlTargetElement(ParentTag = null, Attributes = "include-roles")]
    [HtmlTargetElement(ParentTag = null, Attributes = "exclude-roles")]
    public class RolesTagHelper : TagHelper {
        private readonly IHttpContextAccessor contextAccessor;

        public RolesTagHelper(IHttpContextAccessor contextAccessor) {
            this.contextAccessor = contextAccessor;
        }

        public string ExcludeRoles { get; set; }

        public string IncludeRoles { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output) {
            base.Process(context, output);

            // Process excluded roles
            if (!string.IsNullOrWhiteSpace(this.ExcludeRoles) && MatchRoles(this.contextAccessor.HttpContext.User, this.ExcludeRoles)) {
                output.SuppressOutput();
                return;
            }

            // Process included roles
            if (!string.IsNullOrWhiteSpace(this.IncludeRoles) && !MatchRoles(this.contextAccessor.HttpContext.User, this.IncludeRoles)) {
                output.SuppressOutput();
                return;
            }
        }

        private static bool MatchRoles(ClaimsPrincipal principal, string roleString) {
            if (principal == null) throw new ArgumentNullException(nameof(principal));
            if (roleString == null) throw new ArgumentNullException(nameof(roleString));
            if (string.IsNullOrWhiteSpace(roleString)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(roleString));

            var roles = roleString.Split(',').Select(x => x.Trim());
            foreach (var role in roles) {
                if (role == "?" && !principal.Identity.IsAuthenticated) return true;
                if (role == "*" && principal.Identity.IsAuthenticated) return true;
                if (principal.IsInRole(role)) return true;
            }
            return false;
        }

    }
}
