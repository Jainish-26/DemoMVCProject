using System.Web.Mvc;
using DemoMVC.Service;


namespace DemoMVC.WebUi.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleService _roleService;
        private readonly UserProfileService _userProfileService;

        public HomeController()
        {
            _roleService = new RoleService();
            _userProfileService = new UserProfileService();
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
    }
}
