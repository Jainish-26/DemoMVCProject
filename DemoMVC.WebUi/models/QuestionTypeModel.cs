using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
	public class QuestionTypeModel
	{
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        

        public string _questionTypeCode { get; set; }


        [Required]
        [Display(Name = "Subject Code")]
        [Remote("CheckDuplicateQuestionTypeCode", "QuestionType", HttpMethod = "Post", AdditionalFields = "Id")]
        public string QuestionTypeCode
        {
            get
            {
                if (string.IsNullOrEmpty(_questionTypeCode))
                {
                    return _questionTypeCode;
                }
                return _questionTypeCode.ToUpper();
            }
            set
            {
                _questionTypeCode = value;
            }
        }

        public int CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}