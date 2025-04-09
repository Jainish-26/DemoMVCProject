using DemoMVC.Service;
using System.Web.Mvc;


namespace DemoMVC.WebUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleService _roleService;
        private readonly UserProfileService _userProfileService;
        private readonly UserExamService _userExamService;

        public HomeController()
        {
            _roleService = new RoleService();
            _userProfileService = new UserProfileService();
            _userExamService = new UserExamService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetRoleChartData()
        {
            var data = _roleService.GetRolesWithUserCount();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserStatusChartData()
        {
            var data = _userProfileService.IsActiveUser();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetExamResultStatusSummary()
        {
            var summary = _userExamService.GetExamResultStatusSummary(); // Call your provider/service
            return Json(summary, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetExamStatusSummary()
        {
            var summary = _userExamService.GetExamStatusSummary(); // Call your provider/service
            return Json(summary, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUserExamStartTimeData()
        {
            var data = _userExamService.GetUserExamStartTimeAnalysis();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetDayWiseExamCount()
        {
            var data = _userExamService.GetDayWiseExamCount();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ExamAnalysisChart()
        {
            var data = _userExamService.ExamAnalysisChart();
            return Json(data, JsonRequestBehavior.AllowGet);
        }
    }
}
