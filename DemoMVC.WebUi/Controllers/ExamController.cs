﻿using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Web.Management;
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
        private readonly UserExamService _userExamService;
        private readonly UserAnswerService _userAnswerService;
        private readonly QuestionMediaService _questionMediaService;
        private readonly SubjectService _subjectService;

        public ExamController()
        {
            _examService = new ExamService();
            _questionService = new QuestionService();
            _examQuestionsService = new ExamQuestionsService();
            _commonLookupService = new CommonLookupService();
            _answerService = new AnswerService();
            _messageService = new MessageService();
            _userExamService = new UserExamService();
            _userAnswerService = new UserAnswerService();
            _questionMediaService = new QuestionMediaService();
            _subjectService = new SubjectService();
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
            obj.TotalMarks = model.Exam.TotalMarks;
            obj.PassingMarks = model.Exam.PassingMarks;
            obj.IsActive = model.Exam.IsActive;
            obj.DurationMin = model.Exam.DurationMin;
            obj.StartTime = model.Exam.StartTime;
            obj.EndTime = model.Exam.EndTime;
            obj.ExamStatus = model.Exam.ExamStatus;

            if (model.Exam.ExamId == 0)
            {
                obj.ExamCode = model.Exam.ExamCode;
                obj.CreatedBy = userId;
                obj.CreatedOn = DateTime.UtcNow;
                obj.ExamId = _examService.CreateExam(obj);
                var createQuestions = new List<ExamQuestions>();

                foreach (var question in model.SelectedQuestions)
                {
                    var examQuestion = new ExamQuestions
                    {
                        ExamId = obj.ExamId,
                        QuestionId = question.QuestionId,
                        Marks = question.Marks
                    };

                    createQuestions.Add(examQuestion);
                }
                if (createQuestions.Any())
                {
                    _examQuestionsService.CraeteExamQuestion(createQuestions); 
                }
            }
            else
            {
                obj.UpdatedBy = userId;
                obj.UpdatedOn = DateTime.UtcNow;
                _examService.UpdateExam(obj);

                var existingExamQuestions = _examQuestionsService.GetExamQuestionsById(obj.ExamId).ToList();

                var createQuestions = new List<ExamQuestions>();
                var updateQuestions = new List<ExamQuestions>();
                var deleteQuestionIds = new HashSet<int>();

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
                        createQuestions.Add(newExamQuestion);
                    }
                    else if (existingExamQuestion.Marks != question.Marks)
                    {
                        existingExamQuestion.Marks = question.Marks;
                        updateQuestions.Add(existingExamQuestion);
                    }
                }

                foreach (var existingExamQuestion in existingExamQuestions)
                {
                    if (!selectedQuestionIds.Contains(existingExamQuestion.QuestionId))
                    {
                        deleteQuestionIds.Add(existingExamQuestion.QuestionId);
                    }
                }

                if (createQuestions.Any())
                {
                    _examQuestionsService.CraeteExamQuestion(createQuestions);
                }

                if (updateQuestions.Any())
                {
                    _examQuestionsService.UpdateExamQuestion(updateQuestions); 
                }

                if (deleteQuestionIds.Any())
                {
                    _examQuestionsService.DeleteExamQuestion(deleteQuestionIds, obj.ExamId);
                }
            }
            return model;
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request,string searchTerm)
        {
            var data = _examService.GetExamsGridModels();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                data = data.Where(q =>
                    (q.ExamName != null && q.ExamName.ToLower().Contains(searchTerm)) ||
                    (q.ExamCode != null && q.ExamCode.ToLower().Contains(searchTerm)) ||
                    (q.ExamStatus != null && q.ExamStatus.ToLower().Contains(searchTerm))  ||
                    (q.TotalMarks.ToString().Contains(searchTerm)) ||
                    (q.DurationMin.ToString().Contains(searchTerm))
                );
            }
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
        public ActionResult GetExamQuestionDetails(int? ExamId,int? userExamId)
        {
            int id;
            if (userExamId.HasValue)
            {
                var userExamDetails = _userExamService.GetByUserExamId(userExamId.Value);
                id = userExamId != null ? userExamDetails.ExamId : ExamId.Value;
            }
            else
            {
                id = ExamId.Value;
            }

            var questions = _questionService.GetQuestionsByExamId(id)
            .Select(q =>
            {
                var mediaList = _questionMediaService.GetMediaByQuestionId(q.QuestionId);

                return new QuestionAndAnswerModel
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    IsActive = q.IsActive,
                    QuestionImages = mediaList.Any() && mediaList!=null ? mediaList : null,
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
                };
            }).ToList();
            

            var examDetails = _examService.GetById(id);

            ExamModel exam = new ExamModel
            {
                ExamCode = examDetails.ExamCode,
                ExamName = examDetails.ExamName,
                PassingMarks = examDetails.PassingMarks,
                ExamStatus = examDetails.ExamStatus,
                DurationMin = examDetails.DurationMin,
                TotalMarks = examDetails.TotalMarks,
            };

            if (userExamId.HasValue)
            {
                var userAnswers = _userAnswerService.GetAllAnswerByUser(userExamId.Value).Select(a => new UserAnswerModel
                {
                    Answer = a.AnswerText,
                    QuestionId = a.QuestionId,
                    UserAnswerId = a.UserAnswerId,
                    UserExamId = a.UserExamId,
                    ObtainedMarks = a.ObtainedMarks
                }).OrderBy(q => q.QuestionId).ToList();

                UserExamCheckingModel uecm = new UserExamCheckingModel
                {
                    Exam = exam,
                    Questions = questions,
                    UserExamId = userExamId.Value,
                    UserAnswers = userAnswers
                };

                return PartialView("_ExamQuestionsPartial", uecm);
            }

            UserExamCheckingModel model = new UserExamCheckingModel
            {
                Exam = exam,
                Questions = questions,
            };

            return PartialView("_ExamQuestionsPartial", model);
        }

        public ActionResult GetEmailPartial(int ExamId)
        {
            var model = new UserExamModel { ExamId = ExamId };
            return PartialView("_EmailInputPartial", model);
        }

        public JsonResult CheckUserInExam(int ExamId)
        {
            bool Ans = _userExamService.CountUserExamByExamId(ExamId);
            if (!Ans)
            {
                return Json(new { success = false, message = "Exam Already Assigned To Candidate" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = true, examId = ExamId }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult CreateDuplicateExam(int ExamId)
        {
            try
            {
                var examDetails = _examService.GetById(ExamId);
                var examQuestionDetails = _examQuestionsService.GetExamQuestionsById(ExamId);
                int countOfCopy = _examService.CountCopyOfExams(examDetails.ExamCode + "COPY");

                Exams exam = new Exams();
                List<ExamQuestions> examQuestions = new List<ExamQuestions>();

                exam.ExamName = countOfCopy==0 ? examDetails.ExamName+"Copy" :examDetails.ExamName+"Copy"+countOfCopy;
                exam.ExamCode = countOfCopy == 0 ? examDetails.ExamCode + "COPY" : examDetails.ExamCode + "COPY" + countOfCopy;
                exam.TotalMarks = examDetails.TotalMarks;
                exam.PassingMarks = examDetails.PassingMarks;
                exam.IsActive = examDetails.IsActive;
                exam.DurationMin = examDetails.DurationMin;
                exam.StartTime = examDetails.StartTime;
                exam.EndTime = examDetails.EndTime;
                exam.ExamStatus = Constants.ExamStatus.DRAFT;
                exam.CreatedBy = SessionHelper.UserId;
                exam.CreatedOn = DateTime.UtcNow;

                int examId = _examService.CreateExam(exam);

                foreach(var q in examQuestionDetails)
                {
                    var examQ = new ExamQuestions
                    {
                        ExamId = examId,
                        QuestionId = q.QuestionId,
                        Marks = q.Marks
                        
                    };

                    examQuestions.Add(examQ);
                }

                bool ans =_examQuestionsService.AddAllQuestions(examQuestions);

                if(ans && examId > 0)
                {
                    return Json(new { success = true, message = "Duplicate Exam Created Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    if(examId <= 0)
                    {
                        return Json(new { success = false, message = "Error While Creating Exam" }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { success = false, message = "Error While Adding Questions In Exam" }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        [HttpGet]
        public ActionResult GeneratePracticeTest()
        {
            PracticeTestModel model = new PracticeTestModel();

            BindLevel(ref model);
            BindSubject(ref model);
            return PartialView("_GeneratePracticeTestPartial", model);
        }

        public PracticeTestModel BindSubject(ref PracticeTestModel model)
        {
            var getSubject = _subjectService.GetAllSubjects();
            model._subjectList.Add(new SelectListItem() { Text = "Select Subject", Value = "" });
            foreach (var type in getSubject)
            {
                model._subjectList.Add(new SelectListItem() { Text = type.SubjectName.Trim(), Value = type.SubjectId.ToString() });
            }

            return model;
        }

        private PracticeTestModel BindLevel(ref PracticeTestModel model)
        {
            var data = _commonLookupService.GetLookupByType(LookupType.QuestionDifficultyLevel);
            model._difficultyList.Add(new SelectListItem() { Text = "Select Level     ", Value = "" });
            foreach (var item in data)
            {
                model._difficultyList.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
            }
            return model;
        }

        [HttpPost]
        public ActionResult GeneratePracticeTest(PracticeTestModel model)
        {
            int requiredMarks = 0;

            switch (model.Difficulty)
            {
                case Constants.QuestionDifficultyLevel.EASY:
                    requiredMarks = 30;
                    break;
                case Constants.QuestionDifficultyLevel.MEDIUM:
                    requiredMarks = 45;
                    break;
                case Constants.QuestionDifficultyLevel.HARD:
                    requiredMarks = 60;
                    break;
            }

            if (model.Marks < requiredMarks)
            {
                // Add a custom error to ModelState
                ModelState.AddModelError("Marks", $"Minimum marks for {model.Difficulty} level exam is {requiredMarks}.");
            }

            int requiredMinutes = 0;

            switch (model.Difficulty)
            {
                case Constants.QuestionDifficultyLevel.EASY:
                    requiredMinutes = 30;
                    break;
                case Constants.QuestionDifficultyLevel.MEDIUM:
                    requiredMinutes = 45;
                    break;
                case Constants.QuestionDifficultyLevel.HARD:
                    requiredMinutes = 60;
                    break;
            }

            if (model.DurationMin < requiredMinutes)
            {
                // Add a custom error to ModelState
                ModelState.AddModelError("DurationMin", $"Minimum minutes for {model.Difficulty} level exam is {requiredMinutes}.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return Json(new { success = false, errors });
            }

            int marks = 0;
            List<ExamQuestions> examQuestions = new List<ExamQuestions>();
            string subject = _subjectService.GetSubjectById(model.SubjectId).SubjectName;
            Random rnd = new Random();

            var markRatios = new Dictionary<string, (double easyRatio, double mediumRatio, double hardRatio)>
            {
                { "EASY", (0.7, 0.3, 0.0) },
                { "MEDIUM", (0.4, 0.4, 0.2) },
                { "HARD", (0.2, 0.3, 0.5) },
            };

            var total = model.Marks;
            var ratios = markRatios[model.Difficulty];

            int easyMarks = (int)(ratios.easyRatio * total);
            int mediumMarks = (int)(ratios.mediumRatio * total);
            int hardMarks = total - easyMarks - mediumMarks;

            var allQuestions = _questionService.GetAllQuestions()
            .Where(x => x.SubjectId == model.SubjectId)
            .Select(x => new
            {
                x.QuestionId,
                x.Difficulty,
                x.Marks
            })
            .ToList();

                    // Return if no questions
            if (allQuestions == null || allQuestions.Count == 0)
            {
                return Json(new { success = false, message = "Subject has no questions" });
            }

            var hardQuestions = allQuestions.Where(q => q.Difficulty == Constants.QuestionDifficultyLevel.HARD).OrderBy(x => rnd.Next()).ToList();
            var mediumQuestions = allQuestions.Where(q => q.Difficulty == Constants.QuestionDifficultyLevel.MEDIUM).OrderBy(x => rnd.Next()).ToList();
            var easyQuestions = allQuestions.Where(q => q.Difficulty == Constants.QuestionDifficultyLevel.EASY).OrderBy(x => rnd.Next()).ToList();

            List<dynamic> sourceList = new List<dynamic>();

            if (model.Difficulty == Constants.QuestionDifficultyLevel.HARD)
            {
                sourceList.AddRange(hardQuestions.OrderByDescending(x=>x.Marks));
                sourceList.AddRange(mediumQuestions.OrderByDescending(x => x.Marks)); 
                sourceList.AddRange(easyQuestions.OrderByDescending(x => x.Marks)); 
            }
            else if (model.Difficulty == Constants.QuestionDifficultyLevel.MEDIUM)
            {
                sourceList.AddRange(hardQuestions.OrderByDescending(x => x.Marks));
                sourceList.AddRange(mediumQuestions.OrderByDescending(x => x.Marks));
                sourceList.AddRange(easyQuestions.OrderByDescending(x => x.Marks));  
            }
            else
            {
                sourceList.AddRange(mediumQuestions.OrderByDescending(x => x.Marks));
                sourceList.AddRange(easyQuestions.OrderByDescending(x => x.Marks));
            }


            int easy = 0, medium = 0, hard = 0;
            foreach(var q in sourceList)
            {
                if(total >= easy + medium + hard)
                {
                    if(easy + q.Marks <= easyMarks && q.Difficulty == Constants.QuestionDifficultyLevel.EASY)
                    {
                        examQuestions.Add(new ExamQuestions
                        {
                            QuestionId = q.QuestionId,
                            Marks = q.Marks
                        });
                        easy += q.Marks;
                    }
                    else if (medium + q.Marks <= mediumMarks && q.Difficulty == Constants.QuestionDifficultyLevel.MEDIUM)
                    {
                        examQuestions.Add(new ExamQuestions
                        {
                            QuestionId = q.QuestionId,
                            Marks = q.Marks
                        });
                        medium += q.Marks;
                    }
                    else if (hard + q.Marks <= hardMarks && q.Difficulty == Constants.QuestionDifficultyLevel.HARD)
                    {
                        examQuestions.Add(new ExamQuestions
                        {
                            QuestionId = q.QuestionId,
                            Marks = q.Marks
                        });
                        hard += q.Marks;
                    }
                }
            }

            if (examQuestions.Any())
            {
                var exam = new Exams
                {
                    ExamName = subject+model.Difficulty+DateTime.UtcNow.ToString().Replace(" ",""),
                    ExamCode = subject + model.Difficulty + DateTime.UtcNow.ToString().Replace(" ", "").ToUpper(),
                    ExamStatus = marks == model.Marks ? Constants.ExamStatus.PUBLISHED : Constants.ExamStatus.DRAFT,
                    TotalMarks = model.Marks,
                    DurationMin = model.DurationMin,
                    PassingMarks = (int)(33 * model.Marks / 100),
                    IsActive = true,
                    CreatedBy = SessionHelper.UserId,
                    CreatedOn = DateTime.UtcNow,
                };
                int examId = _examService.CreateExam(exam);

                examQuestions.ForEach(x => x.ExamId = examId);

                _examQuestionsService.AddAllQuestions(examQuestions);

                return Json(new { success = true, message = "Exam Created Successully" });
            }

            return View("Index");
        }
    }
}