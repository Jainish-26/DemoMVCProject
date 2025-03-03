using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.ApplicationServices;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class QuestionController : BaseController
    {
        public readonly QuestionService _questionService;
        public readonly AnswerService _answerService;
        public readonly QuestionTypeService _questionTypeService;
        public readonly SubjectService _subjectService;
        public readonly CommonLookupService _commonLookupService;

        public QuestionController()
        {
            _answerService = new AnswerService();
            _questionService = new QuestionService();
            _questionTypeService = new QuestionTypeService();
            _subjectService = new SubjectService();
            _commonLookupService = new CommonLookupService();
        }
        // GET: Question
        public ActionResult Index()
        {
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), AccessPermission.IsView))
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

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            int userId = SessionHelper.UserId;
            QuestionAndAnswerModel model = new QuestionAndAnswerModel();

            if (id.HasValue)
            {
                var que = _questionService.GetById(id.Value);

                if (que != null)
                {
                    model.QuestionId = id.Value;
                    model.QuestionText = que.QuestionText;
                    model.Marks = que.Marks;
                    model.QuestionImage = que.QuestionImage;
                    model.IsActive = que.IsActive;
                    model.Difficulty = que.Difficulty;
                    model.SubjectId = que.SubjectId;
                    model.QuestionTypeId = que.QuestionTypeId;
                    model.Answers = _answerService.GetByQuestionId(que.QuestionId).Select(
                        
                        ans=> new AnswerViewModel
                        {
                           QuestionId = ans.QuestionId,
                            AnswerId = ans.AnswerId,
                            AnswerText = ans.AnswerText,
                            IsCorrect = ans.IsCorrect,
                        }).ToList();

                }
            }
            BindLevel(ref model);
            BindQuestionType(ref model);
            BindSubject(ref model);
            return View(model);
        }

        [HttpPost]
        public ActionResult Create(QuestionAndAnswerModel model, HttpPostedFileBase QuestionImage)
        {
            string actionPermission = "";
            if (model.QuestionId == 0)
            {
                actionPermission = AccessPermission.IsAdd;
            }
            else if (model.QuestionId > 0)
            {
                actionPermission = AccessPermission.IsEdit;
            }
            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return RedirectToAction("AccessDenied", "Base");
            }

            if (ModelState.IsValid && model.Answers.Count!=0)
            {
                SaveAndUpdateQuestion(model,QuestionImage);
                return RedirectToAction("Index");
            }
            else
            {
                BindSubject(ref model);
                BindLevel(ref model);
                BindQuestionType(ref model);
                return View(model);
            }
        }

        public QuestionAndAnswerModel SaveAndUpdateQuestion(QuestionAndAnswerModel model, HttpPostedFileBase QuestionImage)
        {
            int userId = SessionHelper.UserId;
            Questions que = new Questions();
            Answers ans = new Answers();

            if (model.QuestionId > 0)
            {
                que = _questionService.GetById(model.QuestionId);
              
            }

            if (QuestionImage != null)
            {
                string uniqueCode = new string(Enumerable.Range(0, 10).Select(_ => "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789"[new Random().Next(62)]).ToArray());

                que.QuestionImage = uniqueCode+Path.GetExtension(QuestionImage.FileName);
                QuestionImage.SaveAs(Server.MapPath("~/content/QuestionImage/") + que.QuestionImage);
            }
            
            que.QuestionText = model.QuestionText;
            que.Marks = model.Marks;
            que.IsActive = model.IsActive;
            que.Difficulty = model.Difficulty;
            que.SubjectId = model.SubjectId;
            que.QuestionTypeId = model.QuestionTypeId;

            if (model.QuestionId == 0)
            {
                que.CreatedBy = userId;
                que.CreatedOn = DateTime.UtcNow;
                que.QuestionId = _questionService.CreateQuestion(que);

                if (model.Answers == null || !model.Answers.Any(a => a.IsCorrect))
                {
                    ModelState.AddModelError("Answers", "At least one correct answer must be selected.");
                }
                else
                {
                    foreach (var answer in model.Answers)
                    {

                        ans.AnswerText = answer.AnswerText;
                        ans.IsCorrect = answer.IsCorrect;
                        ans.QuestionId = que.QuestionId;
                        if (answer.AnswerId == 0)
                        {

                            if (model.Answers == null || !model.Answers.Any(a => !string.IsNullOrWhiteSpace(a.AnswerText)))
                            {
                                ModelState.AddModelError("AnswerText", "At least one answer must be provided.");
                            }
                            ans.CreatedBy = userId;
                            ans.CreatedOn = DateTime.UtcNow;
                            ans.AnswerId = _answerService.CreateAnswer(ans);
                        }
                    }
                }
            }
            else
            {
                que.UpdatedBy = userId;
                que.UpdatedOn = DateTime.UtcNow;
                _questionService.UpdateQuestions(que);

                var existingAnswers = _answerService.GetByQuestionId(que.QuestionId);
                var modelAnswerIds = model.Answers.Select(a => a.AnswerId).ToList();

                foreach (var existingAnswer in existingAnswers)
                {
                    if (!modelAnswerIds.Contains(existingAnswer.AnswerId))
                    {
                        _answerService.DeleteAnswer(existingAnswer.AnswerId);
                    }
                }

                

                foreach (var answer in model.Answers)
                {
                    var existingAnswer = existingAnswers.FirstOrDefault(a => a.AnswerId == answer.AnswerId);
                    if (existingAnswer != null)
                    {
                        
                        if (model.Answers == null || !model.Answers.Any(a => !string.IsNullOrWhiteSpace(a.AnswerText)))
                        {
                            ModelState.AddModelError("AnswerText", "At least one answer must be provided.");
                        }
                        if (model.Answers == null || !model.Answers.Any(a => a.IsCorrect))
                        {
                            ModelState.AddModelError("Answers", "At least one correct answer must be selected.");
                        }
                        existingAnswer.AnswerText = answer.AnswerText;
                        existingAnswer.IsCorrect = answer.IsCorrect;

                        if (existingAnswer.AnswerText != answer.AnswerText || existingAnswer.IsCorrect != answer.IsCorrect )
                        {
                            existingAnswer.UpdatedBy = userId;
                            existingAnswer.UpdatedOn = DateTime.UtcNow;
                            _answerService.UpdateAnswers(existingAnswer);
                            
                        }
                        

                    }
                    else if (answer.AnswerId == 0)
                    {
                        Answers newAnswer = new Answers
                        {
                            AnswerText = answer.AnswerText,
                            IsCorrect = answer.IsCorrect,
                            QuestionId = que.QuestionId,
                            CreatedBy = userId,
                            CreatedOn = DateTime.UtcNow
                        };
                        newAnswer.AnswerId = _answerService.CreateAnswer(newAnswer);
                    }
                }
                
            }

            return model;
            
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            var data = _questionService.GetAllQuestionsGridModel();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public QuestionAndAnswerModel BindQuestionType(ref QuestionAndAnswerModel questionAndAnswerModel)
        {
            var getQuestionTypes = _questionTypeService.GetAllQuestionTypes();
            questionAndAnswerModel._questionTypeList.Add(new SelectListItem() { Text = " Question Type     ", Value = "" });
            foreach (var type in getQuestionTypes)
            {
                questionAndAnswerModel._questionTypeList.Add(new SelectListItem() { Text = type.QuestionTypeName.Trim(), Value = type.QuestionTypeId.ToString() });
            }

            return questionAndAnswerModel;
        }
        public QuestionAndAnswerModel BindSubject(ref QuestionAndAnswerModel questionAndAnswerModel)
        {
            var getSubject = _subjectService.GetAllSubjects();
            questionAndAnswerModel._subjectList.Add(new SelectListItem() { Text = " Subject", Value = "" });
            foreach (var type in getSubject)
            {
                questionAndAnswerModel._subjectList.Add(new SelectListItem() { Text = type.SubjectName.Trim(), Value = type.SubjectId.ToString() });
            }

            return questionAndAnswerModel;
        }

        private QuestionAndAnswerModel BindLevel(ref QuestionAndAnswerModel questionAndAnswerModel)
        {
            var data = _commonLookupService.GetLookupByType(LookupType.QuestionDifficultyLevel);
            questionAndAnswerModel._difficultyList.Add(new SelectListItem() { Text = " Level     ", Value = "" });
            foreach (var item in data)
            {
                questionAndAnswerModel._difficultyList.Add(new SelectListItem() { Text = item.Name, Value = item.Code });
            }
            return questionAndAnswerModel;
        }

        public ActionResult Delete(int? id)
        {
            string actionPermission = AccessPermission.IsDelete;

            if (!CheckPermission(AuthorizeFormAccess.FormAccessCode.QUESTION.ToString(), actionPermission))
            {
                return Json(new { success = false, message = "Access Denied" });
            }

            int userId = SessionHelper.UserId;

            try
            {
                bool ans = _questionService.DeleteQuestion(id.Value);

                if (ans)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Question Not Deleted" });
                }

            }
            catch (Exception ex)
            {
                // Log the exception as needed
                return Json(new { success = false, message = ex.Message });
            }
        }
        public ActionResult GetImagePopup(int id)
        {
            var question = _questionService.GetAllQuestionsGridModel().FirstOrDefault(q => q.QuestionId == id);

            if (question.QuestionImage == null)
            {
                return HttpNotFound();
            }

            return PartialView("_ImagePopup", question);
        }


    }
}