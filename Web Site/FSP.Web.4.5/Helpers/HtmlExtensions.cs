using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace FSP.Web.Helpers
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString CheckBoxList(this HtmlHelper helper, string name, IEnumerable<SelectListItem> items)
        {
            var output = new StringBuilder();

            foreach (var item in items)
            {
                output.Append(@"<label class=""checkbox"">");
                output.Append(@"<input type=""checkbox"" name=""");
                output.Append(name);
                output.Append("\" value=\"");
                output.Append(item.Value);
                output.Append("\"");
                if (item.Selected)
                    output.Append(@" checked=""checked""");
                output.Append(" />");
                output.Append(item.Text);
                output.Append("</label>");
            }


            return MvcHtmlString.Create(output.ToString());
        }

        public static MvcHtmlString HtmlMenu(this HtmlHelper helper)
        {
            var sb = new StringBuilder();
            var applicationPath = HttpContext.Current.Request.ApplicationPath;

#if(DEBUG)
            applicationPath = "http://" + HttpContext.Current.Request.Url.Authority;
            //applicationPath = "http://" + HttpContext.Current.Request.Url.Authority + "/octafsp/";
#endif

            var currentControllerName = (string)helper.ViewContext.RouteData.Values["controller"];
            var currentActionName = (string)helper.ViewContext.RouteData.Values["action"];
            
            //Home
            sb.Append($"<li class='{(currentControllerName == "Home" ? "active" : "")}'><a href='{applicationPath}/Home/Index'>Home</a></li>");

            //Map
            sb.Append($"<li class='{(currentControllerName == "Map" && currentActionName == "Index" ? "active" : "")}'><a href='{applicationPath}/Map/Index'>Mapping</a></li>");

            //Dispatch
            sb.Append($"<li class='{(currentControllerName == "Dispatch" && currentActionName == "Index" ? "active" : "")}'><a href='{applicationPath}/Dispatch/Index'>Dispatch</a></li>");

            //Communication
            var comCustomClass = "";
            if ((currentControllerName == "AlertMessages" && currentActionName == "Index") ||
                (currentControllerName == "Incident" && currentActionName == "Index"))
            {
                comCustomClass = "active";
            }


            sb.Append($"<li class='dropdown {comCustomClass}'>");
            sb.Append("<a class='dropdown-toggle' data-toggle='dropdown' href='#'>");
            sb.Append("Communication <span class='caret'></span>");
            sb.Append("</a>");
            sb.Append("<ul class='dropdown-menu'>");
            sb.Append($"<li><a href='{applicationPath}/Dispatch/Index'>Dispatch</a></li>");
            sb.Append($"<li><a href='{applicationPath}/AlertMessages/Index'>Driver Messaging</a></li>");
            sb.Append($"<li><a href='{applicationPath}/Incident/Index'>Incident</a></li>");
            sb.Append("</ul>");
            sb.Append("</li>");

            //Monitoring
            var monCustomClass = "";
            if ((currentControllerName == "AlertMessages" && currentActionName == "Alerts") ||
                (currentControllerName == "AssistAdmin" && currentActionName == "Create") ||
                (currentControllerName == "CHPInformation" && currentActionName == "Index"))
            {
                monCustomClass = "active";
            }


            sb.Append($"<li class='dropdown {monCustomClass}'>");
            sb.Append("<a id='monitoringTab' class='dropdown-toggle' data-toggle='dropdown' href='#'>");
            sb.Append("Monitoring <span class='caret'></span>");
            sb.Append("</a>");
            sb.Append("<ul class='dropdown-menu'>");
            sb.Append($"<li><a id='alertMenuItem' href='{applicationPath}/AlertMessages/Alerts'>Alarms</a></li>");
            sb.Append($"<li><a href='{applicationPath}/AssistAdmin/Create'>Assist Entry</a></li>");
            sb.Append($"<li><a href='{applicationPath}/CHPInformation/Index'>CHP</a></li>");
            sb.Append($"<li><a href='{applicationPath}/Admin/Index'>Database</a></li>");

            var dashboardUrl = ConfigurationManager.AppSettings["DashboardUrl"];

            if (!string.IsNullOrEmpty(dashboardUrl))
                sb.Append($"<li><a href='{dashboardUrl}' target='_blank'>Dashboard</a></li>");

            sb.Append($"<li><a href='{applicationPath}/Incident/Index'>Incidents</a></li>");
            sb.Append($"<li><a href='{applicationPath}/AdminArea/Schedule/IndexNew'>Scheduling</a></li>");
            sb.Append($"<li><a href='{applicationPath}/Truck/Grid'>Truck List</a></li>");
            sb.Append("</ul>");
            sb.Append("</li>");

            //Analysis & System
            var sysCustomClass = "";
            //if ((currentControllerName == "AlertMessages" && currentActionName == "Index") ||
            //    (currentControllerName == "Incident" && currentActionName == "Index"))
            //{
            //    sysCustomClass = "active";
            //}
            sb.Append($"<li class='dropdown {sysCustomClass}'>");
            sb.Append("<a class='dropdown-toggle' data-toggle='dropdown' href='#'>");
            sb.Append("Analysis & System <span class='caret'></span>");
            sb.Append("</a>");
            sb.Append("<ul class='dropdown-menu'>");
            sb.Append($"<li><a href='{applicationPath}/Admin/Index'>Database</a></li>");
            if (!string.IsNullOrEmpty(dashboardUrl))
                sb.Append($"<li><a href='{dashboardUrl}' target='_blank'>Dashboard</a></li>");
            sb.Append($"<li><a href='{applicationPath}/EmailReport/Index'>Email Reports</a></li>");
            sb.Append($"<li><a href='{applicationPath}/Report/Index'>Reports</a></li>");
            sb.Append($"<li><a href='{applicationPath}/Survey/Index'>Survey</a></li>");            
            sb.Append("</ul>");
            sb.Append("</li>");

            return MvcHtmlString.Create(sb.ToString());
        }
    }
}