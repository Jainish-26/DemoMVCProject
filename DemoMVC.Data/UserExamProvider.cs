using DemoMVC.Helper;
using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DemoMVC.Data
{
    public class UserExamProvider : BaseProvider
    {
        public UserExamProvider()
        {

        }
        public List<UserExams> GetAllUserExams()
        {
            return (from e in _db.UserExams select e).ToList();
        }

        public int CreateUserExam(UserExams userExam)
        {
            try
            {
                _db.UserExams.Add(userExam);
                _db.SaveChanges();
                return userExam.UserExamId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public int UpdateUserExam(UserExams userExam)
        {
            try
            {
                _db.Entry(userExam).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return userExam.UserExamId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IQueryable<UserExamGrid> GetUserExamGrids()
        {
            return (from e in _db.UserExams
                    select new UserExamGrid
                    {
                        UserExamId = e.UserExamId,
                        Email = (from u in _db.UserProfile where e.UserId == u.UserId select u.Email).FirstOrDefault(),
                        ExamName = (from i in _db.Exams where e.ExamId == i.ExamId select i.ExamName).FirstOrDefault(),
                        ExamBadgeCode = (from code in _db.CommonLookup where e.ExamStatus == code.Code select code.BadgeCode).FirstOrDefault(),
                        UserToken = e.UserToken,
                        ExamStatus = e.ExamStatus,
                        Result = e.Result,
                        ResultStatus = e.ResultStatus,
                        ResultBadgeCode = (from code in _db.CommonLookup where e.ResultStatus == code.Code select code.BadgeCode).FirstOrDefault()
                    }).Distinct().AsQueryable();
        }

        public UserExams GetByUserExamId(int UserExamId)
        {
            return (from i in _db.UserExams where i.UserExamId == UserExamId select i).FirstOrDefault();
        }

        public UserExams GetByUserIdAndExamId(int ExamId, int UserId)
        {
            return (from i in _db.UserExams where ExamId == i.ExamId && UserId == i.UserId select i).FirstOrDefault();
        }

        public UserExams GetByUserToken(string userToken)
        {
            return (from i in _db.UserExams where i.UserToken == userToken select i).FirstOrDefault();
        }

        public void UpdateExamStatusOnEndTime()
        {
            try
            {
                var expiredDate = _db.UserExams.Where(e => e.EndTime <= DateTime.UtcNow && e.ExamStatus == Constants.UserExamStatus.ONGOING)
                                    .ToList();
                foreach (var exam in expiredDate)
                {
                    exam.ExamStatus = Constants.UserExamStatus.COMPLETED;
                    exam.ExpiryDate = DateTime.UtcNow;
                    exam.ResultStatus = Constants.ResultStatus.PENDING;
                }
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void UpdateResultStatus(int userExamId)
        {
            try
            {
                var userExam = GetByUserExamId(userExamId);
                userExam.ResultStatus = Constants.ResultStatus.INREVIEW;
                _db.SaveChanges();
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public bool CountUserExamByExamId(int ExamId)
        {
            try
            {
                int examCount = (from i in _db.UserExams where i.ExamId == ExamId select i).Count();
                if (examCount == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public List<UserExamGrid> GetByExamId(int ExamId)
        {
            var query = (from a in _db.UserAnswers
                         join e in _db.UserExams on a.UserExamId equals e.UserExamId
                         join u in _db.UserProfile on e.UserId equals u.UserId
                         join i in _db.Exams on e.ExamId equals i.ExamId
                         where e.ExamId == ExamId && e.ResultStatus == Constants.ResultStatus.EVALUATED
                         select new UserExamGrid
                         {
                             UserExamId = e.UserExamId,
                             Email = u.Email,
                             ExamName = i.ExamName,
                             Result = e.Result,
                             StartTime = e.StartTime
                         }).Distinct().OrderByDescending(a => a.Result);
            return query.ToList();
        }
    }
}
