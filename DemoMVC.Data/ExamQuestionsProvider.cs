using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Data
{
    public class ExamQuestionsProvider : BaseProvider
    {
        private readonly QuestionProvider _questionProvider;
        private readonly ExamProvider _examProvider;

        public ExamQuestionsProvider()
        {
            _questionProvider = new QuestionProvider();
            _examProvider = new ExamProvider();
        }
        public IQueryable<QuestionGridModel> GetExamQuestionsGrid(int? examId)
        {

            var questions = (from eq in _db.ExamQuestions
                             join q in _db.Questions on eq.QuestionId equals q.QuestionId
                             where eq.ExamId == examId
                             select new QuestionGridModel
                             {
                                 QuestionId = q.QuestionId,
                                 QuestionText = q.QuestionText,
                                 Subject=q.Subject.SubjectName,
                                 Type = q.QuestionType.QuestionTypeName,
                                 Marks = eq.Marks
                             }).AsQueryable();

            return questions;
        }

        public IQueryable<QuestionGridModel> GetUnassignedQuestions(int? examId)
        {
            var unassignedQuestions = (from q in _db.Questions
                                       where q.IsActive == true &&
                                             ! _db.ExamQuestions
                                                .Where(eq => eq.ExamId == examId)
                                                .Select(eq => eq.QuestionId)
                                                .Contains(q.QuestionId)
                                       select new QuestionGridModel
                                       {
                                           QuestionId = q.QuestionId,
                                           QuestionText = q.QuestionText,
                                           Subject = q.Subject.SubjectName,
                                           Type = q.QuestionType.QuestionTypeName,
                                           Marks = q.Marks
                                       }).AsQueryable();

            return unassignedQuestions;
        }


        public int CraeteExamQuestion(ExamQuestions model)
        {
            try
            {
                _db.ExamQuestions.Add(model);
                _db.SaveChanges();
                return model.ExamQuestionId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int GetTotalMarks(int? ExamId)
        {
            return (from eq in _db.ExamQuestions where eq.ExamId == ExamId select eq).Sum(m => m.Marks);
        }
        public List<ExamQuestions> GetExamQuestionsById(int ExamId)
        {
            return (from i in _db.ExamQuestions where i.ExamId == ExamId select i).ToList();
        }

        public ExamQuestions GetByExamAndQuestionId(int QuestionId ,int ExamId)
        {
            return (from eq in _db.ExamQuestions where eq.QuestionId == QuestionId && eq.ExamId == ExamId select eq).FirstOrDefault();
        }

        public int UpdateExamQuestion(ExamQuestions examQuestions)
        {
            try
            {
                _db.Entry(examQuestions).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return examQuestions.ExamQuestionId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool DeleteExamQuestion(int QuestionId, int ExamId)
        {
            var examQuestion = GetByExamAndQuestionId(QuestionId, ExamId);
            try
            {
                if (examQuestion != null)
                {
                    _db.ExamQuestions.Remove(examQuestion);
                    _db.SaveChanges();
                    return true;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return false;
        }
    }
}
