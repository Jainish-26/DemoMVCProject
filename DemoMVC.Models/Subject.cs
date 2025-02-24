using System;
using System.ComponentModel.DataAnnotations;

namespace DemoMVC.Models
{
    public class Subject
    {
        [Key]
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public bool IsActive { get; set; }
        public string SubjectCode { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }

    public class SubjectGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }
        public string SubjectCode { get; set; }
    }
}
