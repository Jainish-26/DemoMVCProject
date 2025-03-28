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
        public List<UserAnswers> GetAllAnswerByUser(int userExamId)
        {
            return _userAnswerProvider.GetAllAnswerByUser(userExamId);
        }
        public void SaveOrUpdateUserAnswer(UserAnswers model)
        {
            _userAnswerProvider.SaveOrUpdateUserAnswer(model);
        }
    }
}
