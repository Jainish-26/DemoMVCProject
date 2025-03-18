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
    public class ExamController : BaseController
    {
        private readonly ExamService _examService;
        private readonly QuestionService _questionService;
        private readonly ExamQuestionsService _examQuestionsService;
        private readonly CommonLookupService _commonLookupService;
        private readonly AnswerService _answerService;
        private readonly MessageService _messageService;

        public ExamController()
        {
            _examService = new ExamService();
            _questionService = new QuestionService();
            _examQuestionsService = new ExamQuestionsService();
            _commonLookupService = new CommonLookupService();
            _answerService = new AnswerService();
            _messageService = new MessageService();
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
                    model.ExamStatus = exam.ExamStatus;
                }

                ViewBag.TotalMarks = (id > 0) ? _examQuestionsService.GetTotalMarks(id) : 0;

            }
            BindStatus(ref model);
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
                    ExamModel exam = model.Exam;
                    BindStatus(ref exam);
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
                    ExamModel exam = model.Exam;
                    BindStatus(ref exam);
                    return Json(new { success = false, message = "Minimum duration is 10 minutes and total marks should be at least 20." });
                }
                else if (model.Exam.DurationMin < 10)
                {
                    ExamModel exam = model.Exam;
                    BindStatus(ref exam);
                    return Json(new { success = false, message = "Minimum Duration 10 minutes required." });
                }
                else if (model.Exam.TotalMarks < 20)
                {
                    ExamModel exam = model.Exam;
                    BindStatus(ref exam);
                    return Json(new { success = false, message = "Minimum Total 20 marks required." });
                }

                SaveExamWithQuestions(model);
                return Json(new { success = true, message = "Exam saved successfully!" });
            }
            catch (Exception ex)
            {
                ExamModel exam = model.Exam;
                BindStatus(ref exam);
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
            obj.ExamStatus = model.Exam.ExamStatus;

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


                var selectedQuestionIds = new HashSet<int>(model.SelectedQuestions.Select(q => q.QuestionId));

                foreach (var question in model.SelectedQuestions)
                {
                    var existingExamQuestion = existingExamQuestions.FirstOrDefault(q => q.QuestionId == question.QuestionId);

                    if (existingExamQuestion == null)
                    {
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
        public ActionResult GetGridExamQuestionData([DataSourceRequest] DataSourceRequest request, int? ExamId)
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

        private ExamModel BindStatus(ref ExamModel exam)
        {
            var data = _commonLookupService.GetLookupByType(LookupType.ExamStatus);
            exam._statusList.Add(new SelectListItem() { Text = " Status", Value = "" });
            foreach (var item in data)
            {
                exam._statusList.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
            }
            return exam;
        }

        public JsonResult CheckDuplicateExamCode(string ExamCode, int ExamId)
        {
            var getExamDetails = _examService.CheckDuplicateExamCode(ExamCode);
            if (ExamId > 0)
            {
                getExamDetails = getExamDetails.Where(a => a.ExamId != ExamId).ToList();
            }
            if (getExamDetails.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.CODEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true);
            }
        }
        public ActionResult GetExamQuestionDetails(int ExamId)
        {
            var questions = _questionService.GetQuestionsByExamId(ExamId)
            .Select(q => new QuestionAndAnswerModel
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                IsActive = q.IsActive,
                QuestionImage = q.QuestionImage,
                Difficulty = q.Difficulty,
                QuestionType = q.QuestionType.QuestionTypeName,
                Subject = q.Subject.SubjectName,
                Marks = q.Marks,
                Answers = _answerService.GetByQuestionId(q.QuestionId)
                    .Select(ans => new AnswerViewModel
                    {
                        AnswerText = ans.AnswerText,
                        IsCorrect = ans.IsCorrect
                    }).ToList()
            }).ToList();

            var examDetails = _examService.GetById(ExamId);

            ExamModel exam = new ExamModel
            {
                ExamCode = examDetails.ExamCode,
                ExamName = examDetails.ExamName,
                PassingMarks = examDetails.PassingMarks,
                ExamStatus = examDetails.ExamStatus,
                DurationMin = examDetails.DurationMin,
                TotalMarks = examDetails.TotalMarks,
            };

            ExamQuestionViewModel eqvm = new ExamQuestionViewModel
            {
                Exam = exam,
                Questions = questions,
            };

            return PartialView("_ExamQuestionsPartial", eqvm);
        }
    }
}