using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class webpages_UsersInRoles
    {
        [Key]
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
