using System;

namespace DemoMVC.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public DateTime? RecordDate { get; set; }
        public string PageName { get; set; }
        public string PageParameter { get; set; }
        public string MethodName { get; set; }
        public int? LineNumber { get; set; }
        public int? UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string BrowserNameVersion { get; set; }
        public string BrowserCapabilities { get; set; }
        public string LastFormAccessCode { get; set; }
        public bool? IsAjaxRequest { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
    public class ErrorLogGridModel
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string Source { get; set; }
        public DateTime? RecordDate { get; set; }
        public string PageName { get; set; }
        public string PageParameter { get; set; }
        public string MethodName { get; set; }
        public int? LineNumber { get; set; }
        public int? UserId { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string BrowserNameVersion { get; set; }
        public string BrowserCapabilities { get; set; }
        public string LastFormAccessCode { get; set; }
        public bool? IsAjaxRequest { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string ControllerName { get; set; }
    }
}
