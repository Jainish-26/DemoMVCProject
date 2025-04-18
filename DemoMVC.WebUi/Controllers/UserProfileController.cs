﻿using ClosedXML.Excel;
using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Helper;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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
        private readonly MessageService _messageService;
        private readonly FormsService _formsService;
        private readonly FormRoleMappingService _formRoleMapping;

        public UserProfileController()
        {
            _userProfileService = new UserProfileService();
            _rolesService = new RoleService();
            _messageService = new MessageService();
            _formRoleMapping = new FormRoleMappingService();
        }
        // GET: UserProfile
        public ActionResult Index()
        {
            //int a = 5;
            //int b = 0;
            //int ans = a / b;
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

        public ActionResult User_Read([DataSourceRequest] DataSourceRequest request,string searchTerm)
        {
            var data = _userProfileService.GetAllUserProfileGrid();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                data = data.Where(q =>
                        (q.Name != null && q.Name.ToLower().Contains(searchTerm)) ||
                        (q.UserName != null && q.UserName.ToLower().Contains(searchTerm)) ||
                        (q.Role != null && q.Role.ToLower().Contains(searchTerm)) ||
                        (q.Email != null && q.Email.ToLower().Contains(searchTerm))
                    );
            }
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicateUserEmail(string Email, int UserId)
        {
            var checkduplicate = _userProfileService.CheckDuplicateUserEmail(Email).ToList();
            if (UserId > 0)
            {
                checkduplicate = checkduplicate.Where(x => x.UserId != UserId).ToList();
            }
            if (checkduplicate.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.EMAILEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CheckDuplicateUserName(string UserName, int UserId)
        {
            var checkduplicate = _userProfileService.CheckDuplicateUserName(UserName).ToList();
            if (UserId > 0)
            {
                checkduplicate = checkduplicate.Where(x => x.UserId != UserId).ToList();
            }
            if (checkduplicate.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.USERNAMEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CasCadeForm(string Role)
        {
            List<SelectListItem> _DefaultFormList = new List<SelectListItem>();
            _DefaultFormList.Add(new SelectListItem() { Text = "Select Form", Value = "" });
            if (!string.IsNullOrWhiteSpace(Role))
            {
                int RoleId = _rolesService.GetRolesByName(Role).RoleId;
                List<int> getformId = _formRoleMapping.GetAllRoleRightsByRoleId(RoleId).Select(x => x.MenuId).ToList();
                var getparentform = _formsService.GetAllForms().Where(x => x.NavigateURL != "#" && getformId.Contains(x.Id)).Select(a => new FormModel { Id = a.Id, Name = a.Name }).OrderBy(a => a.Name);

                foreach (var item in getparentform)
                {
                    _DefaultFormList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
                }
            }
            return Json(_DefaultFormList, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ExportUserProfiles()
        {
            DataTable dt = _userProfileService.GetUserProfileDataFromEF();

            using (XLWorkbook wb = new XLWorkbook())
            {
                var ws = wb.Worksheets.Add("User Profiles");

                // Add DataTable
                ws.Cell(2, 1).InsertTable(dt);

                // Title Row
                ws.Cell("A1").Value = "User Profiles Report";

                CommonUtility.DesignExcelExport(ws, dt.Columns.Count);

                // Export
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    stream.Position = 0;
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "UserProfiles.xlsx");
                }
            }

        }
    }
}