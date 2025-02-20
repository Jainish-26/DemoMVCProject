using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class FormRoleMapping
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public bool FullRights { get; set; }
        public bool AllowMenu { get; set; }
        public bool AllowView { get; set; }
        public bool AllowInsert { get; set; }
        public bool AllowUpdate { get; set; }
        public bool AllowDelete { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsActive { get; set; }
        public int MenuId { get; set; }
        [NotMapped]
        public string FormName { get; set; }
        [NotMapped]
        public string RoleName { get; set; }
    }
}
