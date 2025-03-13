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
        public string _ExamCode { get; set; }

        [Required]
        [Display(Name = "Exam Code")]
        [Remote("CheckDuplicateExamCode", "Exam", HttpMethod = "Post", AdditionalFields = "ExamId")]
        public string ExamCode
        {
            get
            {
                if (string.IsNullOrEmpty(_ExamCode))
                {
                    return _ExamCode;
                }
                return _ExamCode.ToUpper();
            }
            set
            {
                _ExamCode = value;
            }
        }
        [Required(ErrorMessage = "Total Marks is required.")]
        public int TotalMarks { get; set; }
        [Required(ErrorMessage = "Passing Marks is required.")]
        public int PassingMarks { get; set; }
        [Required(ErrorMessage = "Duration is required.")]
        public int DurationMin { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        [Required(ErrorMessage = "Status is required." )]
        public string ExamStatus { get; set; }
        public List<SelectListItem> _statusList { get; set; }
        public bool IsActive { get; set; } = false;
    }
}