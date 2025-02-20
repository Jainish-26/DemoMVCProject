using DemoMVC.Models;
using DemoMVC.Service;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class FormRoleMappingController : Controller
    {
        private readonly FormRoleMappingService _formRoleMapping;
        private readonly RoleService _roleService;
        public FormRoleMappingController()
        {
            _roleService = new RoleService();
            _formRoleMapping = new FormRoleMappingService();
        }
        // GET: FormRoleMapping
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ViewPermission(int Id = 0)
        {

            string role = _roleService.GetRolesById(SessionHelper.RoleId).RoleName;
            if (role != "sadmin")
            {
                return RedirectToAction("AccessDenied", "Base");

            }

            FormRoleMapping model = new FormRoleMapping();
            if (Id > 0)
            {
                model.RoleId = Id;
                model.RoleName = _roleService.GetRolesById(Id).RoleName;
            }
            return View(model);
        }

        public JsonResult UpdatePermission(IEnumerable<FormRoleMapping> rolerights)
        {
            int CreatedBy = SessionHelper.UserId;
            int UpdatedBy = SessionHelper.UserId;
            var result = _formRoleMapping.UpdateRoleRights(rolerights, CreatedBy, UpdatedBy);
            if (result)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult FormRoleMapping_Read([DataSourceRequest] DataSourceRequest request, int RoleID)
        {
            var getrolerights = _formRoleMapping.GetAllRoleRightsById(RoleID).AsQueryable();

            return Json(getrolerights.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}