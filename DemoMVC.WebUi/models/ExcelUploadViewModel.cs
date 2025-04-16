using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemoMVC.WebUi.Models
{
    public class ExcelUploadViewModel
    {
        [Required]
        public IFormFile ExcelFile { get; set; }
    }

}