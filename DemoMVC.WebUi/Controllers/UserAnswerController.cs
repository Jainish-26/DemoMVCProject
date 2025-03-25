using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DemoMVC.WebUi.Controllers
{
    [AllowAnonymous]
    public class UserAnswerController : BaseController
    {
        private readonly UserAnswerService _userAnswerService;

        public UserAnswerController()
        {
            _userAnswerService = new UserAnswerService();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult StoreUserAnswer(UserAnswerModel userAnswerModel)
        {
            try
            {
                if (userAnswerModel == null)
                {
                    return Json(new { success = false, message = "Invalid data received." });
                }

                var existingAnswers = _userAnswerService.GetAnswers(userAnswerModel.UserExamId, userAnswerModel.QuestionId)
                                                        .Select(a => a.AnswerText)
                                                        .ToList();

                var newAnswers = userAnswerModel.Answer.ToList(); 

                var answersToDelete = new List<string>();
                foreach (var answer in existingAnswers)
                {
                    if (!newAnswers.Contains(answer))
                    {
                        answersToDelete.Add(answer);
                    }
                }

                // Find answers to insert (present in new but not in existing)
                var answersToInsert = new List<string>();
                foreach (var answer in newAnswers)
                {
                    if (!existingAnswers.Contains(answer))
                    {
                        answersToInsert.Add(answer);
                    }
                }

                foreach (var answer in answersToDelete)
                {
                    bool ans = _userAnswerService.DeleteUserAnswers(userAnswerModel.UserExamId, userAnswerModel.QuestionId, answer);
                    if (ans)
                    {
                        continue;
                    }
                    else
                    {
                        return Json(new { success = false, message = "Answer Not Deleted"}, JsonRequestBehavior.AllowGet);
                    }
                }

                // Insert new answers that are different from existing ones
                foreach (var answer in answersToInsert)
                {
                    var newAnswer = new UserAnswers
                    {
                        UserExamId = userAnswerModel.UserExamId,
                        QuestionId = userAnswerModel.QuestionId,
                        AnswerText = answer
                    };
                    _userAnswerService.CreateUserAnswer(newAnswer);
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetUserAnswer(int userExamId, int questionId)
        {
            try
            {
                var answers = _userAnswerService.GetAnswers(userExamId, questionId).Select(x => x.AnswerText).ToList();
               
                if (answers == null)
                {
                    return Json(new { success = true, answer = "", message = "No answer found." }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { success = true, answer = answers, message = "Answer retrieved." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
            
            
        }
    }
}