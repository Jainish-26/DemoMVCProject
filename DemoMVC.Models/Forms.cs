using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class Forms
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NavigateURL { get; set; }
        public int? ParentFormId { get; set; }
        public string FormAccessCode { get; set; }
        public int? DisplayOrder { get; set; }
        public string Icon { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisplayMenu { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
    public class FormsGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string NavigateURL { get; set; }
        public string ParentFormName { get; set; }
        public string FormAccessCode { get; set; }
        public int? DisplayOrder { get; set; }
        public string Icon { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisplayMenu { get; set; }
        public int? ParentFormId { get; set; }
        public object UpdatedOn { get; set; }
    }
}
