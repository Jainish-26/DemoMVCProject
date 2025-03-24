using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoMVC.WebUi.Models
{
	public class StartTestModel
	{
        public int ExamId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string UserEmail { get; set; }
        public string ExamName { get; set; }
        public string UserToken { get; set;  }
        public int TotalMarks { get; set; }
        public int PassingMarks {get; set;}
        public int Duration { get; set; }
    }
}