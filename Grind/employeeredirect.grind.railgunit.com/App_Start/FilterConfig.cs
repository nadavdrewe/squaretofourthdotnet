using System.Web;
using System.Web.Mvc;

namespace employeeredirect.grind.railgunit.com
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
