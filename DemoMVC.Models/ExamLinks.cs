using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DemoMVC.Models
{
    public class ExamLinks
    {
        [Key]
        public int ExamLinkId { get; set; }

        public int ExamId { get; set; }

        [Required]
        [StringLength(500)]
        public string Link { get; set; }

        [Required]
        public int CreatedBy { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public DateTime ExpiryTime { get; set; }

        public bool IsActive { get; set; } = true;

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("ExamId")]
        public virtual Exams Exam { get; set; }
    }
}
