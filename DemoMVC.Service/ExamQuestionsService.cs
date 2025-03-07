using DemoMVC.Data;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Service
{
    public class ExamQuestionsService
    {
        private readonly ExamQuestionsProvider _examQuestionsProvider;

        public ExamQuestionsService()
        {
            _examQuestionsProvider = new ExamQuestionsProvider();
        }

        public int CraeteExamQuestion(ExamQuestions model)
        {
            return _examQuestionsProvider.CraeteExamQuestion(model);
        }

        public IQueryable<QuestionGridModel> GetExamQuestionsGrid(int? examId)
        {
            return _examQuestionsProvider.GetExamQuestionsGrid(examId);
        }

        public IQueryable<QuestionGridModel> GetUnassignedQuestions(int? examId)
        {
            return _examQuestionsProvider.GetUnassignedQuestions(examId);
        }

        public ExamQuestions GetExamQuestionsById(int ExamId)
        {
            return _examQuestionsProvider.GetExamQuestionsById(ExamId);
        }
    }
}
