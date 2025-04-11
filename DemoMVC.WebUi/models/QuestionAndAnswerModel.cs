using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
    public class QuestionAndAnswerModel
    {
        public QuestionAndAnswerModel()
        {
            _questionTypeList = new List<SelectListItem>();
            _subjectList = new List<SelectListItem>();
            _difficultyList = new List<SelectListItem>();
        }

        public int QuestionId { get; set; }
        [Required(ErrorMessage = "Question text is required.")]
        public string QuestionText { get; set; }
        public List<string> QuestionImage { get; set; }
        [Required(ErrorMessage = "Marks are required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Marks must be a positive number.")]
        public int Marks { get; set; }
        [Required]
        public bool IsActive { get; set; } = false;
        [Required(ErrorMessage = "Please select a question type.")]
        public int QuestionTypeId { get; set; }
        [Required(ErrorMessage = "Please select a subject.")]
        public int SubjectId { get; set; }
        [Required(ErrorMessage = "Please select a difficulty level.")]
        public string Difficulty { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public bool IsDelete { get; set; } = false;
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }

        public List<SelectListItem> _questionTypeList { get; set; }
        public List<SelectListItem> _subjectList { get; set; }
        public List<SelectListItem> _difficultyList { get; set; }
        public List<AnswerViewModel> Answers { get; set; } = new List<AnswerViewModel>();

        public string QuestionType { get; set; }
        public string Subject { get; set; }
    }

    public class AnswerViewModel
    {
        public int AnswerId { get; set; }
        public int QuestionId { get; set; }
        [Required(ErrorMessage = "Please Enter Ans.")]
        public string AnswerText { get; set; }
        public bool IsCorrect { get; set; } = false;
        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}