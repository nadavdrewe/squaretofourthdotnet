using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.FourthCreate;
using Revel._808nd.com.Classes.FourthModelMapping;
using Revel._808nd.com.Classes.ServiceImplementaitons;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Models;
using Revel._808nd.com.Extensions;
using Revel._808nd.com.FourthClient;

namespace web.fourth.revel.com.Controllers
{
    public class TestInitialisationController : Controller
    {
        public void TestDataBaseProc()
        {
            db = new RevelContext();
            var brand = db.Brands.First(x => x.brand_id == 11);


            var syncStart = new DateTime(2015, 09, 28, 03, 00, 00);
            var syncEnd = new DateTime(2015, 09, 29, 03, 00, 00);


            var startParam = new SqlParameter("@startDate", SqlDbType.DateTime);
            startParam.Value = syncStart;
            var endParam = new SqlParameter("@endDate", SqlDbType.DateTime);
            endParam.Value = syncEnd;
            var brandParam = new SqlParameter("@brandId", SqlDbType.Int);
            brandParam.Value = brand.brand_id;

            var summedOrders = db.Database.SqlQuery<RevelSummedOrderItems>(
                "Revel_Fourth_OrderItems @startDate, @endDate, @brandId", startParam, endParam, brandParam).ToList();

            var test = "";
        }

    public int TestFourthLogin()
        {
            db = new RevelContext();
            var brand = db.Brands.First(x => x.brand_id == 8);


            var client = new FourthClient();
            var token = client.Login("Babaji_Revel", "fnb5h0p160715");

            var customExports = client.fhAPI.GetCustomExports(token);
            var clientScreens = client.fhAPI.GetEPOSScreens(token);
            var units = client.fhAPI.GetAllUnits(token);

            //var dataTable = client.fhAPI.GetImportTypes(token);
            db = new RevelContext();
            var test = db.OrderItems.OrderByDescending(x => x.created_date).Take(2000).ToList();

            var XML = "";
          //  var clientDone = client.SubmitSalesRequestToFourth(test, brand, out XML);

            //if success, log it


            return 0;
        }

        public void TestAddScheduledTaskLogToDb()
        {
            var db = new RevelContext();

            db.ScheduledTaskLogs.Add(new ScheduledTaskLog
            {

                Detail = "Brand:",
                FireTime = DateTime.Now,
                Result = 1,
                Message = "OrderItems dowloaded from Revel Successfully!",
                Brand = 0,
                BrandName = "",
                Establishment = 0,
                EstablishmentName = "",
                TotalItemCount = 0,
                TotalPounds = 0,
                LogType = "LOCAL",
                ContainerEndDate = DateTime.Now,
                ContainerStartDate = DateTime.Now,
            });

            db.SaveChanges();

            var ok = "";
        }

        public TestInitialisationController()
        {
            branch = new Establishment(1, "TestRevelOrg",
           "50c867e5ad384c94b5dd90940e18c008:fe46716889d34166bd41978290351685f6b25ca13500414c84ce48f0636724b6",
           new Uri("https://victor.revelup.com/"));


            writer = new RevelDBWriter(new RevelContext());
            webReader = new RevelWebserviceDataReader(branch);


        }

        private RevelContext db { get; set; }
        Establishment branch { get; set; }
        RevelWebserviceDataReader webReader { get; set; }
        RevelDBWriter writer { get; set; }

        public async Task<int> CreateRevelStuff()
        {
            //
            // GET: /Test/
            Establishment branch = new Establishment(1, "TestRevelOrg",
                "50c867e5ad384c94b5dd90940e18c008:fe46716889d34166bd41978290351685f6b25ca13500414c84ce48f0636724b6",
                new Uri("https://victor.revelup.com/"));

            db = new RevelContext();
            RevelDBWriter writer = new RevelDBWriter(new RevelContext());
            var webReader = new RevelWebserviceDataReader(branch);

            CreateFourthRevelDBStackService setup = new CreateFourthRevelDBStackService(branch, writer);
            var ok = await setup.CreateFourthRevelDBStack();

            return 0;
        }

        public async Task<int> GetOrderItemsTestStack()
        {


            //INJECT
            db = new RevelContext();

            foreach (var brand in db.Brands.Where(x => x.is_fourth_active).ToList())
            {
                Establishment branch = new Establishment(1, "ARevelOrg",
                brand.key_secret,
                new Uri(brand.revel_base_url));

                RevelDBWriter writer = new RevelDBWriter(new RevelContext());
                var webReader = new RevelWebserviceDataReader(branch);


                //get orderitems for ALL establishments...?
                //REFACTOR THIS TO USE STANDARD
                //GET ORDERS SINCE LAST SYNC
                var orderItemsAstype = new OrderItem();
                var startdate = DateTime.Now.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:ss");
                var endDate = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss");
                string webURL = String.Format(orderItemsAstype.theAddress,
                    startdate,
                    endDate);

                var products = await webReader.GetProductsNoEstablishment();

                var orderItems = await webReader.GetRevelWebserviceData(orderItemsAstype,
                    webURL
                    );


                foreach (var establishment in db.Establishments.Where(x => x.is_fourth_active == true).ToList())
                {
                    var prodsForThisEst = products.Where(x => x.establishment == establishment.resource_uri).ToList();

                    foreach (var orderItem in orderItems)
                    {
                        var sku = prodsForThisEst.FirstOrDefault(x => x.resource_uri == orderItem.product);
                        //do something
                        if (sku != null)
                        {
                            var test = "";
                        }

                    }


                    //run some tests on these items

                    //category tests

                    //money tests

                    //product tests



                }
            }


            return 0;

        }


        public async Task<List<OrderItem>> GetItems()
        {

            var db = new RevelContext();
            var service = new OrderItemsService();

            var start = new DateTime(2015, 04, 01, 02, 00, 00);
            var end = new DateTime(2015, 05, 01, 02, 00, 00);

            var est = db.Establishments.Where(x => x.resource_uri == "/enterprise/Establishment/32/").FirstOrDefault();

            var orderItems = await service.GetOrderItems(db.Brands.First(), est, db, start, end);


            var addOK = db.OrderItems.AddRange(orderItems);
            var saveOk = -await db.SaveChangesAsync();

            return orderItems;

        }

        public async Task<int> CreateEstablishments()
        {
            var query = "/enterprise/Establishment/?format=json&limit=1000";

            var ests = new Establishment();

            var estabos = await webReader.GetRevelWebserviceData<Establishment>(ests, query);

            foreach (var est in estabos)
            {

                db.Establishments.Add(est);
            }

            var ok = db.SaveChanges();

            return 0;
        }


        public async Task<int> CreateProdCats()
        {

            var cat = new ProductCategory();
            var allCats = await webReader.GetRevelWebserviceData<ProductCategory>(cat, cat.theAddress);


            db.ProductCategories.AddRange(allCats);
            var ok = db.SaveChanges();

            return 0;

        }

        public async Task<int> CreateOrderItems()
        {


            var oi = new OrderItem();

            var startDate = new DateTime(2015, 01, 01, 02, 00, 00);
            var endDate = new DateTime(2015, 05, 01, 02, 00, 00);

            var query = "/resources/OrderItem?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=1000";
            var formattedQuery = String.Format(query, startDate, endDate);

            var allCats = await webReader.GetRevelWebserviceData<OrderItem>(oi, formattedQuery, null, 5);

            db.OrderItems.AddRange(allCats);
            var ok = db.SaveChanges();

            throw new NotImplementedException();

        }


        public async Task<int> CreateProducts()
        {

            var establishment = 32;


            db = new RevelContext();
            RevelDBWriter writer = new RevelDBWriter(new RevelContext());
            var webReader = new RevelWebserviceDataReader(branch);

            var prods = new Product();

            var startDate = new DateTime(2015, 01, 01, 02, 00, 00);
            var endDate = new DateTime(2015, 05, 01, 02, 00, 00);

            var query = "/resources/Product/?format=json&limit=500";//&establishment={0}";
            var formattedQuery = String.Format(query, establishment);

            var allProds = await webReader.GetRevelWebserviceData<Product>(prods, formattedQuery);

            db.Products.AddRange(allProds);

            var ok = db.SaveChanges();

            return 0;

        }

    }





}