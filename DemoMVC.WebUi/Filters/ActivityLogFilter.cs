using DemoMVC.Models;
using DemoMVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Filters
{
    public class ActivityLogFilter : ActionFilterAttribute
    {
        private readonly ActivityLogService _activityLogService;
        public ActivityLogFilter()
        {
            _activityLogService = new ActivityLogService();
        }
        string action = "";
        string controller = "";
        //string parameter = "";
        string hostName = "";
        string ipaddress = "";
        string rawurl = "";
        string browsername = "";
        string htttpmethod = "";
        string status = "";
        //ActivityLogService _activitylogService = new ActivityLogService();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContextBase currentContext = new HttpContextWrapper(HttpContext.Current);
            UrlHelper urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            System.Web.Routing.RouteData routeData = urlHelper.RouteCollection.GetRouteData(currentContext);
            action = routeData.Values["action"].ToString();
            controller = routeData.Values["controller"].ToString();
            hostName = System.Net.Dns.GetHostName();
            ipaddress = System.Net.Dns.GetHostByName(hostName).AddressList[0].ToString();
            browsername = currentContext.Request.Browser.Browser + "" + currentContext.Request.Browser.Version;
            rawurl = currentContext.Request.RawUrl;
            htttpmethod = currentContext.ApplicationInstance.Request.HttpMethod;
            status = currentContext.ApplicationInstance.Response.Status;
            //string fullmessage = "Method:" + htttpmethod + " , " + "Status:" + status;
            string fullmessage = "AiRecruitment.Controller." + controller + "Controller";

            ActivityLog log = new ActivityLog();
            log.ActionName = action;
            log.ControllerName = controller;
            log.FullMessage = fullmessage;
            log.PageUrl = rawurl;
            log.BrowserName = browsername;
            log.IPAddress = ipaddress;
            if (SessionHelper.UserId > 0)
                log.UserId = Convert.ToInt32(SessionHelper.UserId);
            log.LogDate = DateTime.UtcNow;
            _activityLogService.CreateActivityLog(log);
        }
    }
}