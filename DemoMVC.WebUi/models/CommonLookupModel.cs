using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
    public class CommonLookupModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Type is required.")]
        public string Type { get; set; }
        [Required(ErrorMessage = "Code is required.")]
        [Remote("CheckDuplicateCode", "CommonLookup", HttpMethod = "POST", AdditionalFields = "Id,Type")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is required.")]
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
}