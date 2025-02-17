using DemoMVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Data
{
    public class ActivityLogProvider : BaseProvider
    {
        public ActivityLogProvider()
        {

        }
        public int CreateActivityLog(ActivityLog activitylog)
        {
            try
            {
                _db.ActivityLog.Add(activitylog);
                _db.SaveChanges();
                return activitylog.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public IQueryable<ActivityLogGridModel> GetAllActivityLogs()
        {
            return _db.ActivityLog
                    .Select(activitylog => new ActivityLogGridModel()
                    {
                        Id = activitylog.Id,
                        PageUrl = activitylog.PageUrl,
                        IPAddress = activitylog.IPAddress,
                        LogDate = activitylog.LogDate,
                        ActionName = activitylog.ActionName,
                        ControllerName = activitylog.ControllerName
                    }).AsQueryable();
        }
        public ActivityLog GetActivityLogById(int id)
        {
            return _db.ActivityLog.Include("UserProfile").Where(x => x.Id == id).FirstOrDefault();
        }
    }
}
