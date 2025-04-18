﻿using DemoMVC.Helper;
using DemoMVC.Models;
using DemoMVC.Service;
using DemoMVC.WebUi.Helper;
using DemoMVC.WebUi.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using WebMatrix.WebData;

namespace DemoMVC.WebUi.Controllers
{
    public class UserExamController : BaseController
    {
        private readonly ExamService _examService;
        private readonly AnswerService _answerService;
        private readonly QuestionService _questionService;
        private readonly UserProfileService _userProfileService;
        private readonly UserExamService _userExamService;
        private readonly ExamQuestionsService _examQuestionsService;
        private readonly MessageService _messageService;
        private readonly UserAnswerService _userAnswerService;
        private readonly QuestionMediaService _questionMediaService;

        public UserExamController()
        {
            _examService = new ExamService();
            _questionService = new QuestionService();
            _answerService = new AnswerService();
            _userProfileService = new UserProfileService();
            _userExamService = new UserExamService();
            _examQuestionsService = new ExamQuestionsService();
            _messageService = new MessageService();
            _userAnswerService = new UserAnswerService();
            _questionMediaService = new QuestionMediaService();
        }
        public ActionResult Index()
        {
            return View();
        }

        //UserLogIn Page When Open Links
        [AllowAnonymous]
        [HttpGet]
        public ActionResult UserExamLogIn(string userToken)
        {
            var userExamDetails = _userExamService.GetByUserToken(userToken);
            var userDetails = _userProfileService.GetUserById(userExamDetails.UserId);
            var examDetails = _examService.GetById(userExamDetails.ExamId);
            if (userToken == null || userExamDetails == null || userExamDetails.UserToken == null)
            {
                ViewBag.ErrorMessage = "User Not found for this Link Verify it with Your Administartor.";
                return View("ErrorPage");
            }
            if (userExamDetails.ExamStatus == Constants.UserExamStatus.COMPLETED && userExamDetails.ExpiryDate < DateTime.UtcNow)
            {
                return View("ExamSummary");
            }
            if (userExamDetails.ExpiryDate < DateTime.UtcNow)
            {
                return View("ExamLinkExpired");
            }

            if (userExamDetails.ExamStatus == Constants.UserExamStatus.ONGOING)
            {
                var model = new StartTestModel
                {
                    Email = userDetails.Email,
                    UserName = userDetails.UserName,
                    Name = userDetails.Name,
                    ExamName = examDetails.ExamName,
                    ExamId = userExamDetails.ExamId,
                    Duration = examDetails.DurationMin,
                    PassingMarks = examDetails.PassingMarks,
                    TotalMarks = examDetails.TotalMarks,
                    UserExamStatus = userExamDetails.ExamStatus
                };
                ViewBag.UserToken = userToken;

                return View(model);
            }
            else
            {
                if (userDetails == null)
                {
                    ViewBag.ErrorMessage = "User not found. Plzz contact Support team.";
                    return View("ErrorPage");
                }

                var model = new StartTestModel
                {
                    Email = userDetails.Email,
                    UserName = userDetails.UserName?? "",
                    Name = userDetails.Name ?? "",
                    ExamName = examDetails.ExamName,
                    ExamId = userExamDetails.ExamId,
                    Duration = examDetails.DurationMin,
                    PassingMarks = examDetails.PassingMarks,
                    TotalMarks = examDetails.TotalMarks,
                    UserExamStatus = userExamDetails.ExamStatus
                };
                ViewBag.UserToken = userToken;

                return View(model);
            }
        }

        //Save Details Enters From LogIn Page
        [HttpPost]
        [AllowAnonymous]
        public ActionResult UserExamLogIn(string userToken, string userName, string name)
        {
            try
            {
                var userExamDetails = _userExamService.GetByUserToken(userToken);

                var decryptedToken = AESCrypto.Decrypt(userExamDetails.UserToken);
                int userId = decryptedToken.userId;
                int examId = decryptedToken.examId;
                var userDetails = _userProfileService.GetUserById(userId);
                var examDetails = _examService.GetById(userExamDetails.ExamId);

                if (userDetails == null)
                {
                    ViewBag.ErrorMessage = "User not found. Please check your email.";
                    return View("ErrorPage");
                }
                if (userExamDetails == null)
                {
                    ViewBag.ErrorMessage = "Invalid token. Please try again.";
                    return View("ErrorPage");
                }

                if (userExamDetails.ExamStatus == Constants.UserExamStatus.ONGOING)
                {
                    return RedirectToAction("UserExamView", "UserExam", new { userToken });
                }
                else
                {
                    // Update user details
                    userDetails.UserName = userName;
                    userDetails.Name = name;
                    int updatedUserId = _userProfileService.UpdateUserProfile(userDetails);

                    if (updatedUserId <= 0)
                    {
                        ViewBag.ErrorMessage = "Failed to update user profile.";
                        return View("ErrorPage");
                    }

                    // Update exam details
                    userExamDetails.StartTime = DateTime.UtcNow;
                    userExamDetails.EndTime = DateTime.UtcNow.AddMinutes(examDetails.DurationMin);
                    userExamDetails.ExamStatus = Constants.UserExamStatus.ONGOING;

                    int userExamId = _userExamService.UpdateUserExam(userExamDetails);
                    if (userExamId <= 0)
                    {
                        ViewBag.ErrorMessage = "Failed to update exam status.";
                        return View("ErrorPage");
                    }

                    var questions = _questionService.GetQuestionsByExamId(userExamDetails.ExamId)
                    .Select(q => new QuestionAndAnswerModel
                    {
                        QuestionId = q.QuestionId,
                    }).ToList();

                    UserAnswers answer = new UserAnswers();
                    answer.UserExamId = userExamDetails.UserExamId;
                    foreach (var q in questions)
                    {
                        answer.QuestionId = q.QuestionId;
                        _userAnswerService.CreateUserAnswer(answer);
                    }
                    // Redirect to the User Exam View
                    return RedirectToAction("UserExamView", "UserExam", new { userToken });
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An error occurred: {ex.Message}";
                return View("ErrorPage");
            }
        }

        //Exam View
        [AllowAnonymous]
        public ActionResult UserExamView(string userToken)
        {
            var userExamDetails = _userExamService.GetByUserToken(userToken);
            var exam = _examService.GetById(userExamDetails.ExamId);

            if(userExamDetails.ExamStatus == Constants.UserExamStatus.COMPLETED)
            {
                return RedirectToAction("ExamSummary", new { id = exam.ExamId });
            }
            var examModel = new ExamModel
            {
                ExamId = exam.ExamId,
                ExamName = exam.ExamName,
                ExamCode = exam.ExamCode,
                TotalMarks = exam.TotalMarks,
                PassingMarks = exam.PassingMarks,
                DurationMin = exam.DurationMin
            };

            var questions = _questionService.GetQuestionsByExamId(exam.ExamId)
            .Select(q =>
            {
                var mediaList = _questionMediaService.GetMediaByQuestionId(q.QuestionId);

                return new QuestionAndAnswerModel
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    IsActive = q.IsActive,
                    QuestionImages = mediaList.Any() && mediaList != null ? mediaList : null,
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
            var model = new ExamQuestionViewModel
            {
                Exam = examModel,
                Questions = questions
            };
            ViewBag.UserExamId = userExamDetails.UserExamId;
            return View(model);
        }

        //Generate Token For User
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
                        userExams.UserToken = AESCrypto.Encrypt(userDetails.UserId, model.ExamId);
                        userExams.ExpiryDate = DateTime.UtcNow.AddDays(2);
                        userExams.ExamStatus = Constants.UserExamStatus.PENDING;
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
                            userExams.UserToken = AESCrypto.Encrypt(userDetails.UserId, model.ExamId);
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
                    userExams.UserToken = AESCrypto.Encrypt(userDetails.UserId, model.ExamId);
                    userExams.ExpiryDate = DateTime.UtcNow.AddDays(2);
                    userExams.ExamStatus = Constants.UserExamStatus.PENDING;
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
        public ActionResult GetGridData([DataSourceRequest] DataSourceRequest request,string searchTerm)
        {
            var data = _userExamService.GetUserExamGrids();
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();

                data = data.Where(q =>
                        (q.Email != null && q.Email.ToLower().Contains(searchTerm)) ||
                        (q.ExamName != null && q.ExamName.ToLower().Contains(searchTerm)) ||
                        (q.Result != null && q.Result.ToString().ToLower().Contains(searchTerm)) ||
                        (q.ExamStatus != null && q.ExamStatus.ToLower().Contains(searchTerm)) ||
                        (q.ResultStatus != null && q.ResultStatus.ToLower().Contains(searchTerm))
                    );
            }
            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        //Create A Link For User
        public JsonResult UserExamLink(int userExamId)
        {
            try
            {
                var userExamDetails = _userExamService.GetByUserExamId(userExamId);

                if (userExamDetails.ExamStatus != Constants.UserExamStatus.COMPLETED)
                {
                    if (userExamDetails.ExamStatus != Constants.UserExamStatus.ONGOING)
                    {
                        if (userExamDetails.ExpiryDate < DateTime.UtcNow)
                        {
                            var decryptedToken = AESCrypto.Decrypt(userExamDetails.UserToken);
                            int userId = decryptedToken.userId;
                            int examId = decryptedToken.examId;

                            userExamDetails.UserToken = AESCrypto.Encrypt(userId, examId);
                            userExamDetails.ExpiryDate = DateTime.UtcNow.AddDays(2);

                            _userExamService.UpdateUserExam(userExamDetails);

                            string baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                            string updatedLink = baseUrl + Url.Action("UserExamLogIn", "UserExam",
                                new
                                {
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
                    else
                    {
                        return Json(new { success = false, message = "User Started Exam Already" });
                    }
                }
                else
                {
                    return Json(new { success = false, message = "User Completed Exam Already" });
                }
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        //Check Duplicate UserName
        [AllowAnonymous]
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

        //Submit Exam
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SubmitExam(int UserExamId)
        {
            var userExamDetails = _userExamService.GetByUserExamId(UserExamId);

            if (userExamDetails != null)
            {
                userExamDetails.ExamStatus = Constants.UserExamStatus.COMPLETED;
                userExamDetails.ResultStatus = Constants.ResultStatus.PENDING;
                userExamDetails.EndTime = DateTime.UtcNow;
                userExamDetails.ExpiryDate = DateTime.UtcNow;

                int updatedUserExamId = _userExamService.UpdateUserExam(userExamDetails);
                if (updatedUserExamId > 0)
                {
                    return Json(new { success = true, redirectUrl = Url.Action("ExamSummary", "UserExam", new { UserExamId }) });
                }
                else
                {
                    return Json(new { success = false, message = "Exam submission failed." });
                }
            }
            else
            {
                return Json(new { success = false, message = "Exam Not Available." });
            }
        }


        [AllowAnonymous]
        //Exam Summary View
        public ActionResult ExamSummary()
        {
            ViewBag.DisableBackButton = true;
            return View();
        }

        // Get Remaining Time From Database
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetRemainingTime(int userExamId)
        {
            var userExamDetails = _userExamService.GetByUserExamId(userExamId);

            if (userExamDetails == null)
            {
                return Json(new { minutes = 0, seconds = 0 }, JsonRequestBehavior.AllowGet);
            }

            // Get the exam end time from the database
            DateTime? endTime = userExamDetails.EndTime;

            if (endTime == null)
            {
                return Json(new { minutes = 0, seconds = 0 }, JsonRequestBehavior.AllowGet);
            }

            // Calculate remaining time
            TimeSpan remainingTime = endTime.Value - DateTime.UtcNow;

            if (remainingTime.TotalSeconds <= 0)
            {
                return Json(new { minutes = 0, seconds = 0 }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                minutes = remainingTime.Minutes,
                seconds = remainingTime.Seconds
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TopScorers()
        {
            LeaderboardModel model = new LeaderboardModel();
            BindExams(ref model);
            return View(model);
        }
        [HttpPost]
        public ActionResult GetTopScorers([DataSourceRequest] DataSourceRequest request,int ExamId,string searchTerm)
        {
            var data = _userExamService.GetByExamId(ExamId);
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();

                data = data
                    .ToList()
                    .Where(x =>
                        (x.Email != null && x.Email.ToLower().Contains(searchTerm)) ||
                        (x.ExamName != null && x.ExamName.ToLower().Contains(searchTerm)) ||
                        x.Result.ToString().Contains(searchTerm) ||
                        (x.StartTime.HasValue && x.StartTime.Value.ToString("dd-MM-yyyy").Contains(searchTerm))
                    )
                    .ToList();
            }

            return Json(data.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }

        public LeaderboardModel BindExams(ref LeaderboardModel leaderboard)
        {
            var getExam = _examService.GetAllExam();
            //leaderboard._ExamList.Add(new SelectListItem() { Text = "Select Exam", Value = "" });
            foreach (var exam in getExam)
            {
                leaderboard._ExamList.Add(new SelectListItem() { Text = exam.ExamName.Trim(), Value = exam.ExamId.ToString() });
            }

            return leaderboard;
        }
    }
}