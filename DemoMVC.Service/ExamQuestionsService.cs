using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

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

        public List<ExamQuestions> GetExamQuestionsById(int ExamId)
        {
            return _examQuestionsProvider.GetExamQuestionsById(ExamId);
        }

        public bool DeleteExamQuestion(int QuestionId, int ExamId)
        {
            return _examQuestionsProvider.DeleteExamQuestion(QuestionId, ExamId);
        }

        public int GetTotalMarks(int? ExamId)
        {
            return _examQuestionsProvider.GetTotalMarks(ExamId);
        }

        public int UpdateExamQuestion(ExamQuestions examQuestions)
        {
            return _examQuestionsProvider.UpdateExamQuestion(examQuestions);
        }

        public int GetMarksByQuestionId(int QuestionId, int ExamId)
        {
            return _examQuestionsProvider.GetMarksByQuestionId(QuestionId, ExamId);
        }
    }
}
