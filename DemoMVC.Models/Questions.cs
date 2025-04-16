using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class Questions
    {
        [Key]
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public string QuestionImage { get; set; }
        public int Marks { get; set; }
        public bool IsActive { get; set; } = false;
        public string Difficulty { get; set; }
        public int QuestionTypeId { get; set; }
        public int SubjectId { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDelete { get; set; } = false;
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        [ForeignKey("QuestionTypeId")]
        public virtual QuestionType QuestionType { get; set; }

        [ForeignKey("SubjectId")]
        public virtual Subject Subject { get; set; }
    }

    public class QuestionGridModel
    {

        public int QuestionId { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string QuestionText { get; set; }
        public string BadgeCode { get; set; }
        public string QuestionImage { get; set; }
        public int Marks { get; set; }
        public bool IsActive { get; set; } = false;
        public string Difficulty { get; set; }
    }
    public class QuestionAnswerExport
    {
        public string QuestionText { get; set; }
        public decimal Marks { get; set; }
        public string QuestionType { get; set; }
        public string Subject { get; set; }
        public string Difficulty { get; set; }
        public string AnswerJson { get; set;  }
    }
}
