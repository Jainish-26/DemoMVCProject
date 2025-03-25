using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Helper;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Linq;
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
        private readonly MessageService _messageService;

        public UserExamController()
        {
            _examService = new ExamService();
            _questionService = new QuestionService();
            _answerService = new AnswerService();
            _userProfileService = new UserProfileService();
            _userExamService = new UserExamService();
            _examQuestionsService = new ExamQuestionsService();
            _messageService = new MessageService();
        }
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult UserExamLogIn(string userToken)
        {
            var userExamDetails = _userExamService.GetByUserToken(userToken);
            var userDetails = _userProfileService.GetUserById(userExamDetails.UserId);
            if (userToken == null || userExamDetails == null || userExamDetails.UserToken == null)
            {
                return HttpNotFound();
            }

            if (userExamDetails.ExpiryDate < DateTime.UtcNow)
            {
                return View("ExamLinkExpired");
            }

            if (userDetails == null)
            {
                return HttpNotFound();
            }

            var model = new StartTestModel
            {
                Email = userDetails.Email,
                ExamId = userExamDetails.ExamId
            };
            ViewBag.UserToken = userToken;

            return View(model);
        }

        [HttpPost]
        public JsonResult SaveUserAndExamDetails(string userToken, string userName, string userEmail, string name)
        {
            try
            {
                var userExamDetails = _userExamService.GetByUserToken(userToken);
                var userDetails = _userProfileService.GetUserByEmailId(userEmail);
                if (userDetails == null)
                {
                    return Json(new { success = false, message = "user not found." });
                }
                if (userExamDetails == null)
                {
                    return Json(new { success = false, message = "Invalid token" });
                }

                userDetails.UserName = userName;
                userDetails.Name = name;
                int userId = _userProfileService.UpdateUserProfile(userDetails);

                if (userId > 0)
                {
                    userExamDetails.StartTime = DateTime.UtcNow;
                    userExamDetails.ExamStatus = "ONGOING";

                    int userExamId = _userExamService.UpdateUserExam(userExamDetails);
                    if (userExamId > 0)
                    {
                        var redirectUrl = Url.Action("UserExamView", "UserExam", new { userToken });

                        return Json(new { success = true, redirectUrl });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "user not found." });
                }


            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            return Json(new { success = false });
        }


        public ActionResult UserExamView(string userToken)
        {
            var userExamDetails = _userExamService.GetByUserToken(userToken);
            var exam = _examService.GetById(userExamDetails.ExamId);

            var examModel = new ExamModel
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                ExamCode = exam.ExamCode,
                TotalMarks = exam.TotalMarks,
                PassingMarks = exam.PassingMarks,
                DurationMin = exam.DurationMin
            };

            var questions = _questionService.GetQuestionsByExamId(userExamDetails.ExamId)
            .Select(q => new QuestionAndAnswerModel
            {
                QuestionId = q.QuestionId,
                QuestionText = q.QuestionText,
                QuestionImage = q.QuestionImage,
                QuestionType = q.QuestionType.QuestionTypeName,
                Marks = _examQuestionsService.GetMarksByQuestionId(q.QuestionId, userExamDetails.ExamId),
                Answers = _answerService.GetByQuestionId(q.QuestionId)
                    .Select(ans => new AnswerViewModel
                    {
                        AnswerText = ans.AnswerText,
                    }).ToList()
            }).ToList();

            var model = new ExamQuestionViewModel
            {
                Exam = examModel,
                Questions = questions
            };
            ViewBag.UserExamId = userExamDetails.UserExamId;
            return View(model);
        }
        [HttpPost]
        public JsonResult GenerateExamLink(UserExamModel model)
        {
            if (model.Email.Count == 0)
            {
                return Json(new { success = false, message = "At least one email is required." });
            }

            int userId = SessionHelper.UserId;
            UserExams userExams = new UserExams();
            foreach (var item in model.Email)
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
                        if (userExamId > 0)
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
                        if (userExamDetails.ExpiryDate < DateTime.UtcNow)
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
                if(userExamDetails.ExpiryDate < DateTime.UtcNow)
                {
                    int userId = int.Parse(AESCrypto.Decrypt(userExamDetails.UserToken));
                    userExamDetails.UserToken = AESCrypto.Encrypt(userId.ToString());
                    userExamDetails.ExpiryDate = DateTime.UtcNow.AddDays(2);

                    _userExamService.UpdateUserExam(userExamDetails);

                    string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string updatedLink = baseUrl + Url.Action("UserExamLogIn", "UserExam", 
                        new { 
                            userToken = userExamDetails.UserToken
                        });
                    return Json(new { success = true, link = updatedLink });
                }
                else
                {
                    string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string generatedLink = baseUrl + Url.Action("UserExamLogIn", "UserExam",
                        new
                        {
                            userToken = userExamDetails.UserToken
                        });

                    return Json(new { success = true, link = generatedLink });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        public ActionResult LoadStartTest(UserExamViewModel userExamModel)
        {
            var userToken = Request.Headers["UserToken"];

            var examDetails = _examService.GetById(userExamModel.ExamId);

            var model = new StartTestModel
            {
                ExamId = userExamModel.ExamId,
                UserName = userExamModel.UserName,
                UserToken = userToken,
                Name = userExamModel.Name,
                Email = userExamModel.Email,
                ExamName = examDetails.ExamName,
                Duration = examDetails.DurationMin,
                TotalMarks = examDetails.TotalMarks,
                PassingMarks = examDetails.PassingMarks
            };

            return PartialView("_StartTestPartial", model);
        }

        public JsonResult CheckDuplicateUserName(string UserName)
        {
            var checkduplicate = _userProfileService.CheckDuplicateUserName(UserName).ToList();

            if (checkduplicate.Count() > 0)
            {
                var message = _messageService.GetMessageByCode(Constants.MessageCode.USERNAMEEXIST);
                return Json(message, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}