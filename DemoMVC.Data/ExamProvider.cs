using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Data
{
    public class ExamProvider : BaseProvider
    {
        private readonly QuestionProvider _questionProvider;
        public ExamProvider()
        {
            _questionProvider = new QuestionProvider();
        }
        public List<Exams> GetAllExam()
        {
            var data = (from e in _db.Exams where e.IsActive == true select e).ToList();

            return data;
        }

        public IQueryable<ExamsGridModel> GetExamGridModal()
        {
            return (from e in _db.Exams
                    where e.IsActive == true
                    select new ExamsGridModel
                    {
                        ExamId = e.ExamId,
                        ExamName = e.ExamName,
                        ExamCode = e.ExamCode,
                        BadgeCode = (from code in _db.CommonLookup where e.ExamStatus == code.Code select code.BadgeCode).FirstOrDefault(),
                        ExamStatus = e.ExamStatus,
                        TotalMarks = e.TotalMarks,
                        DurationMin = e.DurationMin,
                        IsActive = e.IsActive
                    }).AsQueryable();
        }

        public int CreateExam(Exams exam)
        {
            try
            {
                _db.Exams.Add(exam);
                _db.SaveChanges();
                return exam.ExamId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Exams GetById(int id)
        {
            return _db.Exams.Find(id);
        }

        public string GetExamCode(int id)
        {
            return (from e in _db.Exams where e.ExamId == id select e.ExamCode).FirstOrDefault();
        }

        public int UpdateExam(Exams exam)
        {
            try
            {
                _db.Entry(exam).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return exam.ExamId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public List<Exams> CheckDuplicateExamCode(string ExamCode)
        {
            var getExamDetails = (from e in _db.Exams
                                  where e.ExamCode.ToUpper().Trim() == ExamCode.ToUpper().Trim()
                                  select e).ToList();
            return getExamDetails;
        }
    }
}
