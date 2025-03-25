using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
    public class UserExamModel
    {
        public int ExamId { get; set; }
        public List<string> Email { get; set; }
    }

    public class UserExamViewModel
    {
        public int ExamId { get; set; }
        [Required]
        [Remote("CheckDuplicateUserName", "UserExam", HttpMethod = "Post")]
        public string UserName { get; set; }
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
    }
}