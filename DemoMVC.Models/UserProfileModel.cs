using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DemoMVC.Models
{
    public class UserProfileModel
    {
        public UserProfileModel()
        {
            _RoleList = new List<SelectListItem>();
            //_DefaultFormList = new List<SelectListItem>();
            //_OrganizationList = new List<SelectListItem>();
        }
        public int UserId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        //[Remote("CheckDuplicateUserName", "UserProfile", HttpMethod = "Post", AdditionalFields = "UserId")]
        public string UserName { get; set; }

        [Required]
        [RegularExpression(@"^[a-zA-Z ]+$", ErrorMessage = "Only Letters and Spaces are allowed")]
        public string Name { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address.")]
        //[Remote("CheckDuplicateUserEmail", "UserProfile", HttpMethod = "Post", AdditionalFields = "UserId")]
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string Role { get; set; }
        [Required]
        public string Password { get; set; }
        //public int DefaultPageId { get; set; }
        //public List<Organization_Mst> OrgName{ get; set; } 
        //[Required(ErrorMessage = "Organization is required")]
        //public int[] OrganizationId { get; set; }
        //public List<SelectListItem> _OrganizationList { get; set; }
        public List<SelectListItem> _RoleList { get; set; }
        //[Display(Name = "Default Page")]
        //public int? DefaultFormId { get; set; }
        //public List<SelectListItem> _DefaultFormList { get; set; }
        //[Display(Name = "Email Signature")]
        //public string EmailSignature { get; set; }
        //public string Designation { get; set; }
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string MobileNo { get; set; }
        //[RegularExpression(@"^([0-9])$", ErrorMessage = "Invalid Phone Number.")]
        public string PhoneNo { get; set; }
    }
    }
