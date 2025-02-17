using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class ActivityLog
    {
        public int Id { get; set; }
        public string FullMessage { get; set; }
        public string PageUrl { get; set; }
        public string IPAddress { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string BrowserName { get; set; }
        public int? UserId { get; set; }
        public DateTime? LogDate { get; set; }
        [ForeignKey("UserId")]
        public UserProfile UserProfile { get; set; }
        [NotMapped]
        public string UserName { get; set; }
    }
    public class ActivityLogGridModel
    {
        public int Id { get; set; }
        public string FullMessage { get; set; }
        public string PageUrl { get; set; }
        public string IPAddress { get; set; }
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string BrowserName { get; set; }
        public int? UserId { get; set; }
        public DateTime? LogDate { get; set; }
        public string UserName { get; set; }
    }
}
