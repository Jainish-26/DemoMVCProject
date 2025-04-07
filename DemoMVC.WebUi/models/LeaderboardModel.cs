using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
	public class LeaderboardModel
	{
        public LeaderboardModel()
        {
            _ExamList = new List<SelectListItem>();
        }
        public int ExamId { get; set; }
        public string Exams { get; set; }
        public List<SelectListItem> _ExamList { get; set; }
    }
}