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
        public void UpdateExamStatusOnEndTime()
        {
            _userExamProvider.UpdateExamStatusOnEndTime();
        }
        public void UpdateResultStatus(int userExamId)
        {
            _userExamProvider.UpdateResultStatus(userExamId);
        }
        public bool CountUserExamByExamId(int ExamId)
        {
            return _userExamProvider.CountUserExamByExamId(ExamId);
        }
        public List<UserExamGrid> GetByExamId(int ExamId)
        {
            return _userExamProvider.GetByExamId(ExamId);
        }
        public StatusSummary GetExamResultStatusSummary()
        {
            return _userExamProvider.GetExamResultStatusSummary();
        }
        public StatusSummary GetExamStatusSummary()
        {
            return _userExamProvider.GetExamStatusSummary();
        }
        public List<TimeAnalysis> GetUserExamStartTimeAnalysis()
        {
            return _userExamProvider.GetUserExamStartTimeAnalysis();
        }
        public List<DailyExamCount> GetDayWiseExamCount()
        {
            return _userExamProvider.GetDayWiseExamCount();
        }
        public List<ExamAnalysis> ExamAnalysisChart()
        {
            return _userExamProvider.ExamAnalysisChart();
        }
    }
}
