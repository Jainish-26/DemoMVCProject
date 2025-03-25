using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
    public class StartTestModel
    {
        public int ExamId { get; set; }
        [Required]
        [Remote("CheckDuplicateUserName", "UserExam", HttpMethod = "Post")]
        public string UserName { get; set; }
        [Required]
        public string Name { get; set; }
        public string Email { get; set; }
        public string ExamName { get; set; }
        public string UserToken { get; set; }
        public int TotalMarks { get; set; }
        public int PassingMarks { get; set; }
        public int Duration { get; set; }
    }
}