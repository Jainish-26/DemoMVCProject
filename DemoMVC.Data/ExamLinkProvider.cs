using DemoMVC.Models;
using System;
using System.Linq;

namespace DemoMVC.Data
{
    public class ExamLinkProvider : BaseProvider
    {
        public ExamLinks GetExamLinkByExamId(int ExamId)
        {
            return (from link in _db.ExamLinks where link.ExamId == ExamId select link).FirstOrDefault();
        }
        public int CreateExamLink(ExamLinks link)
        {
            try
            {
                _db.ExamLinks.Add(link);
                _db.SaveChanges();

                return link.ExamLinkId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        public int UpdateExamLink(ExamLinks link)
        {
            try
            {
                _db.Entry(link).State = System.Data.Entity.EntityState.Modified;
                _db.SaveChanges();
                return link.ExamLinkId;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
