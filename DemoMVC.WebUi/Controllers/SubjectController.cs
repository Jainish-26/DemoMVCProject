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
    public class SubjectController : BaseController
    {
        private readonly SubjectService _subjectService;
        private readonly MessageService _messageService;
        public SubjectController()
        {
            _subjectService = new SubjectService();
            _messageService = new MessageService();
        }
        // GET: Subject
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }
            int userId = SessionHelper.UserId;

            SubjectModel model = new SubjectModel();
            if (id.HasValue)
            {
                var subjectdata = _subjectService.GetSubjectById(id.Value);
                model.Id = id.Value;
                model.SubjectCode = subjectdata.SubjectCode;
                model.Name = subjectdata.SubjectName;
                model.IsActive = subjectdata.IsActive;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(SubjectModel model)
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.SUBJECT.ToString(), actionPermission))
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

        public SubjectModel SaveAndUpdateSubject(SubjectModel model)
        {
            Subject obj = new Subject();
            if (model.Id > 0)
            {
                obj = _subjectService.GetSubjectById(model.Id);
            }
            int userId = SessionHelper.UserId;
            obj.SubjectId = model.Id;
            obj.IsActive = model.IsActive;
            obj.SubjectName = model.Name;

            if (obj.SubjectId == 0)
            {
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                obj.SubjectCode = model.SubjectCode;
                model.Id = _subjectService.CreateSubjects(obj);
            }
            else
            {
                obj.UpdatedBy = userId;
                obj.UpdatedOn = DateTime.UtcNow;
                _subjectService.UpdateSubject(obj);
            }
            return model;

        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            var data = _subjectService.GetSubjectGridModels();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckDuplicateSubjectCode(string subjectCode, int Id)
        {
            var getSubjectDetails = _subjectService.CheckDuplicateSubjectCode(subjectCode);
            if (Id > 0)
            {
                getSubjectDetails = getSubjectDetails.Where(a => a.SubjectId != Id).ToList();
            }
            if (getSubjectDetails.Count() > 0)
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