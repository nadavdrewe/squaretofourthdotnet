using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using CsvHelper;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.Reporting.ReportingFactory;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Models;
using Web.Grind._808nd.com.Services;


namespace WebReboot.Grind._808nd.com.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {

        private GrindContext _db = new GrindContext();
        private string RevelAPIKEY { get; }
        private string RevelBaseURL { get; }
        //
        // GET: /Test/

        public AdminController()
        {
            RevelAPIKEY = ConfigurationManager.AppSettings["RevelAPIKEY"];
            RevelBaseURL = ConfigurationManager.AppSettings["RevelBaseURL"];
            _db.Database.CommandTimeout = 480;
        }

        // GET: Admin
        public ActionResult RefreshOrders()
        {

            return View();
        }


        public void RunWeeklyAccountingReport()
        {
            var db = new GrindContext();


            /*    var end = new DateTime(2016,07,03);*/

            var start = new DateTime(2015, 12, 06);

            var end = DateTime.Now.AddDays(-1);
            /* var start = end.AddDays(-91);*/


            var listOfFiles = new List<string>();
            var establihsments = db.Establishments
              .Where(x => x.establishment_id != 2)
          .Where(x => x.establishment_id != 9).ToList();

            /*var establihsments = db.Establishments.Where(x => x.establishment_id == 6).ToList();*/

            var factory = new OrderItemReportFactory();

            foreach (var est in establihsments)
            {
                var orderItemsOUtPut = new List<OrderItem>();

                var reportData = factory.CreateProductOrderItemSummaryReport(new ReportContext
                {
                    StartDate = start,
                    EndDate = end,
                    NoOfDaysInEachReportingPeriod = 7,
                    IdOfStore = est.establishment_id
                }, new GrindContext(), out orderItemsOUtPut, "");

                var csvPath = String.Format("c:\\temp\\{0}_{1}.csv", est.name, DateTime.Now.ToLongDateString());

                /*      ListToCSVExtensions.CreateCSVFromGenericList(reportData, csvPath);*/


                using (var csv = new CsvWriter(new StreamWriter(csvPath)))
                {
                    csv.Configuration.HasHeaderRecord = true;
                    csv.Configuration.Delimiter = ",";
                    csv.WriteRecords(reportData);
                }

                listOfFiles.Add(csvPath);
            }


            //mailout
            var to = new List<string>();
            /* to.Add("dan@grindandco.com");*/
            to.Add("emailnadz@gmail.com");
            MailService mail = new MailService(to, "Weekly Item Report", null, listOfFiles);
            mail.SendEmail();
        }

        [HttpPost]
        public async Task<ActionResult> RefreshOrders(DateTime start, DateTime end)
        {

            var revelTImeStart = new DateTime(start.Year, start.Month, start.Day, 02, 00, 00);
            var revelTImeEnd = new DateTime(end.Year, end.Month, end.Day, 02, 00, 00);

            var orderservice = new OrderService(RevelAPIKEY, RevelBaseURL, _db);
            await orderservice.UpdateOrders(revelTImeStart, revelTImeEnd);

            ViewBag.Done = "We're Done!!!!";

            return View();
        }

        public ActionResult RefreshOrderItems()
        {

            return View();
        }

        [HttpPost]
        public async Task<ActionResult> RefreshOrderItems(DateTime start, DateTime end)
        {

            var revelTImeStart = new DateTime(start.Year, start.Month, start.Day, 02, 00, 00);
            var revelTImeEnd = new DateTime(end.Year, end.Month, end.Day, 02, 00, 00);


            var orderItemService = new OrderItemsService();

            await orderItemService.GetAllDailyOrderItemsAndInsertAnyMissingRecords(RevelAPIKEY, RevelBaseURL, _db,
                revelTImeStart, revelTImeEnd);

            ViewBag.Done = "We're Done!!!!";

            return View();
        }

        public async Task<List<OrderItem>> HELPER_ReturnListOfOrderItemsNotInDB(
      List<OrderItem> theOrderItemsFromWebservice, List<OrderItem> theOrderItemsFromTheDB)
        {

            List<OrderItem> theOrderItemsToReturn = new List<OrderItem>();

            try
            {


                List<int> dbOrderItemIDs = (from orderItems in theOrderItemsFromTheDB
                                            select (int)orderItems.orderitem_id).ToList();


                List<int> webServiceOrderItemIDs = (from orderItems in theOrderItemsFromWebservice
                                                        // where orders.closed == true
                                                    select (int)orderItems.orderitem_id).ToList();

                //now get

                var orderItemIdsToInsert = webServiceOrderItemIDs.Except(dbOrderItemIDs);

                GrindContext grindContext = new GrindContext();

                List<OrderItem> orderItemsToInsert = new List<OrderItem>();

                foreach (var item in orderItemIdsToInsert)
                {
                    OrderItem OrderToInsert =
                        theOrderItemsFromWebservice.Where(c => c.orderitem_id == item).FirstOrDefault();
                    orderItemsToInsert.Add(OrderToInsert);

                }



                //check any specific orders exist in new order set
                //missing orders



                return orderItemsToInsert;

            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

    }
}