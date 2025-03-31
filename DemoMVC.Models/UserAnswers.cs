using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class UserAnswers
    {
        [Key]
        public int UserAnswerId { get; set; }

        public int UserExamId { get; set; }

        public int QuestionId { get; set; }

        public string AnswerText { get; set; }

        public int ObtainedMarks { get; set; } = 0;

        [ForeignKey("UserExamId")]
        public virtual UserExams UserExam { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Questions Question { get; set; }

        public bool IsVisited { get; set; }
        public bool IsEvaluate { get; set; }
    }
}
