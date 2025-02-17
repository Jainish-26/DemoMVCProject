using DemoMVC.Models;
using DemoMVC.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class ErrorLogController : BaseController
    {
        private readonly ErrorLogService _errorLogService;
        public ErrorLogController()
        {
            _errorLogService = new ErrorLogService();
        }
        public ActionResult Index(int? Id)
        {
            ErrorLog model;
            if ((Id ?? 0) > 0)
            {
                model = _errorLogService.GetErrorLogById(Id.Value);
            }
            else
                model = new ErrorLog();

            string statusCode = string.Empty;
            if (Request.QueryString.ToString() != null && Request.QueryString.ToString().Length > 0)
            {
                if (Request.QueryString["msg"] != null && Request.QueryString["msg"].ToString().Trim().Length > 0)
                {
                    if (Request.QueryString["msg"] == "404" || Request.QueryString["msg"] == "400" || Request.QueryString["msg"] == "403")
                    {
                        string StatusCode = Request.QueryString["msg"].ToString();
                        statusCode = StatusCode;
                        ViewData["StatusCode"] = statusCode;
                        HttpRequestBase request = HttpContext.Request;
                        ErrorLog errorLog = new ErrorLog()
                        {
                            CreatedBy = SessionHelper.UserId,
                            CreatedOn = DateTime.UtcNow,
                            Description = statusCode,
                            Source = statusCode,
                            RecordDate = DateTime.UtcNow,
                            PageName = statusCode,
                            PageParameter = "",
                            LineNumber = 0,
                            MethodName = "",
                            IpAddress = request.UserHostAddress,
                            UserAgent = request.UserAgent.ToString().Trim().Replace("'", "''"),
                            BrowserNameVersion = request.Browser.Browser + " " + request.Browser.Version,
                            UserId = HttpContext.Session["userid"] != null ? Convert.ToInt32(HttpContext.Session["userid"]) : 0,
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
                        int errorLogId = _errorLogService.CreateErrorLog(errorLog);
                    }
                }
            }
            return View("~/Views/Shared/Error.cshtml", model);
        }

        public ActionResult ErrorLogList()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ERRORLOG.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }
        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ERRORLOG.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            var data = _errorLogService.GetAllErrorLogs();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ErrorDetails(int Id)
        {
            ErrorLogGridModel model = new ErrorLogGridModel();
            var errorLogDetails = _errorLogService.GetErrorLogById(Id);
            return Json(RenderPartialViewToString(this, "_ErrorLogPopUp", errorLogDetails), JsonRequestBehavior.AllowGet);
        }
    }
}