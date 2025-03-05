using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoMVC.WebUi.Models
{
	public class ExamModel
	{
		public int ExamId { get; set; }
		[Required]
		public string ExamName { get; set; }
        [Required]
        public string ExamCode { get; set; }
        [Required]
        public int TotalMarks { get; set; }
        [Required]
        public int PassingMarks { get; set; }
        [Required]
        public int DurationMin { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
        public bool IsActive { get; set; } = false;
    }
}