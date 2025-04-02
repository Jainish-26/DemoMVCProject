using System.Collections.Generic;

namespace DemoMVC.WebUi.Models
{
    public class UserExamCheckingModel
    {
        public int UserExamId { get; set; }
        public ExamModel Exam { get; set; }
        public List<QuestionAndAnswerModel> Questions { get; set; }
        public List<UserAnswerModel> UserAnswers { get; set; }
    }
}