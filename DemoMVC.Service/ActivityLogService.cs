using DemoMVC.Data;
using DemoMVC.Models;
using System.Linq;

namespace DemoMVC.Service
{
    public class ActivityLogService
    {
        private readonly ActivityLogProvider _activityLogProvider;
        public ActivityLogService()
        {
            _activityLogProvider = new ActivityLogProvider();
        }
        public int CreateActivityLog(ActivityLog activitylog)
        {
            return _activityLogProvider.CreateActivityLog(activitylog);
        }
        public IQueryable<ActivityLogGridModel> GetAllActivityLogs()
        {
            return _activityLogProvider.GetAllActivityLogs();
        }
        public ActivityLog GetActivityLogById(int id)
        {
            return _activityLogProvider.GetActivityLogById(id);
        }
    }
}
