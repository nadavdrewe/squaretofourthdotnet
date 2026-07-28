using System.Configuration;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Models;
using web.fourth.revel.com.Controllers;
using web.fourth.revel.com.ScheduledTasks;
using System.Web.Caching;
using System.Net;
using System.Net.Security;

namespace web.fourth.revel.com
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);




 
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            var logRefresh = new ScheduledTaskLogsController();
            var ok = logRefresh.RefreshCachedScheduledTasks();

        }

    }
}
