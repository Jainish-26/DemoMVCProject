using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Service
{
    public class ExamService
    {
        private readonly ExamProvider _examProvider;
        public ExamService()
        {
            _examProvider = new ExamProvider();
        }

        public List<Exams> GetAllExam()
        {
            return _examProvider.GetAllExam();
        }
        public IQueryable<ExamsGridModel> GetExamsGridModels()
        {
            return _examProvider.GetExamGridModal();
        }
        public int CreateExam(Exams exam)
        {
            return _examProvider.CreateExam(exam);
        }
        public Exams GetById(int id)
        {
            return _examProvider.GetById(id);
        }
        public int UpdateExam(Exams exam)
        {
            return _examProvider.UpdateExam(exam);
        }

        public string GetExamCode(int ExamId)
        {
            return _examProvider.GetExamCode(ExamId);
        }
        public List<Exams> CheckDuplicateExamCode(string ExamCode)
        {
            return _examProvider.CheckDuplicateExamCode(ExamCode);
        }
    }
}
