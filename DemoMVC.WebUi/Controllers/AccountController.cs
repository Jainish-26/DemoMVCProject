using DemoMVC.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using DemoMVC.WebUi.Filters;
using DemoMVC.Service;

namespace DemoMVC.WebUi.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        private readonly FormRoleMappingService _formRoleService;
        private readonly RoleService _roleService;
        private readonly FormsService _formsService;
        private readonly UserProfileService _userProfileService;

        public AccountController()
        {
            _formRoleService = new FormRoleMappingService();
            _roleService = new RoleService();
            _formsService = new FormsService();
            _userProfileService = new UserProfileService();
        }
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl) {
            LoginModel model = new LoginModel();
            if (SessionHelper.UserId > 0)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model,string returnUrl)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName.Trim(), model.Password, persistCookie: model.RememberMe))
            {
                var userId = WebSecurity.GetUserId(model.UserName);
                SessionHelper.UserId = userId;
                SessionHelper.IsAdmin = true;
                SessionHelper.UserName = model.UserName;
                SessionHelper.RoleName = Roles.GetRolesForUser(model.UserName).FirstOrDefault();
                SessionHelper.RoleId = _roleService.GetRolesByName(SessionHelper.RoleName).RoleId;
                SessionHelper.RoleCode = _roleService.GetRolesById(SessionHelper.RoleId).RoleCode;
                SessionHelper.UserEmailId = _userProfileService.GetUserById(SessionHelper.UserId).Email;
                SessionHelper.Name = _userProfileService.GetUserById(SessionHelper.UserId).Name;
                //if (!string.IsNullOrEmpty(model.TimeZone))
                //{
                //    TimeZoneInfo tzi = CommonUtility.OlsonTimeZoneToTimeZoneInfo(model.TimeZone);
                //    if (tzi != null)
                //    {
                //        SessionHelper.DefaultTimeZone = tzi.StandardName;
                //    }
                //}
                Session["Menu"] = _formRoleService.GetMenu(userId);

                if (returnUrl == null)
                    return RedirectToAction("Index", "Home");
                else
                    return RedirectToLocal(returnUrl);
            }
            else
            {
                ModelState.AddModelError("Password", "Invalid Username or Password");
                return View(model);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult LogOff()
        {
            WebSecurity.Logout();
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

    }
}