using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebReboot.Grind._808nd.com.PartialViews
{
    public class BreadcrumbWrapper
    {
        public string Title { get; set; }

        public IEnumerable<Breadcrumb> Breadcrumbs { get; set; } 
    }
}