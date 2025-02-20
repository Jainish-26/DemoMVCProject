using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DemoMVC.Models
{
    public class CommonLookup
    {
        public int Id { get; set; }
        public string Type { get; set; }
        [Remote("CheckduplicateCode", "CommonLookup", HttpMethod = "POST", AdditionalFields = "Id")]
        public string Code { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
        public string BadgeCode { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
    public class CommonLookUpGridModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
    }
}
