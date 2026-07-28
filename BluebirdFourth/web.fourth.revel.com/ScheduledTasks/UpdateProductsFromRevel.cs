using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Web;
using Quartz;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Models;
using web.fourth.revel.com.Controllers;

namespace web.fourth.revel.com.ScheduledTasks
{
    public class UpdateProductsFromRevel : IJob
    {
        public async void Execute(IJobExecutionContext context)
        {

            //set up TLS
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            ServicePointManager.ServerCertificateValidationCallback = new
            RemoteCertificateValidationCallback
            (
               delegate { return true; }
            );

            var db = new RevelContext();                
            var itemService = new OrderItemsService();      

            try
            {
                var brandToPullOrdersFor = new List<Brand>();       
                brandToPullOrdersFor = db.Brands
                    .Where(x=>x.brand_id == 27)
                    .Where(x => x.is_fourth_active == true).ToList();
                //brandToPullOrdersFor = db.Brands.Where(x => x.brand_id == 19 || x.brand_id == 21).ToList();

                if (brandToPullOrdersFor.Count() > 0)
                {

                    foreach (var brand in brandToPullOrdersFor)
                    {
                        var clock = Stopwatch.StartNew();
                        Console.WriteLine("Now starting Brand Product Update:" + brand.name.ToString());

                        try
                        {
                            var productsService = new ProductsController();
                            await productsService.RefreshProductsByBrand();

                            clock.Stop();
                            Console.WriteLine("Finishing Brand:" + brand.name.ToString() + " took " + clock.Elapsed.TotalSeconds);
                        }
                        catch (Exception ex)
                        {
                            var log = new ScheduledTaskLog
                            {
                                Detail = "The scheduler failed" + ex.GetType().DeclaringMethod + ex.Message + ex.InnerException,
                                FireTime = DateTime.Now,
                                Result = 0,
                                Message =
                                    "Error updating products",
                                /*Brand = brand.brand_id,
                                BrandName = brand.name,*/
                                Establishment = 0,
                                EstablishmentName = "",
                                LogType = "LOCAL",
                                ContainerEndDate = null,
                                ContainerStartDate = null,
                                User = "Automated - 3am Task"

                            };

                            db.ScheduledTaskLogs.Add(log);
                            db.SaveChanges();

                        }
                    }

                }

            }
            catch (Exception ex)
            {
                //write to eventlog
                //EventLog.CreateEventSource
            }
        }


    }
}