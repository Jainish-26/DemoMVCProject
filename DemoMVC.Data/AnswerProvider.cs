using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Data
{
    public class AnswerProvider: BaseProvider
    {
        public List<Answers> GetAllAnswers()
        {
            var data = from q in _db.Answers select q;

            return data.ToList();
        }

        public int CreateAnswer(Answers Ans)
        {
            try
            {
                _db.Answers.Add(Ans);
                _db.SaveChanges();
                return Ans.AnswerId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public Answers GetById(int id)
        {
            return _db.Answers.Find(id);
        }

        public List<Answers> GetByQuestionId(int id)
        {
            return (from ans in _db.Answers where ans.QuestionId == id select ans).ToList();
        }
        public int UpdateAnswer(Answers Ans)
        {
            try
            {
                _db.Entry(Ans).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return Ans.AnswerId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool DeleteAnswer(int id)
        {
            var answer = GetById(id);
            try
            {
                if (answer != null)
                {
                    _db.Answers.Remove(answer);
                    _db.SaveChanges();
                    return true;
                }
            }
            catch(Exception e)
            {
                throw e;
            }
            return false;
        }
    }
}
