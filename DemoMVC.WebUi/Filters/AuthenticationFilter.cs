using DemoMVC.Models;
using DemoMVC.WebUi.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoMVC.WebUi.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Controller controller = filterContext.Controller as Controller;
            if (controller != null && !(controller is AccountController)
               && SessionHelper.UserId == 0)
            {
                filterContext.Result =
                       new RedirectToRouteResult(
                           new RouteValueDictionary {
                           { "controller", "Account" },
                           { "action", "Login" },
                            { "returnUrl", filterContext.HttpContext.Request.RawUrl }
                       });
            }
            base.OnActionExecuting(filterContext);
        }
    }
}