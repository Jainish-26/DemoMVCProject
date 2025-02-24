using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class CommonLookupController : BaseController
    {
        string formCode = AuthorizeFormAccess.FormAccessCode.COMMONLOOKUP.ToString();
        private readonly CommonLookupService _commonLookupService;
        private readonly MessageService _messageService;
        public CommonLookupController()
        {
            _commonLookupService = new CommonLookupService();
            _messageService = new MessageService();
        }
        // GET: CommonLookup
        public ActionResult Index()
        {
            if (!CheckPermission(formCode, AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.COMMONLOOKUP.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            CommonLookupModel model = new CommonLookupModel();
            if (id.HasValue)
            {
                var lookupDetails = _commonLookupService.GetCommonLookupById(id.Value);
                if (lookupDetails != null)
                {
                    model.Id = lookupDetails.Id;
                    model.Type = lookupDetails.Type;
                    model.Code = lookupDetails.Code;
                    model.Name = lookupDetails.Name;
                    model.DisplayOrder = lookupDetails.DisplayOrder;
                    model.IsActive = lookupDetails.IsActive;
                    model.Comment = lookupDetails.Comment;
                    model.BadgeCode = lookupDetails.BadgeCode;
                }
            }
            return View(model);
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult Create(CommonLookupModel model)
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.COMMONLOOKUP.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveUpdatedCommonLookup(model);
            }
            return RedirectToAction("Index");
        }

        public CommonLookupModel SaveUpdatedCommonLookup(CommonLookupModel model)
        {
            int userId = SessionHelper.UserId;
            CommonLookup obj = new CommonLookup();
            if (model.Id > 0)
            {
                obj = _commonLookupService.GetCommonLookupById(model.Id);
            }
            obj.Id = model.Id;
            obj.Type = model.Type;
            obj.Code = model.Code;
            obj.Name = model.Name;
            obj.DisplayOrder = model.DisplayOrder;
            obj.IsActive = model.IsActive;
            obj.Comment = model.Comment;
            obj.BadgeCode = model.BadgeCode;
            if (obj.Id == 0)
            {
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                model.Id = _commonLookupService.CreateCommonLookup(obj);
            }
            else
            {
                obj.ModifiedBy = userId;
                obj.ModifiedOn = DateTime.UtcNow;
                _commonLookupService.UpdateCommonLookup(obj);
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            if (!CheckPermission(formCode, AccessPermission.IsView))
                return AccessDenied();
            var commonLookUpData = _commonLookupService.GetAllLookup();
            return Json(commonLookUpData.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetDropDownData([DataSourceRequest] DataSourceRequest request, string type)
        {
            List<CommonLookup> data = new List<CommonLookup>();
            if (!string.IsNullOrEmpty(type))
            {
                data = _commonLookupService.GetLookupByType(type);
            }
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicateCode(string Code, int Id, string Type)
        {
            var checkduplicate = _commonLookupService.CheckDuplicateLookupCode(Code);
            if (Id > 0)
            {
                checkduplicate = checkduplicate.Where(x => x.Id != Id && x.Type == Type).ToList();
            }
            else
            {
                checkduplicate = checkduplicate.Where(x => x.Type == Type).ToList();
            }
            if (checkduplicate.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }



        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CommonLookUp_Create([DataSourceRequest] DataSourceRequest request, CommonLookup commonLookup)
        //{
        //    CustomModelValidation(commonLookup);
        //    if (commonLookup != null && ModelState.IsValid)
        //    {
        //        commonLookup.CreatedBy = SessionHelper.UserId;
        //        commonLookup.CreatedOn = DateTime.UtcNow;
        //        commonLookup.Code = commonLookup.Code.ToUpper();
        //        _commonLookupService.CreateCommonLookup(commonLookup);
        //    }

        //    return Json(new[] { commonLookup }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CommonLookUp_Update([DataSourceRequest] DataSourceRequest request, CommonLookup commonLookup)
        //{
        //    CustomModelValidation(commonLookup);
        //    if (commonLookup != null && ModelState.IsValid)
        //    {
        //        var commonLookupDetails = _commonLookupService.GetCommonLookupById(commonLookup.Id);
        //        commonLookupDetails.Code = commonLookup.Code.ToUpper();
        //        commonLookupDetails.Comment = commonLookup.Comment;
        //        commonLookupDetails.DisplayOrder = commonLookup.DisplayOrder;
        //        commonLookupDetails.Type = commonLookup.Type;
        //        commonLookupDetails.Name = commonLookup.Name;
        //        commonLookupDetails.IsActive = commonLookup.IsActive;
        //        commonLookupDetails.ModifiedBy = SessionHelper.UserId;
        //        commonLookupDetails.ModifiedOn = DateTime.UtcNow;
        //        _commonLookupService.UpdateCommonLookup(commonLookupDetails);
        //    }

        //    return Json(new[] { commonLookup }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult CommonLookup_Destroy([DataSourceRequest] DataSourceRequest request, CommonLookup commonLookup)
        //{
        //    if (commonLookup != null)
        //    {
        //        _commonLookupService.UpdateCommonLookup(commonLookup);
        //    }

        //    return Json(new[] { commonLookup }.ToDataSourceResult(request, ModelState));
        //}


        //private bool CustomModelValidation(CommonLookup model)
        //{
        //    bool isValid = true;
        //    var commonlookupDetails = _commonLookupService.CheckDuplicateLookupCode(model.Code);
        //    if (model.Id > 0)
        //    {
        //        commonlookupDetails = commonlookupDetails.Where(a => a.Id != model.Id).ToList();
        //    }
        //    if (commonlookupDetails.Count() > 0)
        //    {
        //        var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
        //        ModelState.AddModelError("Code", message);
        //        isValid = false;
        //    }
        //    return isValid;
        //}
    }
}