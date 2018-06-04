using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace OnlineEventsMarketingApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "DataRoute",
                url: "Data/{action}/{month}/{year}",
                defaults: new { controller = "Data", action = "DataSheet", month = UrlParameter.Optional, year = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "ReportYearRoute",
                url: "Data/{action}/{year}",
                defaults: new { controller = "Report", action = "MonthlyTagsRun"}
                );

            routes.MapRoute(
                name: "ReportsRoute",
                url: "Report/{action}/{year}/{month}",
                defaults: new { controller = "Report", action = "WeeklyTagsRun", month = UrlParameter.Optional, year = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "SettingsRoute",
                url: "Settings/{action}/{month}/{year}",
                defaults: new {controller = "Settings", action = "Tags", month = UrlParameter.Optional, year = UrlParameter.Optional }
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
