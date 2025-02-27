using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Data
{
    public class QuestionProvider :BaseProvider
    {
        private readonly AnswerProvider _answerProvider;

        public QuestionProvider()
        {
            _answerProvider = new AnswerProvider();
        }
        public List<Questions> GetAllQuestions()
        {
            var data = from q in _db.Questions where q.IsActive == true select q;

            return data.ToList();
        }

        public IQueryable<QuestionGridModel> GetAllQuestionsGridModel()
        {
            return (from q in _db.Questions
                    where q.IsActive == true
                    select new QuestionGridModel
                    {
                        QuestionId = q.QuestionId,
                        Type = q.QuestionType.QuestionTypeName,
                        Subject = q.Subject.SubjectName,
                        QuestionText = q.QuestionText,
                        QuestionImage = q.QuestionImage,
                        BadgeCode = (from difficulty in _db.CommonLookup where q.Difficulty == difficulty.Code select difficulty.BadgeCode).FirstOrDefault(), 
                        Marks = q.Marks,
                        Difficulty = q.Difficulty,
                        IsActive = q.IsActive,

                    }).AsQueryable();
        }

        public int CreateQuestion(Questions questiion)
        {
            try
            {
                _db.Questions.Add(questiion);
                _db.SaveChanges();
                return questiion.QuestionId;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public Questions GetById(int id)
        {
            return _db.Questions.Find(id);
        }

        public int UpdateQuestions(Questions Que)
        {
            try
            {
                _db.Entry(Que).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return Que.QuestionId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteQuestion(int id)
        {
            var Que = GetById(id);

            if (Que != null)
            {
                var ans = _answerProvider.GetByQuestionId(id);

                if (ans == null)
                {
                    _db.Questions.Remove(Que);
                    _db.SaveChanges();
                    return true;
                }
                else
                {
                    foreach(var a in ans)
                    {
                        bool deleted = _answerProvider.DeleteAnswer(a.AnswerId);

                        if (deleted)
                        {
                            continue;
                        }
                        else
                        {
                            return false;
                        }
                    }

                    _db.Questions.Remove(Que);
                    _db.SaveChanges();
                    return true;
                }
            }

            return false;
        }
    }
}
