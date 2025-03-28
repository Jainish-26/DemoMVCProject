using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoMVC.WebUi.Models
{
	public class UserExamChekingModel
	{
        public ExamModel Exam { get; set; }
        public List<QuestionAndAnswerModel> Questions { get; set; }
        public List<UserAnswerModel> UserAnswers { get; set; }
    }
}