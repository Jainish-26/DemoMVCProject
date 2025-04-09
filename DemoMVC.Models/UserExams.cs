using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class UserExams
    {
        [Key]
        public int UserExamId { get; set; }

        public int UserId { get; set; }

        public int ExamId { get; set; }

        public string UserToken { get; set; }

        public DateTime ExpiryDate { get; set; }

        public string ExamStatus { get; set; }

        public double? Result { get; set; }

        public string ResultStatus { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("UserId")]
        public virtual UserProfile User { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exams Exam { get; set; }
    }

    public class UserExamGrid
    {
        public int UserExamId { get; set; }

        public int UserId { get; set; }
        public string Email { get; set; }

        public int ExamId { get; set; }
        public string ExamName { get; set; }

        public string UserToken { get; set; }

        public DateTime ExpiryDate { get; set; }
        public string ExamStatus { get; set; }
        public string ExamBadgeCode { get; set; }

        public double? Result { get; set; }

        public string ResultStatus { get; set; }
        public string ResultBadgeCode { get; set; }
        public DateTime? StartTime { get; set; }
    }
    public class StatusSummary
    {
        public int Pending { get; set; }
        public int Ongoing { get; set; }
        public int InReview { get; set; }
        public int Evaluated { get; set; }
        public int Completed { get; set; }
    }

    public class TimeAnalysis
    {
        public int Hour { get; set; }
        public int Count { get; set; }
    }
    public class DailyExamCount
    {
        public string Date { get; set; }
        public int Count { get; set; }
    }

    public class ExamAnalysis
    {
        public string ExamName { get; set; }
        public int Pending { get; set; }
        public int Ongoing { get; set; }
        public int InReview { get; set; }
        public int Evaluated { get; set; }
        public int Completed { get; set; }
    }
}
