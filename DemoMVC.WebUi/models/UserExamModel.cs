using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoMVC.WebUi.Models
{
	public class UserExamModel
	{
		public int ExamId { get; set; }
		public List<string> Email { get; set; }
	}
}