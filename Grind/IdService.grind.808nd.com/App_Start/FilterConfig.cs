using System.Web;
using System.Web.Mvc;

namespace IdService.grind._808nd.com
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
