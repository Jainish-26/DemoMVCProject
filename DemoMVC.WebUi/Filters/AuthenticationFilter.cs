using DemoMVC.Models;
using DemoMVC.WebUi.Controllers;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace DemoMVC.WebUi.Filters
{
    public class AuthenticationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Check if the action has AllowAnonymous attribute
            bool hasAllowAnonymous = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any() ||
                                    filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();

            // Only redirect to login if not anonymous and user is not authenticated
            if (!hasAllowAnonymous && !(filterContext.Controller is AccountController) && SessionHelper.UserId == 0)
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