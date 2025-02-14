using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DemoMVC.Models
{
    public class FormModel
    {
        public FormModel()
        {
            _ParentFormList = new List<SelectListItem>();
        }

        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Display(Name = "Navigate URL")]
        public string NavigateURL { get; set; }

        [Display(Name = "Parent form")]
        public int? ParentFormId { get; set; }

        [Required]
        [Display(Name = "Form access code")]
        //[Remote("CheckDuplicateFormAccessCode", "Form", HttpMethod = "Post", AdditionalFields = "Id")]
        public string FormAccessCode { get; set; }

        [Display(Name = "Display order")]
        public int? DisplayOrder { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get; set; }
        public bool IsDisplayMenu { get; set; }
        public bool? IsDeleted { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        public List<SelectListItem> _ParentFormList { get; set; }
    }
}
