using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoMVC.WebUi.Models
{
	public class UserAnswerModel
	{
		public int UserAnswerId { get; set; }
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public List<string> Answer { get; set; }
    }
}