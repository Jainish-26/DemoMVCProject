using DemoMVC.Data;
using DemoMVC.Models;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Service
{
    public class UserExamService
    {
        private readonly UserExamProvider _userExamProvider;
        public UserExamService()
        {
            _userExamProvider = new UserExamProvider();
        }
        public List<UserExams> GetAllUserExams()
        {
            return _userExamProvider.GetAllUserExams();
        }
        public int CreateUserExam(UserExams userExam)
        {
            return _userExamProvider.CreateUserExam(userExam);
        }
        public IQueryable<UserExamGrid> GetUserExamGrids()
        {
            return _userExamProvider.GetUserExamGrids();
        }
        public UserExams GetByUserExamId(int UserExamId)
        {
            return _userExamProvider.GetByUserExamId(UserExamId);
        }
        public UserExams GetByUserIdAndExamId(int ExamId, int UserId)
        {
            return _userExamProvider.GetByUserIdAndExamId(ExamId, UserId);
        }
        public int UpdateUserExam(UserExams userExam)
        {
            return _userExamProvider.UpdateUserExam(userExam);
        }
        public UserExams GetByUserToken(string userToken)
        {
            return _userExamProvider.GetByUserToken(userToken);
        }
    }
}
