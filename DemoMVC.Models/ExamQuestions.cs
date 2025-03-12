using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class ExamQuestions
    {
        [Key]
        public int ExamQuestionId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        [ForeignKey("ExamId")]
        public virtual Exams Exams { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Questions Questions { get; set; }
        public int Marks { get; set; }
    }

    public class ExamQuestionsGridModel
    {
        public int ExamQuestionId { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string ExamCode { get; set; }
        public int Marks { get; set; }
    }
}
