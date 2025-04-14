using DemoMVC.Helper;
using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Controllers
{
    public class UserExamCheckController : BaseController
    {
        private readonly UserExamService _userExamService;
        private readonly ExamService _examService;
        private readonly UserAnswerService _userAnswerService;
        private readonly QuestionService _questionService;
        private readonly ExamQuestionsService _examQuestionsService;
        private readonly AnswerService _answerService;
        private readonly UserProfileService _userProfileService;
        public UserExamCheckController()
        {
            _userExamService = new UserExamService();
            _examService = new ExamService();
            _userAnswerService = new UserAnswerService();
            _examQuestionsService = new ExamQuestionsService();
            _questionService = new QuestionService();
            _answerService = new AnswerService();
            _userProfileService = new UserProfileService();
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult CheckExam(int userExamId)
        {
            var userExamDetails = _userExamService.GetByUserExamId(userExamId);
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
                QuestionTypeId = q.QuestionTypeId,
                QuestionText = q.QuestionText,
                //QuestionImage = q.QuestionImage,
                QuestionType = q.QuestionType.QuestionTypeName,
                Marks = _examQuestionsService.GetMarksByQuestionId(q.QuestionId, userExamDetails.ExamId),
                Answers = _answerService.GetByQuestionId(q.QuestionId)
                    .Select(ans => new AnswerViewModel
                    {
                        IsCorrect = ans.IsCorrect,
                        AnswerText = ans.AnswerText
                    }).ToList()
            }).OrderBy(q => q.QuestionId).ToList();

            var userAnswers = _userAnswerService.GetAllAnswerByUser(userExamId).Select(a => new UserAnswerModel
            {
                Answer = a.AnswerText,
                QuestionId = a.QuestionId,
                UserAnswerId = a.UserAnswerId,
                UserExamId = a.UserExamId,
                ObtainedMarks = a.ObtainedMarks,
                IsEvaluate = a.IsEvaluate
            }).OrderBy(q => q.QuestionId).ToList();

            var user = _userProfileService.GetUserById(userExamDetails.UserId);

            // ✅ Automatic Checking for MCQs
            foreach (var question in questions)
            {
                var userAnswer = userAnswers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                if (userAnswer != null)
                {
                    userAnswer.ObtainedMarks = CheckAutomatic(question, userAnswer);
                    if (question.QuestionTypeId == 1 || question.QuestionTypeId == 2)
                    {
                        userAnswer.IsEvaluate = true;
                    }
                   
                }
            }
            var model = new UserExamCheckingModel
            {
                UserExamId = userExamId,
                Exam = examModel,
                Questions = questions,
                UserAnswers = userAnswers,
                UserProfile = user
            };
            _userExamService.UpdateResultStatus(userExamId);
            return View(model);
        }

        // ✅ Function to Automatically Check MCQs
        private int CheckAutomatic(QuestionAndAnswerModel question, UserAnswerModel userAnswer)
        {
            if (question.QuestionType == "Single Choice MCQ")
            {
                var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect)?.AnswerText;
                int obtainedMarks = userAnswer.Answer == correctAnswer ? question.Marks : 0;
                _userAnswerService.UpdateManualMarks(userAnswer.UserAnswerId, obtainedMarks);
                return obtainedMarks;

            }
            else if (question.QuestionType == "Multiple Choice MCQ")
            {
                var correctAnswers = new HashSet<string>(
                                question.Answers
                                .Where(a => a.IsCorrect)
                                .Select(a => a.AnswerText.Trim().ToLowerInvariant())
                    );

                var userAnswers = string.IsNullOrWhiteSpace(userAnswer.Answer)
                    ? new HashSet<string>()
                    : new HashSet<string>(
                        userAnswer.Answer
                            .Split(',')
                            .Select(a => a.Trim().ToLowerInvariant())
                    );

                bool isCorrect = correctAnswers.SetEquals(userAnswers);


                if (isCorrect)
                {
                    _userAnswerService.UpdateManualMarks(userAnswer.UserAnswerId, question.Marks);
                    return question.Marks;
                }
                else
                {
                    _userAnswerService.UpdateManualMarks(userAnswer.UserAnswerId, 0);
                    return 0;
                }
            }
            else if (question.QuestionType == "Text Answer" || question.QuestionType == "SQL Query")
            {
                if (userAnswer.ObtainedMarks > 0)
                {
                    return userAnswer.ObtainedMarks;
                }
                else
                {
                    var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect)?.AnswerText;
                    int obtainedMarks = string.Equals(userAnswer.Answer?.Trim(), correctAnswer?.Trim(), StringComparison.OrdinalIgnoreCase)
                        ? question.Marks
                        : 0;
                    if (obtainedMarks > 0)
                    {
                        _userAnswerService.UpdateManualMarks(userAnswer.UserAnswerId, question.Marks);
                    }

                    return obtainedMarks;
                }
            }
            return 0;
        }
        [HttpPost]
        public JsonResult SaveManualMarks(int userAnswerId, int obtainedMarks)
        {
            _userAnswerService.UpdateManualMarks(userAnswerId, obtainedMarks);

            return Json(new { success = true });
        }

        public JsonResult SaveUserResult(int userExamId, int totalObtainedMarks)
        {
            var userExamDetails = _userExamService.GetByUserExamId(userExamId);

            if (userExamDetails == null)
            {
                return Json(new { success = false, message = "Failed to store result." });
            }

            var examDetails = _examService.GetById(userExamDetails.ExamId);
            if (examDetails == null)
            {
                return Json(new { success = false, message = "Exam details not found." });
            }

            userExamDetails.ResultStatus = Constants.ResultStatus.EVALUATED;
            userExamDetails.Result = totalObtainedMarks;

            _userExamService.UpdateUserExam(userExamDetails);

            return Json(new { success = true, message = "Exam result saved successfully!" });
        }

    }
}