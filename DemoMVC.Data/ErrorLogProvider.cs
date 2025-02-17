using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Data
{
    public class ErrorLogProvider : BaseProvider
    {
        public int CreateErrorLog(ErrorLog errorLog)
        {
            try
            {
                _db.ErrorLog.Add(errorLog);
                _db.SaveChanges();
                return errorLog.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
        public ErrorLog GetErrorLogById(int Id)
        {
            return _db.ErrorLog.Find(Id);
        }
        public IQueryable<ErrorLogGridModel> GetAllErrorLogs()
        {
            return _db.ErrorLog
                   .Select(errorlog => new ErrorLogGridModel()
                   {
                       Id = errorlog.Id,
                       PageName = errorlog.PageName,
                       LineNumber = errorlog.LineNumber,
                       RecordDate = errorlog.RecordDate,
                       MethodName = errorlog.MethodName
                   }).AsQueryable();
        }
    }
}
