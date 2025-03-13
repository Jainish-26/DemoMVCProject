using System.Collections.Generic;

namespace DemoMVC.WebUi.Models
{
    public class ExamQuestionModel
    {
        public ExamModel Exam { get; set; }
        public List<QuestionSelectionModel> SelectedQuestions { get; set; }
    }

    public class QuestionSelectionModel
    {
        public int QuestionId { get; set; }
        public int Marks { get; set; }
    }
}