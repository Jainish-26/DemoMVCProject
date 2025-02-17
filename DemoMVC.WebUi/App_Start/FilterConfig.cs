using DemoMVC.WebUi.Filters;
using System.Web;
using System.Web.Mvc;

namespace DemoMVC.WebUi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationFilter());
            filters.Add(new ActivityLogFilter());
        }
    }
}
