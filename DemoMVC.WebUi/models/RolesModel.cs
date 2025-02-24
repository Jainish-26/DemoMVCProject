using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace DemoMVC.WebUi.Models
{
    public class RolesModel
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public string _RoleCode { get; set; }


        [Required]
        [Display(Name = "Role Code")]
        [Remote("CheckDuplicateRoleCode", "Roles", HttpMethod = "Post", AdditionalFields = "Id")]
        //D:\ASP.NET\DemoMVC\DemoMVC.WebUi\
        public string RoleCode
        {
            get
            {
                if (string.IsNullOrEmpty(_RoleCode))
                {
                    return _RoleCode;
                }
                return _RoleCode.ToUpper();
            }
            set
            {
                _RoleCode = value;
            }
        }

        public int UpdatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
    }

}