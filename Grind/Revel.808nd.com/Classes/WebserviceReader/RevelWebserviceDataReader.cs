using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.ObjectCreationFactories;

namespace Revel._808nd.com.Classes.WebserviceReader
{

    /// <summary>
    /// Pulls ALL records from the webservice 
    /// </summary>
    public class RevelWebserviceDataReader : IRevelReaderAsync, IDisposable
    {

        private string orderItemSuffix =
            @"&fields=product,order,voided_by,voided_reason,created_date,cost,deleted,discount,id,initial_price,tax_amount,taxed_flag,uuid,product_name_override,price,quantity,exchange_discount,exchanged,created_by,discount_amount,updated_date,kitchen_completed,start_time,expedited";

        private string orderSuffix = @"&fields=id,service_charge,closed,discount,discount_amount,final_total,gratuity,tax,establishment,prevailing_surcharge,prevailing_tax,subtotal,surcharge,created_date,created_by,discount_reason,discount_rule_amount,discount_rule_type,discount_tax_amount,discount_taxed,is_discounted,is_unpaid,points_added,points_redeemed,remaining_due,updated_date,web_order,bill_parent";
        public Establishment Establishment { get; set; }
        public RevelWebserviceDataReader(Establishment est)
        {
            //dont' actually use the establishmentID in this service but need it to init
            this.Establishment = est;
            helperFactory = new RevelFactory(est);

            FactoryURLs = new List<RevelFactoryURL>();
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_Product,
                    "/resources/Product?format=json&limit=700")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_ProductCategory,
                    "/products/ProductCategory?format=json&limit=0")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_Order,
                    "/resources/Order?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=0&closed=true" + orderSuffix)
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_OrderItem,
                    "/resources/OrderItem?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=0")// + orderItemSuffix)
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_Order_Time,
                    "/resources/Order?format=json&created_date__gt={0}&created_date__lte={1}&limit=0&closed=true")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_OrderItem_Time,
                    "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0")// + orderItemSuffix)
                );

            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_OrderItem_Time,
                    "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0")// + orderItemSuffix)
                );

            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_OrderItem_Time,
                    "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0")// + orderItemSuffix)
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.Customer,
                    "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0"));

            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.RewardsCardNew,
                    "/resources/RewardsCardNew?format=json&created_date__gt={0}&created_date__lte={1}&limit=0"));


            FactoryURLs.Add(
           new RevelFactoryURL(RevelObjectType.Product, "/resources/Product?format=json&limit=0")
           );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.ProductCategory, "/products/ProductCategory?format=json&limit=0")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.Order, "/resources/Order?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=100&limit=0&order_by=created_date" + orderSuffix)
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.OrderItem, "/resources/OrderItem?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=0&order_by=created_date") //+ orderItemSuffix)
                );
            FactoryURLs.Add(
               new RevelFactoryURL(RevelObjectType.Order_Time, "/resources/Order?format=json&created_date__gt={0}&created_date__lte={1}&limit=0&order_by=created_date" + orderSuffix)
               );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.OrderItem_Time, "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0&order_by=created_date") //orderItemSuffix)
                );

            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.Payment,
                    "/resources/Payment?format=json&created_date__gt={0}&created_date__lte={1}&limit=0") //&fields=amount,amount_authorized,card_type,created_date,deleted,establishment,executed,id,order,payment_date,updated_date
                );

            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.Discount,
                    "/resources/Discount?format=json&limit=0")
                );



            //FactoryURLs.Add(
            //new RevelFactoryURL(RevelObjectType.Order_CLOSED_FALSE, "/resources/Order?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=5000&order_by=created_date" + orderSuffix)
            //);
            FactoryURLs.Add(
               new RevelFactoryURL(RevelObjectType.Order_Time_CLOSE_FALSE, "/resources/Order?format=json&created_date__gt={0}&created_date__lte={1}&limit=4000&order_by=created_date" + orderSuffix)
               );




            //FOR REVEL IMPLEMENTATION
            FactoryURLs.Add(
     new RevelFactoryURL(RevelObjectType.ZEROLIMIT_Product, "/resources/Product?format=json&limit=0")
     );
            FactoryURLs.Add(
             new RevelFactoryURL(RevelObjectType.ZEROLIMIT_Order_Time, "/resources/Order?format=json&created_date__gt={0}&created_date__lte={1}&limit=0&order_by=created_date" + orderSuffix)
             );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.ZEROLIMIT_OrderItem_Time, "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0&order_by=created_date") //orderItemSuffix)
                );

        }


        public List<RevelFactoryURL> FactoryURLs { get; set; }

        public RevelFactory helperFactory { get; set; }



        public async Task<List<Discount>> GetDiscounts()
        {
            var COUNT = 0;
            List<Discount> thediscounts = new List<Discount>();

            //just use any Establishment, the method dooesn't use it
            HttpClient client = helperFactory.CreateHttpClient();

            string NextURLToPullFrom =
                (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.Discount).First()).WebserviceURL;

            NextURLToPullFrom = String.Format(NextURLToPullFrom);
            int callCount = 0;
            int failCount = 0;

            do
            {
                try
                {
                    string thisURLInCaseOfFailure = NextURLToPullFrom;

                    //response object to test for failures
                    var response = await
                        client.GetAsync(NextURLToPullFrom);

                    //actual content of response - e.g. the data
                    string successfulResponse =
                        await
                            response.Content.ReadAsStringAsync();

                    //test if response was correct (e.g. - 200OK) -- and if so, parse the data
                    callCount += 1;

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"])
                        {

                            //create strongly typed discountItems                  
                            try
                            {
                                COUNT += 1;
                                thediscounts.Add(new Discount(prop));
                            }
                            catch (Exception ex)
                            {

                                //do nothing, just process the next discount
                                //maybe write an error log?
                                var except = prop;
                                throw ex;
                            }
                        }

                        NextURLToPullFrom = foo["meta"]["next"];

                    }
                    else
                    {
                        failCount += 1;
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }
                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                    throw exception;
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount < 2 && callCount <= 100);

            return thediscounts;
        }


        public async Task<List<Payment>> GetPayments(DateTime StartDate, DateTime EndDate)
        {
            var COUNT = 0;
            List<Payment> thePayments = new List<Payment>();

            //just use any Establishment, the method dooesn't use it
            HttpClient client = helperFactory.CreateHttpClient();

            string NextURLToPullFrom =
                (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.Payment).First()).WebserviceURL;

            NextURLToPullFrom = String.Format(NextURLToPullFrom, StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                EndDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            int callCount = 0;
            int failCount = 0;

            do
            {
                try
                {
                    string thisURLInCaseOfFailure = NextURLToPullFrom;

                    //response object to test for failures
                    var response = await
                        client.GetAsync(NextURLToPullFrom);

                    //actual content of response - e.g. the data
                    string successfulResponse =
                        await
                            response.Content.ReadAsStringAsync();

                    //test if response was correct (e.g. - 200OK) -- and if so, parse the data
                    callCount += 1;

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"])
                        {

                            //create strongly typed PaymentItems                  
                            try
                            {
                                COUNT += 1;
                                thePayments.Add(new Payment(prop));
                            }
                            catch (Exception ex)
                            {

                                //do nothing, just process the next Payment
                                //maybe write an error log?
                                var except = prop;
                                throw ex;
                            }
                        }

                        NextURLToPullFrom = foo["meta"]["next"].ToString();

                    }
                    else
                    {
                        failCount += 1;
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }
                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                    throw exception;
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount < 2 && callCount <= 200);

            return thePayments;
        }

        public async Task<List<T>> GetRevelWebserviceData<T>(T theType, string URLtoPullFrom, GenericFactory objectCreatorFactory = null, int numberOfCalls = 50) where T : IRevelAddressable, IRevelCreateable, new()
        {

            var COUNT = 0;
            List<T> theList = new List<T>();

            string NextURLToPullFrom = URLtoPullFrom;




            int callCount = 0;
            int failCount = 0;

            do
            {
                try
                {
                    string thisURLInCaseOfFailure = NextURLToPullFrom;

                    HttpClient client = helperFactory.CreateHttpClient();

                    //response object to test for failures
                    var response = await
                        client.GetAsync(NextURLToPullFrom);

                    //actual content of response - e.g. the data
                    string successfulResponse =
                        await
                            response.Content.ReadAsStringAsync();

                    //test if response was correct (e.g. - 200OK) -- and if so, parse the data
                    callCount += 1;

                    if (response.IsSuccessStatusCode)
                    {

                        Console.WriteLine("Success code from Revel at: " + DateTime.Now);
                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"])
                        {

                            //what type of creation method are we using - v1 or v2? (Create or factory)                           
                            try
                            {
                                COUNT += 1;
                                var instance = new T();

                                if (objectCreatorFactory == null) //v1
                                {
                                    instance.Create(prop);
                                    theList.Add(instance);
                                }
                                else
                                { //v2

                                    //strip first and last curlies
                                    JObject jobj = JObject.Parse(prop.ToString());
                                    instance = objectCreatorFactory.Create<T>(jobj);
                                    theList.Add(instance);
                                }

                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("FAILED REVEL READER: " + ex.Message);
                                //do nothing, just process the next order
                                //maybe write an error log?
                                var except = prop;
                                throw ex;
                            }
                        }

                        NextURLToPullFrom = foo["meta"]["next"];

                    }
                    else
                    {
                        failCount += 1;
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }
                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything

                    throw new Exception("The Revel data reader couldn't work", exception);
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount < 2 && callCount <= numberOfCalls);

            return theList;
        }


        public async Task<T> GetRevelWebserviceItem<T>(T theType, string URLtoPullFrom, GenericFactory objectCreatorFactory = null, int numberOfCalls = 50) where T : IRevelAddressable, IRevelCreateable, new()
        {
            T instance;
            var COUNT = 0;
            List<T> theList = new List<T>();

            string NextURLToPullFrom = URLtoPullFrom;

            int callCount = 0;
            int failCount = 0;

            try
            {
                string thisURLInCaseOfFailure = NextURLToPullFrom;

                HttpClient client = helperFactory.CreateHttpClient();

                //response object to test for failures
                var response = await
                    client.GetAsync(NextURLToPullFrom);

                //actual content of response - e.g. the data
                string successfulResponse =
                    await
                        response.Content.ReadAsStringAsync();

                //test if response was correct (e.g. - 200OK) -- and if so, parse the data
                callCount += 1;

                if (response.IsSuccessStatusCode)
                {

                    dynamic foo = JObject.Parse(successfulResponse);

                    //what type of creation method are we using - v1 or v2? (Create or factory)                           
                    try
                    {
                        COUNT += 1;
                        instance = new T();

                        if (objectCreatorFactory == null) //v1
                        {
                            instance.Create(foo);

                        }
                        else
                        { //v2

                            //strip first and last curlies
                            JObject jobj = JObject.Parse(foo.ToString());
                            instance = objectCreatorFactory.Create<T>(jobj);

                        }

                    }
                    catch (Exception ex)
                    {

                        //do nothing, just process the next order
                        //maybe write an error log?
                        var except = foo;
                        throw ex;
                    }

                    return instance;

                }
                else
                {
                    throw new Exception("The Revel data reader couldn't work - didn't get success code from the API: Instead got - " + response.StatusCode);
                }
            }
            catch (Exception exception)
            {
                //we've caugth the exception, but we're not going to do anything

                throw new Exception("The Revel data reader couldn't work", exception);
            }

            throw new Exception("THe Revel Webservice Data Reader couldnt' create a properly formed item from the webservice");
        }




        /// <summary>
        /// Gets all orders for all branches
        /// </summary>
        /// <param name="StartDate">This should be the latest order in the database</param>
        /// <param name="EndDate">This should be DateTime.Now + T3.00.00am (e.g the latest possible order, to ensure all orders are captured) (</param>
        /// <returns></returns>
        public async Task<List<Order>> GetOrdersSinglePull(DateTime StartDate, DateTime EndDate)
        {
            var COUNT = 0;
            List<Order> theOrders = new List<Order>();

            //just use any Establishment, the method dooesn't use it
            HttpClient client = helperFactory.CreateHttpClient();

            string NextURLToPullFrom =
                (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.Order_Time).First()).WebserviceURL;

            NextURLToPullFrom = String.Format(NextURLToPullFrom, StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                EndDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            int callCount = 0;
            int failCount = 0;


            try
            {
                string thisURLInCaseOfFailure = NextURLToPullFrom;

                //response object to test for failures
                var response = await
                    client.GetAsync(NextURLToPullFrom);

                //actual content of response - e.g. the data
                string successfulResponse =
                    await
                        response.Content.ReadAsStringAsync();

                //test if response was correct (e.g. - 200OK) -- and if so, parse the data
                callCount += 1;

                if (response.IsSuccessStatusCode)
                {

                    dynamic foo = JObject.Parse(successfulResponse);
                    foreach (var prop in foo["objects"])
                    {

                        //create strongly typed OrderItems                  
                        try
                        {
                            COUNT += 1;
                            var order = new Order(prop);
                            theOrders.Add(order);
                        }
                        catch (Exception ex)
                        {

                            //do nothing, just process the next order
                            //maybe write an error log?
                            var except = prop;
                            throw ex;
                        }
                    }


                }
                else
                {
                    failCount += 1;
                    NextURLToPullFrom = thisURLInCaseOfFailure;
                }
            }
            catch (Exception exception)
            {
                //we've caugth the exception, but we're not going to do anything
                var wellWereHere = "We've had an exception - probably a timeout!";
                throw exception;
            }



            return theOrders;
        }

        public async Task<List<Order>> GetOrdersStandard(DateTime StartDate, DateTime EndDate)
        {
            var COUNT = 0;
            List<Order> theOrders = new List<Order>();

            //just use any Establishment, the method dooesn't use it
            HttpClient client = helperFactory.CreateHttpClient();

            string NextURLToPullFrom =
                (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.Order_Time_CLOSE_FALSE).First()).WebserviceURL;

            NextURLToPullFrom = String.Format(NextURLToPullFrom, StartDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                EndDate.ToString("yyyy-MM-ddTHH:mm:ss"));
            int callCount = 0;
            int failCount = 0;

            do
            {
                try
                {
                    string thisURLInCaseOfFailure = NextURLToPullFrom;

                    //response object to test for failures
                    var response = await
                        client.GetAsync(NextURLToPullFrom);

                    //actual content of response - e.g. the data
                    string successfulResponse =
                        await
                            response.Content.ReadAsStringAsync();

                    //test if response was correct (e.g. - 200OK) -- and if so, parse the data
                    callCount += 1;

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"])
                        {
                            COUNT += 1;
                            //create strongly typed OrderItems                  
                            try
                            {
                                var order = new Order(prop);
                                theOrders.Add(order);
                            }
                            catch (Exception ex)
                            {

                                //do nothing, just process the next order
                                //maybe write an error log?
                                throw ex;
                            }
                        }

                        NextURLToPullFrom = foo["meta"]["next"].ToString();

                    }
                    else
                    {
                        failCount += 1;
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }
                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                    throw exception;
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount < 2 && callCount <= 10);

            return theOrders;
        }



        public async Task<List<OrderItem>> GetOrderItems(DateTime startDate, DateTime endDate)
        {
            JObject testtheJson;

            List<OrderItem> theOrderItems = new List<OrderItem>();

            HttpClient client = helperFactory.CreateHttpClient();

            string NextURLToPullFrom =
                (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.OrderItem_Time).First()).WebserviceURL;


            NextURLToPullFrom = String.Format(NextURLToPullFrom, startDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                endDate.ToString("yyyy-MM-ddTHH:mm:ss"));

            int callCount = 0;
            int failCount = 0;

            do
            {
                try
                {
                    string thisURLInCaseOfFailure = NextURLToPullFrom;

                    //response object to test for failures
                    var response = await
                        client.GetAsync(NextURLToPullFrom);

                    //actual content of response - e.g. the data
                    string successfulResponse =
                        await
                            response.Content.ReadAsStringAsync();

                    callCount += 1;

                    //test if response was correct (e.g. - 200OK) -- and if so, parse the data

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);



                        foreach (var prop in foo["objects"])
                        {
                            testtheJson = prop; //so we can see what's happening if it voids out

                            //test for voided / test
                            var voided = prop["voided_reason"].ToString();

                            /*if (voided == "" || voided == null)
                            {
                            */
                            //create strongly typed OrderItems                  
                            try
                            {
                                var newOi = new OrderItem();
                                newOi.Create(prop);
                                theOrderItems.Add(newOi);
                            }
                            catch (Exception exception)
                            {

                                throw exception;
                            }
                            /*}*/
                        }


                        if (foo["meta"]["next"] == null)
                        {
                            NextURLToPullFrom = "";
                        }
                        else
                        {
                            NextURLToPullFrom = foo["meta"]["next"].ToString();
                        }


                        callCount += 1;
                    }

                    else
                    {
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                        failCount += 1;
                    }

                }
                catch (Exception exception)
                {
                    var wellWereHere = "We've had an exception - probably a timeout!";

                    throw exception;
                    //we've caugth the exception, but we're not going to do anything

                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount <= 2 && callCount <= 70);

            return theOrderItems;
        }




        public async Task<List<Product>> GetProducts()
        {
            List<Product> products = new List<Product>();
            try
            {



                int callCount = 0;
                int failCount = 0;

                HttpClient client = helperFactory.CreateHttpClient();
                string NextURLToPullFrom = "";

                NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.NOEST_Product).First()).WebserviceURL;

                //NextURLToPullFrom = String.Format(NextURLToPullFrom, this.Establishment.establishment_id.ToString());

                do
                {

                    string URLinCaseRequestTimesOut = NextURLToPullFrom;

                    var response =
                        await
                            client.GetAsync(NextURLToPullFrom);


                    if (response.IsSuccessStatusCode)
                    {

                        string successfulResponse = await response.Content.ReadAsStringAsync();

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var item in foo["objects"].Children())
                        {
                            //create strongly typed OrderItems
                            Product prod = new Product();

                            prod.brand = (string)item["brand"];
                            prod.product_id = (int)item["id"];
                            prod.name = (string)item["name"];

                            prod.price = Convert.ToDecimal(RevelHelper.CheckIfJSONZeroAndReturnZeroDecimalString((string)item["price"]));
                            //   sku = (string)item["sku"];
                            //prod.tax_included = (bool)item["tax_included"];
                            //prod.tax = Convert.ToDecimal(RevelHelper.CheckIfJSONZeroAndReturnZeroDecimalString(item["tax"]));
                            //  tax_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(tax);
                            prod.active = (string)item["active"];

                            prod.establishment = (string)item["establishment"];
                            prod.establishment_id =
                                    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                        (string)item["establishment"]);

                            prod.category = (string)item["category"];
                            prod.categories = ((string)item["category"]).Split(';');
                            prod.category_ids = new List<int?>();
                            prod.productclass = (string)item["productclass"];
                            prod.categoryID = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)item["category"]);

                            products.Add(prod);
                        }

                        NextURLToPullFrom = foo.meta.next;

                        callCount += 1;

                    }
                    else
                    {
                        NextURLToPullFrom = URLinCaseRequestTimesOut;
                        failCount += 1;
                    }

                } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount <= 2);



                //now we have all the products - each product go in and convert the categories array to ids[]
                foreach (var prod in products)
                {
                    for (int i = 0; i < prod.categories.Length; i++)
                    {
                        prod.category_ids.Add(RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(prod.categories.ElementAt(i)));

                        //ok great! list[0] will be primary cat, list [1] will be secondary cat
                    }

                }

                //finish populating the categories

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return products;
        }


        public async Task<List<Product>> GetProductsNoEstablishment()
        {
            List<Product> products = new List<Product>();
            try
            {


                int callCount = 0;
                int failCount = 0;

                HttpClient client = helperFactory.CreateHttpClient();
                string NextURLToPullFrom = "";

                NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.NOEST_Product).First()).WebserviceURL;

                //NextURLToPullFrom = String.Format(NextURLToPullFrom, this.Establishment.establishment_id.ToString());

                do
                {

                    string URLinCaseRequestTimesOut = NextURLToPullFrom;

                    var response =
                        await
                            client.GetAsync(NextURLToPullFrom);


                    if (response.IsSuccessStatusCode)
                    {

                        string successfulResponse = await response.Content.ReadAsStringAsync();

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var item in foo["objects"].Children())
                        {
                            //create strongly typed OrderItems
                            Product prod = new Product();

                            prod.brand = (string)item["brand"];
                            prod.product_id = (int)item["id"];

                            //testing
                            if (prod.product_id == 1388)
                            {
                                var stop = "";
                            }

                            prod.name = (string)item["name"];

                            prod.price = Convert.ToDecimal(RevelHelper.CheckIfJSONZeroAndReturnZeroDecimalString((string)item["price"]));
                            prod.sku = (string)item["sku"];
                            prod.tax_included = (bool)item["tax_included"];


                            prod.tax = 0.00M;//Convert.ToDecimal(RevelHelper.CheckIfJSONZeroAndReturnZeroDecimalString((string)item["tax"])) ?? 0.00;

                            //  tax_id = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(tax);
                            prod.active = (string)item["active"];

                            prod.resource_uri = (string)item["resource_uri"];
                            prod.establishment = (string)item["establishment"];
                            prod.establishment_id =
                                    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                        (string)item["establishment"]);

                            prod.category = (string)item["category"];
                            prod.categories = ((string)item["category"]).Split(';');
                            prod.category_ids = new List<int?>();
                            prod.productclass = (string)item["productclass"];
                            prod.categoryID = RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)item["category"]);

                            products.Add(prod);
                        }

                        if (foo["meta"] != null)
                        {
                            if (foo["meta"]["next"] != null)
                            {
                                NextURLToPullFrom = (string)foo["meta"]["next"];
                            }
                            else
                            {
                                NextURLToPullFrom = "";
                            }
                        }
                        else
                        {
                            NextURLToPullFrom = "";
                        }

                        callCount += 1;

                    }
                    else
                    {
                        NextURLToPullFrom = URLinCaseRequestTimesOut;
                        failCount += 1;
                    }

                } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount <= 2);



                //now we have all the products - each product go in and convert the categories array to ids[]
                foreach (var prod in products)
                {
                    for (int i = 0; i < prod.categories.Length; i++)
                    {
                        prod.category_ids.Add(RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(prod.categories.ElementAt(i)));

                        //ok great! list[0] will be primary cat, list [1] will be secondary cat
                    }

                }

                //finish populating the categories

            }
            catch (Exception ex)
            {

                throw ex;
            }

            return products;
        }


        public async Task<List<ProductCategory>> GetProductCategoriesNoEstablishment()
        {
            int callCount = 0;
            var failcount = 0;
            try
            {

                List<ProductCategory> productCategories = new List<ProductCategory>();

                HttpClient client = helperFactory.CreateHttpClient();

                string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.NOEST_ProductCategory).First()).WebserviceURL;


                do
                {

                    string URLinCaseRequestTimesOut = NextURLToPullFrom;

                    var response =
                        await
                            client.GetAsync(NextURLToPullFrom);

                    if (response.IsSuccessStatusCode)
                    {

                        string successfulResponse = await response.Content.ReadAsStringAsync();

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var cat in foo["objects"].Children())
                        //(int i = 0; i < foo["objects"].Children().Count(); i++)
                        {
                            ProductCategory prodCategory = new ProductCategory();

                            //    prodCategory.active = Convert.ToBoolean((string)cat["active"]);
                            //    prodCategory.brand = (string)cat["brand"];
                            prodCategory.productcategory_id = (int)cat["id"];
                            prodCategory.name = (string)cat["name"];
                            prodCategory.parent = (string)cat["parent"];
                            try
                            {
                                prodCategory.parent_id =
                                                     RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey((string)cat["parent"]);
                            }
                            catch (Exception)
                            {

                                throw;
                            }

                            prodCategory.establishment = (string)cat["establishment"];
                            prodCategory.establishment_id =
                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                    (string)cat["establishment"]);

                            // prodCategory.subcategories = new List<ProductCategory>(
                            //);


                            /*
                                                        //nested list for subcats
                                                        foreach (var nestedCat in cat["subcategories"].Children())
                                                        {
                                                            ProductCategory anotherCategory = new ProductCategory();

                                                 //           anotherCategory.brand = (string)nestedCat["brand"];
                                                            anotherCategory.productcategory_id = (int)nestedCat["id"];
                                                            anotherCategory.name = (string)nestedCat["name"];
                                                  //          anotherCategory.parent = (string)nestedCat["parent"];
                                                  //          anotherCategory.parent_id =
                                                   //             RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                                     //               (string)nestedCat["parent"]);

                                                            anotherCategory.establishment = (string)nestedCat["establishment"];
                                                            anotherCategory.establishment_id =
                                                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                                                    (string)nestedCat["establishment"]);

                                                        /*    anotherCategory.subcategories = new List<ProductCategory>(
                                                            );


                                                            prodCategory.subcategories.Add(anotherCategory);
                            #1#
                                                        }*/

                            //add to the list    
                            productCategories.Add(prodCategory);

                        }

                        NextURLToPullFrom = "";
                    }
                    else
                    {
                        NextURLToPullFrom = URLinCaseRequestTimesOut;
                        failcount += 1;
                    }

                } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failcount <= 2);


                //now assign my HIGH LEVEL categories to the correct types
                /*   foreach (var productCategory in productCategories)
                   {
                       //it's alcohol
                       if (productCategory.parent_id == 3)
                       {
                           AlcoholProductCategories.Add(productCategory);

                       }

                       //it's food - cat 13
                       if (productCategory.parent_id == 13)
                       {

                           FoodProductCategories.Add(productCategory);
                       }

                       //it's a drink - cat 8
                       if (productCategory.parent_id == 8)
                       {
                           if (productCategory.id == 9 || productCategory.id == 10)
                       
                           {HotDrinksProductCategories.Add(productCategory);}

                       }


                   }*/

                return productCategories;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }





        public async Task<List<ProductCategory>> GetProductCategories()
        {
            int callCount = 0;
            var failcount = 0;
            try
            {

                List<ProductCategory> productCategories = new List<ProductCategory>();

                HttpClient client = helperFactory.CreateHttpClient();

                string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.NOEST_ProductCategory).First()).WebserviceURL;


                do
                {

                    string URLinCaseRequestTimesOut = NextURLToPullFrom;

                    var response =
                        await
                            client.GetAsync(NextURLToPullFrom);

                    if (response.IsSuccessStatusCode)
                    {

                        string successfulResponse = await response.Content.ReadAsStringAsync();

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var cat in foo["objects"].Children())
                        //(int i = 0; i < foo["objects"].Children().Count(); i++)
                        {
                            ProductCategory prodCategory = new ProductCategory();

                            //    prodCategory.active = Convert.ToBoolean((string)cat["active"]);
                            //    prodCategory.brand = (string)cat["brand"];
                            prodCategory.productcategory_id = (int)cat["id"];
                            prodCategory.name = (string)cat["name"];
                            //    prodCategory.parent = (string)cat["parent"];
                            //  prodCategory.parent_id =
                            //    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                            //         (string)cat["parent"]);

                            prodCategory.establishment = (string)cat["establishment"];
                            prodCategory.establishment_id =
                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                    (string)cat["establishment"]);

                            // prodCategory.subcategories = new List<ProductCategory>(
                            //);


                            /*
                                                        //nested list for subcats
                                                        foreach (var nestedCat in cat["subcategories"].Children())
                                                        {
                                                            ProductCategory anotherCategory = new ProductCategory();

                                                 //           anotherCategory.brand = (string)nestedCat["brand"];
                                                            anotherCategory.productcategory_id = (int)nestedCat["id"];
                                                            anotherCategory.name = (string)nestedCat["name"];
                                                  //          anotherCategory.parent = (string)nestedCat["parent"];
                                                  //          anotherCategory.parent_id =
                                                   //             RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                                     //               (string)nestedCat["parent"]);

                                                            anotherCategory.establishment = (string)nestedCat["establishment"];
                                                            anotherCategory.establishment_id =
                                                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                                                    (string)nestedCat["establishment"]);

                                                        /*    anotherCategory.subcategories = new List<ProductCategory>(
                                                            );


                                                            prodCategory.subcategories.Add(anotherCategory);
                            #1#
                                                        }*/

                            //add to the list    
                            productCategories.Add(prodCategory);

                        }

                        NextURLToPullFrom = "";
                    }
                    else
                    {
                        NextURLToPullFrom = URLinCaseRequestTimesOut;
                        failcount += 1;
                    }

                } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failcount <= 2);


                //now assign my HIGH LEVEL categories to the correct types
                /*   foreach (var productCategory in productCategories)
                   {
                       //it's alcohol
                       if (productCategory.parent_id == 3)
                       {
                           AlcoholProductCategories.Add(productCategory);

                       }

                       //it's food - cat 13
                       if (productCategory.parent_id == 13)
                       {

                           FoodProductCategories.Add(productCategory);
                       }

                       //it's a drink - cat 8
                       if (productCategory.parent_id == 8)
                       {
                           if (productCategory.id == 9 || productCategory.id == 10)
                       
                           {HotDrinksProductCategories.Add(productCategory);}

                       }


                   }*/

                return productCategories;

            }
            catch (Exception ex)
            {

                throw ex;
            }


        }



        public void Dispose()
        {
            this.Establishment = null;
            this.helperFactory = null;
        }
    }


}

//end class

