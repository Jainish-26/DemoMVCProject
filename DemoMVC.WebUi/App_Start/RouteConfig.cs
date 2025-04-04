﻿using System.Web.Mvc;
using System.Web.Routing;

namespace DemoMVC.WebUi
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
            //routes.MapRoute(
            //    name: "ExamLink",
            //    url: "UserExam/UserExamView/{ExamId}/{examCode}/{uniqueId}",
            //    defaults: new { controller = "UserExam", action = "UserExamView" }
            //);
        }
    }
}
