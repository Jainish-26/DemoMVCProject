using DemoMVC.Models;
using DemoMVC.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class ActivityLogController : BaseController
    {
        // GET: ActivityLog
        private readonly ActivityLogService _activityLogService;
        private readonly UserProfileService _userProfileService;
        public ActivityLogController()
        {
            _activityLogService = new ActivityLogService();
            _userProfileService = new UserProfileService();
        }
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ACTIVITYLOG.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ACTIVITYLOG.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            var data = _activityLogService.GetAllActivityLogs();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult ActivityLogDetails(int Id)
        {
            ActivityLog model = new ActivityLog();
            var data = _activityLogService.GetActivityLogById(Id);
            if (data.UserId.HasValue)
            {
                data.UserName = data.UserProfile.UserName;
            }
            return Json(RenderPartialViewToString(this, "_ActivityLogPopUp", data), JsonRequestBehavior.AllowGet);
        }
    }
}