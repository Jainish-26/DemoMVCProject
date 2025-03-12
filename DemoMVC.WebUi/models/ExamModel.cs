using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Exam Name is required.")]
        public string ExamName { get; set; }
        [Required(ErrorMessage = "Exam Code is required.")]
        public string ExamCode { get; set; }
        [Required(ErrorMessage = "Total Marks is required.")]
        public int TotalMarks { get; set; }
        [Required(ErrorMessage = "Passing Marks is required.")]
        public int PassingMarks { get; set; }
        [Required(ErrorMessage = "Duration is required.")]
        public int DurationMin { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string ExamStatus { get; set; }
        public List<SelectListItem> _statusList { get; set; }
        public bool IsActive { get; set; } = false;
    }
}