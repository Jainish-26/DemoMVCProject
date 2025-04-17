using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

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

    public class ExamQuestionViewModel
    {
        public ExamModel Exam { get; set; }
        public List<QuestionAndAnswerModel> Questions { get; set; }
    }
    public class PracticeTestModel
    {
        public PracticeTestModel()
        {
            _subjectList = new List<SelectListItem>();
            _difficultyList = new List<SelectListItem>();
        }
        public string Subject { get; set; }
        [Required]
        public int SubjectId { get; set; }
        public List<SelectListItem> _subjectList { get; set; }
        [Required]
        public string Difficulty { get; set; }
        public List<SelectListItem> _difficultyList { get; set; }
        [Range(20, int.MaxValue, ErrorMessage = "Minimum marks should be 20")]
        public int Marks { get; set; }
        [Range(10, int.MaxValue, ErrorMessage = "Minimum duration should be 10 minute")]
        public int DurationMin { get; set; }
    }
}