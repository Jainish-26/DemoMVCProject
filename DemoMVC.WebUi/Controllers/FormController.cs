using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    [Authorize]
    public class FormController : BaseController
    {
        private readonly FormsService _formsService;
        public FormController()
        {
            _formsService = new FormsService();
        }
        // GET: Form
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.FORMMASTER.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.FORMMASTER.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            FormModel model = new FormModel();
            if (id.HasValue)
            {
                var formDetail = _formsService.GetFormsById(id.Value);
                if (formDetail != null)
                {
                    model.Id = id.Value;
                    model.Id = formDetail.Id;
                    model.Name = formDetail.Name;
                    model.NavigateURL = formDetail.NavigateURL;
                    model.ParentFormId = formDetail.ParentFormId;
                    model.FormAccessCode = formDetail.FormAccessCode;
                    model.DisplayOrder = formDetail.DisplayOrder;
                    model.Icon = formDetail.Icon;
                    model.IsDisplayMenu = formDetail.IsDisplayMenu;
                    model.IsActive = formDetail.IsActive ?? false;
                }
            }
            BindDropdown(ref model);
            return View(model);
        }
        [HttpPost]
        public ActionResult Create(FormModel model)
        {
            string actionPermission = "";
            if (model.Id == null)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.Id > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.FORMMASTER.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;

            if (ModelState.IsValid)
            {
                SaveUpdateForm(model);
            }
            return RedirectToAction("Index");
        }

        public FormModel SaveUpdateForm(FormModel model)
        {
            int userId = SessionHelper.UserId;

            Forms obj = new Forms();

            if (model.Id > 0)
            {
                obj = _formsService.GetFormsById(model.Id);
            }
            obj.Name = model.Name;
            obj.NavigateURL = model.NavigateURL;
            obj.ParentFormId = model.ParentFormId;
            obj.DisplayOrder = model.DisplayOrder;
            obj.Icon = model.Icon;
            obj.IsActive = model.IsActive;
            obj.IsDisplayMenu = model.IsDisplayMenu;
            model.IsDeleted = false;
            obj.Id = model.Id;
            if (obj.Id == 0)
            {
                obj.FormAccessCode = model.FormAccessCode.ToUpper();
                obj.CreatedBy = SessionHelper.UserId;
                obj.CreatedOn = DateTime.UtcNow;
                model.Id = _formsService.CreateForms(obj);
            }
            else
            {
                obj.UpdatedBy = SessionHelper.UserId;
                obj.UpdatedOn = DateTime.UtcNow;
                _formsService.UpdateForms(obj);
            }
            return model;
        }

        private void BindDropdown(ref FormModel model)
        {
            BindParentForm(ref model);
        }

        public FormModel BindParentForm(ref FormModel model)
        {
            int currentFormId = model.Id;
            var getparentform = _formsService.GetAllForms().Where(f => f.Id != currentFormId).
                Select(a => new FormModel { Id = a.Id, Name = a.Name }).OrderBy(a => a.Name);
            model._ParentFormList.Add(new SelectListItem() { Text = "Select Parent", Value = "" });
            foreach (var item in getparentform)
            {
                model._ParentFormList.Add(new SelectListItem() { Text = item.Name, Value = item.Id.ToString() });
            }
            return model;
        }
        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {

            var data = _formsService.GetAllFormsGrid();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicateFormAccessCode(string FormAccessCode, int Id)
        {
            var checkduplicate = _formsService.CheckDuplicateFormAccessCode(FormAccessCode);
            if (Id > 0)
            {
                checkduplicate = checkduplicate.Where(x => x.Id != Id).ToList();
            }
            if (checkduplicate.Count() > 0)
            {
                return Json("FormAccessCode is already exist.", JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }      
    }
}