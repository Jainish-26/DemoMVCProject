﻿namespace DemoMVC.WebUi.Models
{
    public class UserAnswerModel
    {
        public int UserAnswerId { get; set; }
        public int UserExamId { get; set; }
        public int QuestionId { get; set; }
        public string Answer { get; set; }
        public bool IsVisited { get; set; }
        public int ObtainedMarks { get; set; } = 0;
        public bool IsEvaluate { get; set; }
    }
}