using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class Answers
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required]
        public int AnswerId { get; set; }
        [Required]
        public int QuestionId { get; set; }
        [Required]
        public string AnswerText { get; set; }
        [Required]
        public bool IsCorrect { get; set; } = false;
        [Required]
        public int CreatedBy { get; set; }
        [Required]
        public DateTime CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Questions Question { get; set; }
    }
}
