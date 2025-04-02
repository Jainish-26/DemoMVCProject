using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using System;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class UserAnswerController : BaseController
    {
        private readonly UserAnswerService _userAnswerService;
        private readonly UserExamService _userExamService;

        public UserAnswerController()
        {
            _userAnswerService = new UserAnswerService();
            _userExamService = new UserExamService();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult SaveUserAnswer(UserAnswers answerData)
        {
            try
            {
                var existingAnswer = _userAnswerService.GetExistingAnswer(answerData.UserExamId, answerData.QuestionId);
                var userExamDetails = _userExamService.GetByUserExamId(answerData.UserExamId);
                if(userExamDetails.ExamStatus == Constants.UserExamStatus.COMPLETED)
                {
                    return Json(new { success = false, message = "Your Response Already Saved Plzz Close Exam Otherwise You Will Be Disqualified" }, JsonRequestBehavior.AllowGet);
                }

                if (existingAnswer != null)
                {
                    if (existingAnswer.AnswerText != answerData.AnswerText || !existingAnswer.IsVisited)
                    {
                        existingAnswer.AnswerText = answerData.AnswerText;
                        existingAnswer.IsVisited = true;
                        _userAnswerService.UpdateAnswer(existingAnswer);
                    }
                }

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAllAnswers(int userExamId)
        {
            try
            {
                var answers = _userAnswerService.GetAllAnswerByUser(userExamId);

                return Json(new { success = true, data = answers }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}