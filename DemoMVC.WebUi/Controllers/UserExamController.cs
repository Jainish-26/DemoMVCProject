using DemoMVC.Models;
using DemoMVC.Service;
using System;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    [AllowAnonymous]
    public class UserExamController : Controller
    {
        private readonly ExamService _examService;
        private readonly ExamLinkService _examLinkService;
        private readonly AnswerService _answerService;
        private readonly QuestionService _questionService;

        public UserExamController()
        {
            _examService = new ExamService();
            _examLinkService = new ExamLinkService();
            _questionService = new QuestionService();
            _answerService = new AnswerService();
        }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserExamView(int ExamId, string examCode, string uniqueId)
        {

            var examLink = _examLinkService.GetExamLinkByExamId(ExamId);

            if (examLink == null || examLink.ExpiryTime < DateTime.UtcNow || !examLink.IsActive)
            {
                return View("ExamLinkExpired");
            }

            var exam = _examService.GetById(ExamId);
            if (exam == null)
            {
                return HttpNotFound();
            }
            return View(exam);
        }

        [HttpGet]
        public JsonResult GenerateExamLink(int ExamId)
        {
            var examLink = _examLinkService.GetExamLinkByExamId(ExamId);
            string examCode = _examService.GetExamCode(ExamId);
            string uniqueId = Guid.NewGuid().ToString();
            int userId = SessionHelper.UserId;

            ExamLinks links = new ExamLinks();

            if (examLink != null)
            {
                if (examLink.ExpiryTime < DateTime.UtcNow)
                {
                    string newUniqueId = Guid.NewGuid().ToString();
                    string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    examLink.Link = baseUrl + Url.Action("UserExamView", "UserExam", new { examId = ExamId, examCode = examCode, uniqueId = newUniqueId });
                    examLink.UpdatedBy = userId;
                    examLink.UpdatedOn = DateTime.UtcNow;
                    examLink.ExpiryTime = DateTime.UtcNow.AddDays(7);
                    _examLinkService.UpdateExamLink(examLink);
                }
                return Json(new { success = true, link = examLink.Link }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                links.Link = baseUrl + Url.Action("UserExamView", "UserExam", new { examId = ExamId, examCode = examCode, uniqueId = uniqueId });
                links.ExamId = ExamId;
                links.CreatedBy = userId;
                links.CreatedOn = DateTime.UtcNow;
                links.ExpiryTime = DateTime.UtcNow.AddDays(7);
                links.IsActive = true;
                links.ExamLinkId = _examLinkService.CreateExamLink(links);

                return Json(new { success = true, link = links.Link }, JsonRequestBehavior.AllowGet);
            }

        }
    }
}