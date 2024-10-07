using System.ComponentModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Altairis.TagHelpers;

public static class PageModelExtensions {

    public static RedirectToPageResult AmbientRedirectToPage(this PageModel page) => page.AmbientRedirectToPage(pageName: null, pageHandler: null, routeValues: null, fragment: null);

    public static RedirectToPageResult AmbientRedirectToPage(this PageModel page, object routeValues) => page.AmbientRedirectToPage(pageName: null, pageHandler: null, routeValues, fragment: null);

    public static RedirectToPageResult AmbientRedirectToPage(this PageModel page, string pageName) => page.AmbientRedirectToPage(pageName, pageHandler: null, routeValues: null, fragment: null);

    public static RedirectToPageResult AmbientRedirectToPage(this PageModel page, string pageName, object routeValues) => page.AmbientRedirectToPage(pageName, pageHandler: null, routeValues, fragment: null);

    public static RedirectToPageResult AmbientRedirectToPage(this PageModel page, string pageName, string pageHandler) => page.AmbientRedirectToPage(pageName, pageHandler, routeValues: null, fragment: null);

    public static RedirectToPageResult AmbientRedirectToPage(this PageModel page, string pageName, string pageHandler, string fragment) => page.AmbientRedirectToPage(pageName, pageHandler, routeValues: null, fragment);

    public static RedirectToPageResult AmbientRedirectToPage(this PageModel page, string? pageName, string? pageHandler, object? routeValues, string? fragment) {
        // Copy values from routeValues object
        var newRouteValues = new Dictionary<string, object>();
        if (routeValues != null) {
            foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(routeValues)) {
                var value = descriptor.GetValue(routeValues);
                if (value != null) newRouteValues.Add(descriptor.Name, value);
            }
        }

        // Copy ambient values
        foreach (var key in page.RouteData.Values.Keys) {
            var value = page.RouteData.Values[key];
            if (!newRouteValues.ContainsKey(key) && value != null) newRouteValues[key] = value;
        }

        // Call original method
        return page.RedirectToPage(pageName, pageHandler, newRouteValues, fragment);
    }
}