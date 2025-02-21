using System;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class webpages_Roles
    {
        [Key]
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public string RoleCode { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
        public bool? IsDeleted { get; set; }
    }

    public class RolesGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string RoleCode { get; set; }
    }

    public class RoleUserCountModel
    {
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public int UserCount { get; set; }
    }

}
