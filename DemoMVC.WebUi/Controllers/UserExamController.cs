using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Helper;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DemoMVC.WebUi.Controllers
{
    [AllowAnonymous]
    public class UserExamController : Controller
    {
        private readonly ExamService _examService;
        private readonly AnswerService _answerService;
        private readonly QuestionService _questionService;
        private readonly UserProfileService _userProfileService;
        private readonly UserExamService _userExamService;
        private readonly ExamQuestionsService _examQuestionsService;

        public UserExamController()
        {
            _examService = new ExamService();
            _questionService = new QuestionService();
            _answerService = new AnswerService();
            _userProfileService = new UserProfileService();
            _userExamService = new UserExamService();
            _examQuestionsService = new ExamQuestionsService();
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateExamLink(UserExamModel model)
        {
            if ( model.Email.Count == 0)
            {
                return Json(new { success = false, message = "At least one email is required." });
            }

            int userId = SessionHelper.UserId;
            UserExams userExams = new UserExams();
            foreach(var item in model.Email)
            {
                //Get All Emails From UserProfile
                var emailList = _userProfileService.GetAllEmails();

                //If Mail ALready Present In UserProfile
                if (emailList.Contains(item))
                {
                    var userDetails = _userProfileService.GetUserByEmailId(item);

                    //Get UserExamDetails By User And ExamId
                    var userExamDetails = _userExamService.GetByUserIdAndExamId(model.ExamId, userDetails.UserId);

                    //Token Not Craeted
                    if (userExamDetails == null)
                    {
                        userExams.UserId = userDetails.UserId;
                        userExams.ExamId = model.ExamId;
                        userExams.UserToken = AESCrypto.Encrypt(userDetails.UserId.ToString());
                        userExams.ExpiryDate = DateTime.UtcNow.AddDays(2);
                        userExams.ExamStatus = "PENDING";
                        userExams.CreatedBy = userId;
                        userExams.CreatedOn = DateTime.UtcNow;

                        int userExamId = _userExamService.CreateUserExam(userExams);
                        if(userExamId > 0)
                        {
                            continue;
                        }
                        else
                        {
                            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                        }
                    }
                    //If Token Available Check Expired Or Not
                    else
                    {
                        if(userExamDetails.ExpiryDate < DateTime.UtcNow)
                        {
                            userExams.UserToken = AESCrypto.Encrypt(userExamDetails.UserId.ToString());
                            userExams.ExpiryDate = DateTime.UtcNow.AddDays(2);
                            userExams.UpdatedBy = userId;
                            userExams.UpdatedOn = DateTime.UtcNow;

                            int userExamId = _userExamService.UpdateUserExam(userExams);
                            if (userExamId > 0)
                            {
                                continue;
                            }
                            else
                            {
                                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                //If Email Not Exist
                else
                {
                    WebSecurity.CreateUserAndAccount(item, "Candidate@26", propertyValues:
                                               new
                                               {
                                                   Email = item,
                                                   IsActive = 1,
                                                   IsDeleted = 0,
                                                   CreatedOn = DateTime.UtcNow,
                                                   CreatedBy = userId,
                                               });
                    Roles.AddUserToRole(item, "CANDIDATE");

                    var userDetails = _userProfileService.GetUserByEmailId(item);

                    userExams.UserId = userDetails.UserId;
                    userExams.ExamId = model.ExamId;
                    userExams.UserToken = AESCrypto.Encrypt(userDetails.UserId.ToString());
                    userExams.ExpiryDate = DateTime.UtcNow.AddDays(2);
                    userExams.ExamStatus = "PENDING";
                    userExams.CreatedBy = userId;
                    userExams.CreatedOn = DateTime.UtcNow;

                    int userExamId = _userExamService.CreateUserExam(userExams);
                    if (userExamId > 0)
                    {
                        continue;
                    }
                    else
                    {
                        return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                    }
                }

            }
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request)
        {
            var data = _userExamService.GetUserExamGrids();
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public JsonResult UserExamLink(int userExamId)
        {
            try
            {
                var userExamDetails = _userExamService.GetByUserExamId(userExamId);
                string newUniqueId = Guid.NewGuid().ToString();
                string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string generatedLink = baseUrl + Url.Action("UserExamView", "UserExam", new { userToken = userExamDetails.UserToken});

                return Json(new { success = true, link = generatedLink });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }
    }
}