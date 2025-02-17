using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Filters
{
    public class ExceptionHandlingFilter : HandleErrorAttribute
    {
        private readonly ErrorLogService _errorLogService;
        public ExceptionHandlingFilter()
        {
            _errorLogService = new ErrorLogService();
        }
        public override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;
            filterContext.ExceptionHandled = true;
            HttpRequestBase request = filterContext.HttpContext.Request;
            string controllerName = string.Empty;
            string actionName = string.Empty;
            if (filterContext.RouteData != null && filterContext.RouteData.Values.Count > 0)
            {
                if (filterContext.RouteData.Values["Controller"] != null)
                {
                    controllerName = filterContext.RouteData.Values["Controller"].ToString();
                }
                if (filterContext.RouteData.Values["Action"] != null)
                {
                    actionName = filterContext.RouteData.Values["Action"].ToString();
                }
            }

            string description = filterContext.Exception.Message != null ? filterContext.Exception.Message.Replace("'", "''") : string.Empty;
            string source = filterContext.Exception.StackTrace != null ? filterContext.Exception.StackTrace.Replace("'", "''") : string.Empty;

            string pageName = request.ApplicationPath.ToString().Replace("/", "") + "/" + controllerName + "/" + actionName;
            string pageParameter = request.Url != null ? request.Url.Query : string.Empty;
            int lineNumber = 0;
            const string lineSearch = ":line ";
            var index = filterContext.Exception.StackTrace.LastIndexOf(lineSearch);
            if (index != -1)
            {
                string lineNumberText = filterContext.Exception.StackTrace.Substring(index + lineSearch.Length);
                Match match = Regex.Match(lineNumberText, "^[0-9]+");
                if (match.Value != "")
                {
                    lineNumber = Convert.ToInt32(match.Value);
                }
            }

            bool _isAjaxRequest = filterContext.HttpContext.Request.IsAjaxRequest();
            ErrorLog errorLog = new ErrorLog()
            {
                CreatedBy = SessionHelper.UserId,
                CreatedOn = DateTime.UtcNow,
                Description = description,
                Source = source,
                RecordDate = DateTime.UtcNow,
                PageName = pageName,
                PageParameter = pageParameter,
                LineNumber = lineNumber,
                MethodName = controllerName + "/" + actionName,
                IpAddress = request.UserHostAddress,
                UserAgent = request.UserAgent.ToString().Trim().Replace("'", "''"),
                BrowserNameVersion = request.Browser.Browser + " " + request.Browser.Version,
                UserId = filterContext.HttpContext.Session["userid"] != null ? Convert.ToInt32(filterContext.HttpContext.Session["userid"]) : 0,
                BrowserCapabilities = "[Type = " + request.Browser.Type + "];" +
                                             "[Name = " + request.Browser.Browser + "];" +
                                             "[Version = " + request.Browser.Version + "];" +
                                             "[Major Version = " + request.Browser.MajorVersion + "];" +
                                             "[Minor Version = " + request.Browser.MinorVersion + "];" +
                                             "[Platform = " + request.Browser.Platform + "];" +
                                             "[Is Beta = " + request.Browser.Beta + "];" +
                                             "[Is Crawler = " + request.Browser.Crawler + "];" +
                                             "[Is AOL = " + request.Browser.AOL + "];" +
                                             "[Is Win16 = " + request.Browser.Win16 + "];" +
                                             "[Is Win32 = " + request.Browser.Win32 + "];" +
                                             "[Supports Frames = " + request.Browser.Frames + "];" +
                                             "[Supports Tables = " + request.Browser.Tables + "];" +
                                             "[Supports Cookies = " + request.Browser.Cookies + "];" +
                                             "[Supports VBScript = " + request.Browser.VBScript + "];" +
                                             "[Supports JavaScript = " + request.Browser.EcmaScriptVersion.ToString() + "];" +
                                             "[Supports Java Applets = " + request.Browser.JavaApplets + "];" +
                                             "[Supports ActiveX Controls = " + request.Browser.ActiveXControls + "];" +
                                             "[Supports JavaScript Version = " + request.Browser["JavaScriptVersion"] + "];"
            };

            if (filterContext.Controller != null && filterContext.Controller.ViewBag.FormAccessCode != null)
            {
                errorLog.LastFormAccessCode = filterContext.Controller.ViewBag.FormAccessCode;
            }
            else if (!string.IsNullOrEmpty(controllerName))
            {
                errorLog.LastFormAccessCode = FormUtility.getFormNavigation(controllerName);
            }
            int errorLogId = _errorLogService.CreateErrorLog(errorLog);

            filterContext.HttpContext.Response.Clear();
            ContentResult contentResult = new ContentResult();
            contentResult.Content = "{ \"error\": " + filterContext.Exception.Message + "}";
            filterContext.Result = contentResult;

            if (_isAjaxRequest && filterContext.Exception != null)
            {
                errorLog.Id = errorLogId;

                filterContext.HttpContext.Response.StatusCode = (int)System.Net.HttpStatusCode.InternalServerError;
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        filterContext.Exception.Message,
                        filterContext.Exception.StackTrace,
                        response = errorLog
                    }
                };
                filterContext.ExceptionHandled = true;
            }
            else
            {
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);
                if (controllerName != "Error")
                {
                    var routeValues = new System.Web.Routing.RouteValueDictionary()
                                                {
                                                    { "controller", "ErrorLog" },
                                                    { "action", "Index" },
                                                    { "id",errorLogId},
                                                };
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                    filterContext.ExceptionHandled = true;
                }
            }
        }
    }
}