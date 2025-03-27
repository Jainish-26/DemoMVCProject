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
        public JsonResult SaveUserAnswer(UserAnswers answerData)
        {
            try
            {
                _userAnswerService.SaveOrUpdateUserAnswer(answerData);

                return Json(new { success = true } , JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(new { success = false , message =e.Message} , JsonRequestBehavior.AllowGet);
            }
        }   

        [HttpGet]
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