using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class UserProfile
    {
        [Key]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        //public int DefaultPageId { get; set; }

        [NotMapped]
        public string RoleCode { get; set; }

        [NotMapped]
        public string Role { get; set; }

        //public string EmailSignature { get; set; }

        //public string Designation { get; set; }

        public string MobileNo { get; set; }

        public string PhoneNo { get; set; }
        //public int? GeneratedOTP { get; set; }
        //public DateTime? OTPExpiryDate { get; set; }
    }
    public class UserProfileGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; }
        public string RoleCode { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
