using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using static DemoMVC.Models.SessionHelper;
using System.Web.Security;
using WebMatrix.WebData;
using DemoMVC.WebUi.Filters;

namespace DemoMVC.WebUi.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login() {
            LoginModel model = new LoginModel();
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid && WebSecurity.Login(model.UserName.Trim(), model.Password, persistCookie: model.RememberMe))
            {
                var userId = WebSecurity.GetUserId(model.UserName);
                SessionHelper.UserId = userId;
                SessionHelper.IsAdmin = true;
                SessionHelper.UserName = model.UserName;
                //SessionHelper.RoleName = Roles.GetRolesForUser(model.UserName).FirstOrDefault();
                //SessionHelper.RoleId = _roleService.GetRolesByName(SessionHelper.RoleName).RoleId;
                //SessionHelper.RoleCode = _roleService.GetRolesById(SessionHelper.RoleId).RoleCode;
                //SessionHelper.UserEmailId = _userProfileService.GetUserById(SessionHelper.UserId).Email;
                //SessionHelper.Name = _userProfileService.GetUserById(SessionHelper.UserId).Name;
                //SessionHelper.OrganizationIds = new List<int>();
                //if (SessionHelper.RoleCode != Constants.RoleCode.SADMIN && SessionHelper.RoleCode != Constants.RoleCode.ADMIN)
                //{
                //    SessionHelper.OrganizationIds = _userOrganizationService.GetOrgIdByUserId(SessionHelper.UserId).ToList();
                //    SessionHelper.IsAdmin = false;
                //}
                //if (!string.IsNullOrEmpty(model.TimeZone))
                //{
                //    TimeZoneInfo tzi = CommonUtility.OlsonTimeZoneToTimeZoneInfo(model.TimeZone);
                //    if (tzi != null)
                //    {
                //        SessionHelper.DefaultTimeZone = tzi.StandardName;
                //    }
                //}
                //Session["Menu"] = _formRoleService.GetMenu(userId);

                //if (returnUrl == null)
                //    return RedirectToAction("Index", "Home");
                //else
                //    return RedirectToLocal(returnUrl);

                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("Password", "Invalid Username or Password");
                return View(model);
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