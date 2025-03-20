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
        private readonly AnswerService _answerService;
        private readonly QuestionService _questionService;

        public UserExamController()
        {
            _examService = new ExamService();
            _questionService = new QuestionService();
            _answerService = new AnswerService();
        }
        public ActionResult Index()
        {
            return View();
        }

       

        
    }
}