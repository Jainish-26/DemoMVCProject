using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoMVC.Models
{
    public class MenuVW
    {
        public int Id { get; set; }
        public string FormAccessCode { get; set; }
        public string Name { get; set; }
        public int? ParentFormId { get; set; }
        public string NavigateURL { get; set; }
        public string Icon { get; set; }
        public int? DisplayOrder { get; set; }
        public List<MenuVW> SubMenu { get; set; }
    }
}
