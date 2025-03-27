using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace DemoMVC.Data
{
    public class UserAnswerProvider : BaseProvider
    {
        public int CreateUserAnswer(UserAnswers ans)
        {
            try
            {
                _db.UserAnswers.Add(ans);
                _db.SaveChanges();
                return ans.UserAnswerId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool DeleteUserAnswers(int userExamId, int questionId , string answerText)
        {
            try
            {
                // Fetch the specific answer to delete
                var userAnswer = _db.UserAnswers
                    .FirstOrDefault(a => a.UserExamId == userExamId
                                      && a.QuestionId == questionId
                                      && a.AnswerText == answerText);

                if (userAnswer != null)
                {
                    _db.UserAnswers.Remove(userAnswer); 
                    _db.SaveChanges(); 
                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public UserAnswers GetExistingAnswer(int userExamId, int questionId)
        {
            try
            {
                return _db.UserAnswers.FirstOrDefault(i => i.UserExamId == userExamId && i.QuestionId == questionId);
            }
            catch (Exception e)
            {
                throw new Exception("Error retrieving existing answer", e);
            }
        }

        public List<UserAnswers> GetAllAnswerByUser(int userExamId)
        {
            try
            {
                var allAnswer = (from i in _db.UserAnswers where userExamId == i.UserExamId select i).ToList();
                return allAnswer;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public int UpdateAnswer(UserAnswers ans)
        {
            try
            {
                _db.Entry(ans).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return ans.UserAnswerId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveOrUpdateUserAnswer(UserAnswers model)
        {
            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    var existingAnswer = _db.UserAnswers
                        .FirstOrDefault(a => a.UserExamId == model.UserExamId && a.QuestionId == model.QuestionId);

                    if (existingAnswer != null)
                    {
                        if (existingAnswer.AnswerText != model.AnswerText || !existingAnswer.IsVisited)
                        {
                            existingAnswer.AnswerText = model.AnswerText;
                            existingAnswer.IsVisited = true;
                            _db.Entry(existingAnswer).State = EntityState.Modified;
                            _db.SaveChanges();
                        }
                    }
                    else
                    {
                        

                        _db.UserAnswers.Add(model);
                    }

                    _db.SaveChanges();
                    transaction.Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    transaction.Rollback();
                    throw new Exception("Concurrency error: " + ex.Message);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception("Error saving answer: " + ex.Message);
                }
            }
        }

    }
}
