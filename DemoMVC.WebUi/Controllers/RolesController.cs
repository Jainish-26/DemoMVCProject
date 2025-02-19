using DemoMVC.Models;
using DemoMVC.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemoMVC.Helper;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


namespace DemoMVC.WebUi.Controllers
{
    [Authorize]
    public class RolesController : BaseController
    {
        private readonly RoleService _rolesService;

        public RolesController()
        {
            _rolesService = new RoleService();
        }
        // GET: Roles
        public ActionResult Index()
        {

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            RolesModel model = new RolesModel();

            if (id.HasValue)
            {
                var rolesDetails = _rolesService.GetRolesById(id.Value);
                if (rolesDetails != null)
                {
                    model.Id = id.Value;
                    model.RoleCode = rolesDetails.RoleCode;
                    model.Name = rolesDetails.RoleName;
                    model.IsActive = rolesDetails.IsActive;
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(RolesModel model)
        {
            string actionPermission = "";
            if (model.Id == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.Id > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdateRoles(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }
        public RolesModel SaveUpdateRoles(RolesModel model)
        {
            webpages_Roles obj = new webpages_Roles();
            if (model.Id > 0)
            {
                obj = _rolesService.GetRolesById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.RoleId = model.Id;
            obj.IsActive = model.IsActive;
            obj.RoleName = model.Name;
            obj.UpdatedBy = userId;
            obj.UpdatedOn = DateTime.UtcNow;
            if (obj.RoleId == 0)
            {
                obj.RoleCode = model.RoleCode;
                model.Id = _rolesService.CreateRoles(obj);
            }
            else
            {
                _rolesService.UpdateRoles(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            var data = _rolesService.GetAllRolesGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        //public JsonResult CheckDuplicateRoleCode(string RoleCode, int Id)
        //{
        //    var getRoleDetails = _rolesService.CheckDuplicateRoleCode(RoleCode);
        //    if (Id > 0)
        //    {
        //        getRoleDetails = getRoleDetails.Where(a => a.RoleId != Id).ToList();
        //    }
        //    if (getRoleDetails.Count() > 0)
        //    {
        //        var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
        //        return Json(message, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json(true, JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult DeleteRole(int? id)
        {
            string actionPermission = AccessPermission.IsDelete;

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.ROLES.ToString(), actionPermission))
            {
                return Json(new { success = false, message = "Access Denied" });
            }

            int userId = SessionHelper.UserId;
            var data = _rolesService.GetRolesById(id.Value);
            TempData["RoleName"] = data.RoleName;

            try
            {
                bool ans = _rolesService.DeleteRole(id.Value, userId);

                if (ans)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return  Json(new { success = false, message = "Active User Assigned To This Role" });
                }
                
            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}