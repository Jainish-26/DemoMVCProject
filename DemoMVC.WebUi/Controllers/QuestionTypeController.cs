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
    public class QuestionTypeController : BaseController
    {
        private readonly QuestionTypeService _questionTypeService;
        private readonly MessageService _messageService;
        public QuestionTypeController()
        {
            _questionTypeService = new QuestionTypeService();
            _messageService = new MessageService();
        }
        // GET: Subject
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;

            QuestionTypeModel model = new QuestionTypeModel();
            if (id.HasValue)
            {
                var questionTypeData = _questionTypeService.GetQuestionTypeById(id.Value);
                model.Id = id.Value;
                model.QuestionTypeCode = questionTypeData.QuestionTypeCode;
                model.Name = questionTypeData.QuestionTypeName;
                
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(QuestionTypeModel model)
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTIONTYPE.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;
            if (ModelState.IsValid)
            {
                SaveAndUpdateSubject(model);
                return RedirectToAction("Index");
            }
            else
            {
                return View(model);
            }
        }

        public QuestionTypeModel SaveAndUpdateSubject(QuestionTypeModel model)
        {
            QuestionType obj = new QuestionType();
            if (model.Id > 0)
            {
                obj = _questionTypeService.GetQuestionTypeById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.QuestionTypeId = model.Id;
            obj.QuestionTypeName = model.Name;

            if (obj.QuestionTypeId == 0)
            {
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                obj.QuestionTypeCode = model.QuestionTypeCode;
                model.Id = _questionTypeService.CreateQuestionType(obj);
            }
            else
            {
                obj.UpdatedBy = userId;
                obj.UpdatedOn = DateTime.UtcNow;
                _questionTypeService.UpdateQuestionType(obj);
            }
            return model;

        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            var data = _questionTypeService.GetAllQuestionTypesGridModels();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicateQuestionTypeCode(string QTypeCode, int Id)
        {
            var getQuestionTypeDetails = _questionTypeService.CheckDuplicateQuestionTypeCode(QTypeCode);
            if (Id > 0)
            {
                getQuestionTypeDetails = getQuestionTypeDetails.Where(a => a.QuestionTypeId != Id).ToList();
            }
            if (getQuestionTypeDetails.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(DemoMVC.Helper.Constants.MessageCode.CODEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);

            }
            else
            {
                return Json(true);
            }
        }
    }
}