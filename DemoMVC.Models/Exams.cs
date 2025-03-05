using Microsoft.SqlServer.Server;
using System;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Exams
    {
        [Key]
        public int ExamId { get; set; }

        [Required]
        [StringLength(255)]
        public string ExamName { get; set; }

        [Required]
        public int TotalMarks { get; set; }

        [Required]
        public int DurationMin { get; set; } 

        [Required]
        public int PassingMarks { get; set; }

        [Required]
        [StringLength(50)]
        public string ExamCode { get; set; }

        [Required]
        [StringLength(50)]
        public string ExamStatus { get; set; } 
        public DateTime? StartTime { get; set; }
        public bool IsActive { get; set; }

        public DateTime? EndTime { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }

    public class ExamsGridModel
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; }
        public string ExamCode { get; set; }
        public string ExamStatus { get; set; }
        public int TotalMarks { get; set; }
        public bool IsActive { get; set; }
        public int DurationMin { get; set; }
    }
}
