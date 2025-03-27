using DemoMVC.Data;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Service
{
    public class UserAnswerService
    {
        private readonly UserAnswerProvider _userAnswerProvider;
        public UserAnswerService()
        {
            _userAnswerProvider = new UserAnswerProvider();
        }

        public int CreateUserAnswer(UserAnswers ans)
        {
            return _userAnswerProvider.CreateUserAnswer(ans);
        }

        public bool DeleteUserAnswers(int userExamId, int questionId , string answerText)
        {
            return _userAnswerProvider.DeleteUserAnswers(userExamId, questionId , answerText);
        }
        public UserAnswers GetExistingAnswer(int userExamId, int questionId)
        {
            return _userAnswerProvider.GetExistingAnswer(userExamId, questionId);
        }
        public List<UserAnswers> GetAllAnswerByUser(int userExamId)
        {
            return _userAnswerProvider.GetAllAnswerByUser(userExamId);
        }
        public int UpdateAnswer(UserAnswers ans)
        {
            return _userAnswerProvider.UpdateAnswer(ans);
        }
        public void SaveOrUpdateUserAnswer(UserAnswers model)
        {
            _userAnswerProvider.SaveOrUpdateUserAnswer(model);
        }
    }
}
