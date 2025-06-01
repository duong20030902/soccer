using Microsoft.AspNetCore.Mvc.Rendering;

namespace Soccer.Font_end.Helpers
{
    public static class ActiveMenu
    {
        public static string IsActive(this IHtmlHelper htmlHelper, string controller, params string[] actions)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();
            var currentAction = routeData.Values["action"]?.ToString();
            return (controller == currentController && actions.Contains(currentAction)) ? "active" : string.Empty;
        }

        public static string IsActiveParent(this IHtmlHelper htmlHelper, params string[] controllers)
        {
            var routeData = htmlHelper.ViewContext.RouteData;
            var currentController = routeData.Values["controller"]?.ToString();
            return controllers.Contains(currentController) ? "active" : string.Empty;
        }
    }
}
