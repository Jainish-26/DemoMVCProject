using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class QuestionMedia
    {
        [Key]
        public int Id { get; set; }
        public string MediaName { get; set; }
        public string MediaType { get; set; }
        public int QuestionId { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Questions Questions { get; set; }
    }
}
