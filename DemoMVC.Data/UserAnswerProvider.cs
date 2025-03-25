using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public List<UserAnswers> GetAnswers(int userExamId, int questionId)
        {
            try
            {
                var answers = (from i in _db.UserAnswers where i.UserExamId == userExamId && i.QuestionId == questionId select i);
                return answers.ToList();
            }catch(Exception e)
            {
                throw e;
            }
        }
    }
}
