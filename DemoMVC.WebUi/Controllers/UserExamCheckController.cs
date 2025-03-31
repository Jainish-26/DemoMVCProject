using DemoMVC.Service;
using DemoMVC.WebUi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
        public UserExamCheckController()
        {
            _userExamService = new UserExamService();
            _examService = new ExamService();
            _userAnswerService = new UserAnswerService();
            _examQuestionsService = new ExamQuestionsService();
            _questionService = new QuestionService();
            _answerService = new AnswerService();
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
                QuestionText = q.QuestionText,
                QuestionImage = q.QuestionImage,
                QuestionType = q.QuestionType.QuestionTypeName,
                Marks = _examQuestionsService.GetMarksByQuestionId(q.QuestionId, userExamDetails.ExamId),
                Answers = _answerService.GetByQuestionId(q.QuestionId)
                    .Select(ans => new AnswerViewModel
                    {
                        IsCorrect = ans.IsCorrect,
                        AnswerText = ans.AnswerText
                    }).ToList()
            }).ToList();

            var userAnswers = _userAnswerService.GetAllAnswerByUser(userExamId).Select(a => new UserAnswerModel
            {
                Answer = a.AnswerText,
                QuestionId = a.QuestionId,
                UserAnswerId = a.UserAnswerId,
                UserExamId = a.UserExamId,
                ObtainedMarks = a.ObtainedMarks,
                IsEvaluate = a.IsEvaluate
            }).ToList();

            _userExamService.UpdateResultStatus(userExamId);
            // ✅ Automatic Checking for MCQs
            foreach (var question in questions)
            {
                var userAnswer = userAnswers.FirstOrDefault(a => a.QuestionId == question.QuestionId);
                if (userAnswer != null)
                {
                    userAnswer.ObtainedMarks = CheckAutomatic(question, userAnswer);
                }
            }

            var model = new UserExamChekingModel
            {
                Exam = examModel,
                Questions = questions,
                UserAnswers = userAnswers
            };

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
                var correctAnswers = question.Answers.Where(a => a.IsCorrect).Select(a => a.AnswerText).ToList();
                var userAnswers = userAnswer.Answer.Split(',').Select(a => a.Trim()).ToList();

                if (!userAnswers.All(ua => question.Answers.Any(q => q.AnswerText == ua)))
                {
                    return 0;
                }

                int correctCount = userAnswers.Count(answer => correctAnswers.Contains(answer));
                int incorrectCount = userAnswers.Count(answer => !correctAnswers.Contains(answer));

                if (correctCount == correctAnswers.Count && incorrectCount == 0)
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
                if(userAnswer.ObtainedMarks > 0)
                {
                    return userAnswer.ObtainedMarks;
                }
                else
                {
                    var correctAnswer = question.Answers.FirstOrDefault(a => a.IsCorrect)?.AnswerText;
                    int obtainedMarks =  string.Equals(userAnswer.Answer?.Trim(), correctAnswer?.Trim(), StringComparison.OrdinalIgnoreCase)
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
            _userAnswerService.UpdateManualMarks(userAnswerId , obtainedMarks);

            return Json(new { success = true});
        }
    }
}