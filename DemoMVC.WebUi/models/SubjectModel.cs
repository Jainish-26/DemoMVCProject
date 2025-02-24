using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
    public class SubjectModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public string _subjectCode { get; set; }


        [Required]
        [Display(Name = "Subject Code")]
        [Remote("CheckDuplicateSubjectCode", "Subject", HttpMethod = "Post", AdditionalFields = "Id")]
        public string SubjectCode
        {
            get
            {
                if (string.IsNullOrEmpty(_subjectCode))
                {
                    return _subjectCode;
                }
                return _subjectCode.ToUpper();
            }
            set
            {
                _subjectCode = value;
            }
        }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}
