using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class QuestionType
    {
        public int QuestionTypeId { get; set; }         
        public string QuestionTypeName { get; set; }      
        public string QuestionTypeCode { get; set; }      
        public int? CreatedBy { get; set; }             
        public DateTime CreatedOn { get; set; }          
        public int? UpdatedBy { get; set; }            
        public DateTime? UpdatedOn { get; set; }          
    }

    public class QuestionTypesGridModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string QuestionTypeCode { get; set; }
    }
}
