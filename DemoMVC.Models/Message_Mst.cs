using System;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Message_Mst
    {
        public int Id { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
    public class MessageGridModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Message is required")]
        public string Message { get; set; }
        public bool IsActive { get; set; }
        public string Comment { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
