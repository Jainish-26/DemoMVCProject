using DemoMVC.Data;
using DemoMVC.Models;
using System.Linq;

namespace DemoMVC.Service
{
    public class ErrorLogService
    {
        private readonly ErrorLogProvider _errorLogProvider;
        public ErrorLogService()
        {
            _errorLogProvider = new ErrorLogProvider();
        }
        public int CreateErrorLog(ErrorLog errorLog)
        {
            return _errorLogProvider.CreateErrorLog(errorLog);
        }
        public ErrorLog GetErrorLogById(int Id)
        {
            return _errorLogProvider.GetErrorLogById(Id);
        }
        public IQueryable<ErrorLogGridModel> GetAllErrorLogs()
        {
            return _errorLogProvider.GetAllErrorLogs();
        }
    }
}
