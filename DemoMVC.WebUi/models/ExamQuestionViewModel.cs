using System.Collections.Generic;

namespace DemoMVC.WebUi.Models
{
    public class ExamQuestionViewModel
    {
        public ExamModel Exam { get; set; }
        public List<QuestionAndAnswerModel> Questions { get; set; }
    }
}