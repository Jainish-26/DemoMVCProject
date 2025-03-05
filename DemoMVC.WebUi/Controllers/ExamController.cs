using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class ExamController : BaseController
    {
        private readonly ExamService _examService;

        public ExamController()
        {
            _examService = new ExamService();
        }
        public ActionResult Index()
        {
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.EXAM.ToString(), actionPermission))
                return RedirectToAction("AccessDenied", "Base");

            int userId = SessionHelper.UserId;
            ExamModel model = new ExamModel();

            if (id.HasValue)
            {
                var exam = _examService.GetById(id.Value);

                if (exam != null)
                {
                    model.ExamId = id.Value;
                    model.ExamName = exam.ExamName;
                    model.TotalMarks = exam.TotalMarks;
                    model.PassingMarks = exam.PassingMarks;
                    model.StartTime = exam.StartTime;
                    model.EndTime = exam.EndTime;
                    model.IsActive = exam.IsActive;
                    model.ExamCode = exam.ExamCode;
                    model.DurationMin = exam.DurationMin;
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ExamModel exam)
        {
            string actionPermission = "";
            if (exam.ExamId ==0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (exam.ExamId  > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.EXAM.ToString(), actionPermission))
                return RedirectToAction("AccessDenied", "Base");

            int userId = SessionHelper.UserId;

            if (ModelState.IsValid)
            {
                SaveAndUpdateExam(exam);
                return RedirectToAction("Index");
            }
            else
            {
                return View(exam);
            }
        }

        public ExamModel SaveAndUpdateExam(ExamModel exam)
        {
            Exams obj = new Exams();
            int userId = SessionHelper.UserId;

            if (exam.ExamId > 0)
            {
                obj = _examService.GetById(exam.ExamId);
            }

            obj.ExamId = exam.ExamId;
            obj.ExamName = exam.ExamName;
            obj.ExamCode = exam.ExamCode;
            obj.TotalMarks = exam.TotalMarks;
            obj.PassingMarks = exam.PassingMarks;
            obj.IsActive = exam.IsActive;
            obj.DurationMin = exam.DurationMin;
            obj.StartTime = exam.StartTime;
            obj.EndTime = exam.EndTime;
            obj.ExamStatus = "OnGoing";

            if (exam.ExamId == 0)
            {
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                obj.ExamId = _examService.CreateExam(obj);
            }
            else
            {
                obj.UpdatedBy = userId;
                obj.UpdatedOn = DateTime.UtcNow;
                _examService.UpdateExam(obj);
            }

            return exam;
        }
        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            var data = _examService.GetExamsGridModels();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}