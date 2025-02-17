using DemoMVC.Models;
using DemoMVC.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;
using RoleService = DemoMVC.Service.RoleService;

namespace DemoMVC.WebUi.Controllers
{
    public class UserProfileController : BaseController
    {
        private readonly UserProfileService _userProfileService;
        private readonly RoleService _rolesService;

        public UserProfileController()
        {
            _userProfileService = new UserProfileService();
            _rolesService = new RoleService();
        }
        // GET: UserProfile
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), AccessPermission.IsView))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            return View();
        }
        public ActionResult Create(int? id)
        {
            string actionPermission = "";
            if (id == null)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if ((id ?? 0) > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), actionPermission))
                return RedirectToAction("AccessDenied", "Base");

            UserProfileModel userProfile = new UserProfileModel();
            if (id > 0)
            {
                var getUser = _userProfileService.GetUserById(id.Value);

                if (getUser != null)
                {
                    userProfile.UserId = id.Value;
                    userProfile.UserName = getUser.UserName;
                    userProfile.Name = getUser.Name;
                    userProfile.Email = getUser.Email;
                    userProfile.PhoneNo = getUser.PhoneNo;
                    userProfile.MobileNo = getUser.MobileNo;
                    userProfile.IsActive = getUser.IsActive;
                    int roleId = _userProfileService.GetRoleIdByUserId(id.Value).RoleId;
                    userProfile.Role = _rolesService.GetRolesById(roleId).RoleName;
                }
            }
            BindRole(ref userProfile);
            return View(userProfile);
        }

        [HttpPost]
        public ActionResult Create(UserProfileModel userProfile)
        {
            string actionPermission = "";
            if (userProfile.UserId == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (userProfile.UserId > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.USER.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            if (userProfile.UserId > 0)
            {
                ModelState.Remove("Password");
            }
            if (ModelState.IsValid)
            {
                SaveUpdateUserProfile(userProfile);
                return RedirectToAction("Index");
            }
            else
            {
                BindRole(ref userProfile);
                return View(userProfile);
            }
        }

        public void SaveUpdateUserProfile(UserProfileModel userProfile)
        {
            int userId = SessionHelper.UserId;
            if (userProfile.UserId > 0)
            {
                UserProfile user = new UserProfile();
                user.Name = userProfile.Name;
                user.UserName = userProfile.UserName;
                user.UserId = userProfile.UserId;
                user.Email = userProfile.Email;
                user.PhoneNo = userProfile.PhoneNo;
                user.MobileNo = userProfile.MobileNo;
                user.IsActive = userProfile.IsActive;
                user.UpdatedOn = DateTime.UtcNow;
                user.UpdatedBy = userId;
                
                int id = _userProfileService.UpdateUserProfile(user);
                
                foreach (var role in Roles.GetRolesForUser(userProfile.UserName))
                {
                    Roles.RemoveUserFromRole(userProfile.UserName, role);
                }
                Roles.AddUserToRole(userProfile.UserName, userProfile.Role);
            }
            else
            {
                WebSecurity.CreateUserAndAccount(userProfile.UserName, userProfile.Password, propertyValues: 
                                                new
                                                {
                                                    Name = userProfile.Name,
                                                    Email = userProfile.Email,
                                                    PhoneNo = userProfile.PhoneNo,
                                                    MobileNo = userProfile.MobileNo,
                                                    IsActive = 1,
                                                    IsDeleted = 0,
                                                    CreatedOn = DateTime.UtcNow,
                                                    CreatedBy = userId,
                                                    UpdatedOn = DateTime.UtcNow,
                                                    UpdatedBy = userId,
                                                    //DefaultPageId = (userProfile.DefaultFormId == null ? 0 : userProfile.DefaultFormId.Value) }
                                                });
                Roles.AddUserToRole(userProfile.UserName, userProfile.Role);
            }
        }
        public UserProfileModel BindRole(ref UserProfileModel model)
        {
            var getroles = _rolesService.GetAllRoles().Where(x => x.RoleName.Trim().ToLower() != DemoMVC.Helper.Constants.RoleCode.SADMIN.ToLower());
            model._RoleList.Add(new SelectListItem() { Text = "Select Role", Value = "" });
            foreach (var item in getroles)
            {
                model._RoleList.Add(new SelectListItem() { Text = item.RoleName.Trim(), Value = item.RoleName.Trim() });
            }
            return model;
        }

        public ActionResult User_Read([DataSourceRequest] DataSourceRequest request)
        {
            var getallusers = _userProfileService.GetAllUserProfileGrid();
            return Json(getallusers.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}