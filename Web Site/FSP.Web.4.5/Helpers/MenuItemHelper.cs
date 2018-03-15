using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FSP.Web.Helpers
{
    public static class MenuItemHelper
    {
        public static MvcHtmlString DatabaseTab(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            var applicationPath = HttpContext.Current.Request.ApplicationPath;

            if (HttpContext.Current.User.Identity.IsAuthenticated)
                if (HttpContext.Current.User.IsInRole("Contractor") == false)
                {
                    sb.Append("<li><a href='" + applicationPath + "/Admin'>Database</a>");
                    sb.Append("</li>");
                }

            return MvcHtmlString.Create(sb.ToString());
        }

        public static MvcHtmlString MenuItem(this HtmlHelper helper, string linkText, string actionName,
            string controllerName,
            string area, string id)
        {
            var currentControllerName = (string) helper.ViewContext.RouteData.Values["controller"];
            var currentActionName = (string) helper.ViewContext.RouteData.Values["action"];

            var builder = new TagBuilder("li");

            // Add selected class
            if (currentControllerName.Equals(controllerName, StringComparison.CurrentCultureIgnoreCase) &&
                currentActionName.Equals(actionName, StringComparison.CurrentCultureIgnoreCase))
                builder.AddCssClass("active");


            // Add link
            if (!string.IsNullOrEmpty(area))
                builder.InnerHtml = helper
                    .ActionLink(linkText, actionName, controllerName, "", "", "", new {area}, new {id}).ToHtmlString();
            else
                builder.InnerHtml = helper
                    .ActionLink(linkText, actionName, controllerName, "", "", "", new {area = ""}, new {id})
                    .ToHtmlString();


            // Render Tag Builder
            return MvcHtmlString.Create(builder.ToString(TagRenderMode.Normal));
        }
        
    }
}