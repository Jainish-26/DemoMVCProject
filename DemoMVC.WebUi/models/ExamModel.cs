using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
	public class ExamModel
	{
        public ExamModel()
        {
            _statusList = new List<SelectListItem>();
        }
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
        public string ExamStatus { get; set; }
        public List<SelectListItem> _statusList { get; set; }
        public bool IsActive { get; set; } = false;
    }
}