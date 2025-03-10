using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemoMVC.WebUi.Models
{
	public class ExamQuestionModel
	{
		public ExamModel  Exam { get; set; }
        public List<QuestionSelectionModel> SelectedQuestions { get; set; }
        public List<int> UnSelectedQuestions { get; set; } = new List<int>();
    }

    public class QuestionSelectionModel
    {
        public int QuestionId { get; set; }
        public int Marks { get; set; }
    }
}