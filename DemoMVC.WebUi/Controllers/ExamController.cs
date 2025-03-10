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
    public class ExamController : BaseController
    {
        private readonly ExamService _examService;
        private readonly QuestionService _questionService;
        private readonly ExamQuestionsService _examQuestionsService;

        public ExamController()
        {
            _examService = new ExamService();
            _questionService = new QuestionService();
            _examQuestionsService = new ExamQuestionsService();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(int? id)
        {
            ViewBag.ExamId = id;
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

                ViewBag.TotalMarks = (id > 0) ? _examQuestionsService.GetTotalMarks(id) : 0;

            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ExamQuestionModel model)
        {
            try
            { 
                string actionPermission = "";
                if (model.Exam.ExamId == 0)
                {
                    actionPermission = AccessPermission.IsAdd;
                }
                else if (model.Exam.ExamId > 0)
                {
                    actionPermission = AccessPermission.IsEdit;
                }

                if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.EXAM.ToString(), actionPermission))
                {
                    return Json(new { success = false, message = "Access Denied!" });
                }
                int userId = SessionHelper.UserId;

                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        success = false,
                        errors = ModelState.ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    )
                    });
                }
                if (model.Exam.DurationMin < 10 && model.Exam.TotalMarks < 20)
                {
                    return Json(new { success = false, message = "Minimum duration is 10 minutes and total marks should be at least 20." });
                }
                else if (model.Exam.DurationMin < 10)
                {
                    return Json(new { success = false, message = "Minimum Duration 10 minutes required." });
                }
                else if (model.Exam.TotalMarks < 20)
                {
                    return Json(new { success = false, message = "Minimum Total 20 marks required." });
                }

                SaveExamWithQuestions(model);
                return Json(new { success = true, message = "Exam saved successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }


        public ExamQuestionModel SaveExamWithQuestions(ExamQuestionModel model)
        {
            Exams obj = new Exams();
            int userId = SessionHelper.UserId;

            if (model.Exam.ExamId > 0)
            {
                obj = _examService.GetById(model.Exam.ExamId);
            }

            obj.ExamId = model.Exam.ExamId;
            obj.ExamName = model.Exam.ExamName;
            obj.ExamCode = model.Exam.ExamCode;
            obj.TotalMarks = model.Exam.TotalMarks;
            obj.PassingMarks = model.Exam.PassingMarks;
            obj.IsActive = model.Exam.IsActive;
            obj.DurationMin = model.Exam.DurationMin;
            obj.StartTime = model.Exam.StartTime;
            obj.EndTime = model.Exam.EndTime;
            obj.ExamStatus = "OnGoing";

            if (model.Exam.ExamId == 0) 
            {
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                obj.ExamId = _examService.CreateExam(obj);

             
                foreach (var question in model.SelectedQuestions)
                {
                    var examQuestion = new ExamQuestions
                    {
                        ExamId = obj.ExamId,
                        QuestionId = question.QuestionId,
                        Marks = question.Marks
                    };
                    _examQuestionsService.CraeteExamQuestion(examQuestion);
                }
            }
            else 
            {
                obj.UpdatedBy = userId;
                obj.UpdatedOn = DateTime.UtcNow;
                _examService.UpdateExam(obj);


                var existingExamQuestions = _examQuestionsService.GetExamQuestionsById(obj.ExamId).ToList();

                // Selected question IDs for quick lookup
                var selectedQuestionIds = new HashSet<int>(model.SelectedQuestions.Select(q => q.QuestionId));

                foreach (var question in model.SelectedQuestions)
                {
                    var existingExamQuestion = existingExamQuestions.FirstOrDefault(q => q.QuestionId == question.QuestionId);

                    if (existingExamQuestion == null)
                    {
                        // Create new exam question if not exists
                        var newExamQuestion = new ExamQuestions
                        {
                            ExamId = obj.ExamId,
                            QuestionId = question.QuestionId,
                            Marks = question.Marks
                        };
                        _examQuestionsService.CraeteExamQuestion(newExamQuestion);
                    }
                    else if (existingExamQuestion.Marks != question.Marks)
                    {
                        
                        existingExamQuestion.Marks = question.Marks;
                        _examQuestionsService.UpdateExamQuestion(existingExamQuestion);
                    }
                }

                // Remove questions that are no longer selected
                foreach (var existingExamQuestion in existingExamQuestions)
                {
                    if (!selectedQuestionIds.Contains(existingExamQuestion.QuestionId))
                    {
                        _examQuestionsService.DeleteExamQuestion(existingExamQuestion.QuestionId, obj.ExamId);
                    }
                }

            }

            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            var data = _examService.GetExamsGridModels();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetGridExamQuestionData([DataSourceRequest] DataSourceRequest request,int? ExamId)
        {
            var data = _examQuestionsService.GetExamQuestionsGrid(ExamId);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetGridUnassignedQuestionData([DataSourceRequest] DataSourceRequest request, int? ExamId)
        {
            var data = _examQuestionsService.GetUnassignedQuestions(ExamId);
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
    }
}