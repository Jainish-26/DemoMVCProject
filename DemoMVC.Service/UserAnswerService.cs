using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;

namespace DemoMVC.Service
{
    public class UserAnswerService
    {
        private readonly UserAnswerProvider _userAnswerProvider;
        public UserAnswerService()
        {
            _userAnswerProvider = new UserAnswerProvider();
        }
        public List<UserAnswers> GetAllAnswerByUser(int userExamId)
        {
            return _userAnswerProvider.GetAllAnswerByUser(userExamId);
        }
        public int CreateUserAnswer(UserAnswers ans)
        {
            return _userAnswerProvider.CreateUserAnswer(ans);
        }
        public void SaveOrUpdateUserAnswer(UserAnswers model)
        {
            _userAnswerProvider.SaveOrUpdateUserAnswer(model);
        }
        public void UpdateManualMarks(int userAnswerId, int obtainedMarks)
        {
            _userAnswerProvider.UpdateManualMarks(userAnswerId, obtainedMarks);
        }
        public UserAnswers GetExistingAnswer(int userExamId, int questionId)
        {
            return _userAnswerProvider.GetExistingAnswer(userExamId, questionId);
        }
        public int UpdateAnswer(UserAnswers ans)
        {
            return _userAnswerProvider.UpdateAnswer(ans);
        }
    }
}
