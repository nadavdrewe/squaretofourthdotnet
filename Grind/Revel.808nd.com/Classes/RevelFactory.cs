using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Revel._808nd.com.Interfaces;

namespace Revel._808nd.com.Classes
{
    public class RevelFactory : IRevelFactoryAsync
    {

        protected Establishment Establishment
        { get; set; }
        public Uri BaseAddress { get; set; }
        private List<RevelFactoryURL> FactoryURLs { get; set; }
        public string DefaultHeaderAPIkey { get; set; }


        public HttpClient CreateShoreditchGrindHttpClient(string baseURL, string apikey)
        {
            HttpClient shoreditchGrindClient = new HttpClient();
            shoreditchGrindClient.Timeout = TimeSpan.FromSeconds(600);

            shoreditchGrindClient.BaseAddress = new Uri(baseURL);
            shoreditchGrindClient.DefaultRequestHeaders.Accept.Clear();
            shoreditchGrindClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            shoreditchGrindClient.DefaultRequestHeaders.Add("API-AUTHENTICATION",
                apikey);
            shoreditchGrindClient.DefaultRequestHeaders.Add("Referer", baseURL);

            return shoreditchGrindClient;
        }


        public HttpClient CreateHttpClient()
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(600);

            client.BaseAddress = this.BaseAddress;
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            client.DefaultRequestHeaders.Add("API-AUTHENTICATION", this.DefaultHeaderAPIkey);
            client.DefaultRequestHeaders.Add("Referer", this.BaseAddress.ToString());

            return client;
        }


        public RevelFactory(Establishment Establishment)
        {
            this.Establishment = Establishment;
            this.BaseAddress = Establishment.BaseUri;
            this.DefaultHeaderAPIkey = Establishment.api_key;
            this.FactoryURLs = new List<RevelFactoryURL>();
            //intialise all the factory URLs
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.Product, "/resources/Product?format=json&limit=0&establishment={0}")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.ProductCategory, "/products/ProductCategory?format=json&limit=0&establishment={0}")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.Order, "/resources/Order?format=json&closed=true&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=100&establishment={2}")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.OrderItem, "/resources/OrderItem?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=100&establishment={2}")
                );
            FactoryURLs.Add(
               new RevelFactoryURL(RevelObjectType.Order_Time, "/resources/Order?format=json&closed=true&created_date__gt={0}T02:00:00&created_date__lte={1}&limit=100&establishment={2}")
               );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.OrderItem_Time, "/resources/OrderItem?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}&limit=100&establishment={2}")
                );


            //new URLs added for pulling back without an establishment
            FactoryURLs.Add(
                  new RevelFactoryURL(RevelObjectType.NOEST_Product, "/resources/Product?format=json&limit=700")
                  );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_ProductCategory, "/products/ProductCategory?format=json&limit=0")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_Order, "/resources/Order?format=json&closed=true&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=220")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_OrderItem, "/resources/OrderItem?format=json&created_date__gt={0}T02:00:00&created_date__lte={1}T02:00:00&limit=180")
                );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_Order_Time, "/resources/Order?format=json&closed=true&created_date__gt={0}&created_date__lte={1}&limit=0")
               );
            FactoryURLs.Add(
                new RevelFactoryURL(RevelObjectType.NOEST_OrderItem_Time, "/resources/OrderItem?format=json&created_date__gt={0}&created_date__lte={1}&limit=0")
                );







        }




        //methods to create collections
        public async Task<List<Product>> CreateProducts(List<Product> products)
        {
            try
            {

                HttpClient client = CreateHttpClient();
                string NextURLToPullFrom = "";

                NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.Product).First()).WebserviceURL;

                NextURLToPullFrom = String.Format(NextURLToPullFrom, this.Establishment.establishment_id.ToString());

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
                            //   tax_included = (bool)item["tax_included"];
                            //   tax = (string)item["tax"];
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

                    }
                    else
                    {
                        NextURLToPullFrom = URLinCaseRequestTimesOut;
                    }

                } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom));



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





        public async Task<List<ProductCategory>> CreateProductCategories(List<ProductCategory> productCategories)
        {
            try
            {

                HttpClient client = CreateHttpClient();

                string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.ProductCategory).First()).WebserviceURL;

                NextURLToPullFrom = String.Format(NextURLToPullFrom, this.Establishment.establishment_id.ToString());

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


//                            prodCategory.brand = (string)cat["brand"];
                            prodCategory.productcategory_id = (int)cat["id"];
                            prodCategory.name = (string)cat["name"];
                     /*       prodCategory.parent = (string)cat["parent"];
                            prodCategory.parent_id =
                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                    (string)cat["parent"]);
*/
                            prodCategory.establishment = (string)cat["establishment"];
                            prodCategory.establishment_id =
                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                    (string)cat["establishment"]);

  /*                          prodCategory.subcategories = new List<ProductCategory>(
                            );
*/


/*
                            //nested list for subcats
                            foreach (var nestedCat in cat["subcategories"].Children())
                            {
                                ProductCategory anotherCategory = new ProductCategory();

                                anotherCategory.brand = (string)nestedCat["brand"];
                                anotherCategory.productcategory_id = (int)nestedCat["id"];
                                anotherCategory.name = (string)nestedCat["name"];
                                anotherCategory.parent = (string)nestedCat["parent"];
                                anotherCategory.parent_id =
                                    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                        (string)nestedCat["parent"]);

                                anotherCategory.establishment = (string)nestedCat["establishment"];
                                anotherCategory.establishment_id =
                                    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                        (string)nestedCat["establishment"]);

                                anotherCategory.subcategories = new List<ProductCategory>(
                                );


                                prodCategory.subcategories.Add(anotherCategory);

                            }
*/

                            //add to the list    
                            productCategories.Add(prodCategory);

                        }

                        NextURLToPullFrom = "";
                    }
                    else
                    {
                        NextURLToPullFrom = URLinCaseRequestTimesOut;
                    }

                } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom));


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


        public async Task<List<OrderItem>> CreateOrderItems(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<OrderItem> theOrderItems)
        {

            HttpClient client = CreateHttpClient();

            string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.OrderItem).First()).WebserviceURL;


            NextURLToPullFrom = String.Format(NextURLToPullFrom, startQueryDateTime.ToString("yyyy-MM-dd"), EndQueryDateTime.ToString("yyyy-MM-dd"), this.Establishment.establishment_id);

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

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);
                      

                       
                            foreach (var prop in foo["objects"])
                            {
                                //test for voided / test
                                var voided = prop["voided_reason"].ToString();

                                if (voided == "" || voided == null)
                                {
                                    //create strongly typed OrderItems                  
                                    theOrderItems.Add(new OrderItem(prop));
                                }
                            }
                       


                        NextURLToPullFrom = foo["meta"]["next"].ToString();
                    }

                    else
                    {
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }

                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom));

            return theOrderItems;
        }

        public async Task<List<OrderItem>> CreateOrderItems_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<OrderItem> theOrderItems)
        {
           

            HttpClient client = CreateHttpClient();

            string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.OrderItem_Time).First()).WebserviceURL;


            NextURLToPullFrom = String.Format(NextURLToPullFrom, startQueryDateTime.ToString("yyyy-MM-dd"), EndQueryDateTime.ToString("yyyy-MM-ddTHH:mm:ss"), this.Establishment.establishment_id);

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

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"].Children())
                        {
                            //test for voided / test
                           /* var voided = prop["voided_reason"].ToString();

                            if (voided == "" || voided == null)
                            {
                                //create strongly typed OrderItems                  */
                                theOrderItems.Add(new OrderItem(prop));
                            /*}*/
                        }

                        NextURLToPullFrom = foo["meta"]["next"].ToString();
                    }

                    else
                    {
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }

                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom));

           return theOrderItems;
        }


        public async Task<List<Order>> CreateOrders(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<Order> theOrders)
        {


            HttpClient client = CreateHttpClient();
           
            string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.Order).First()).WebserviceURL;
            NextURLToPullFrom = String.Format(NextURLToPullFrom, startQueryDateTime.ToString("yyyy-MM-dd"), EndQueryDateTime.ToString("yyyy-MM-dd"), this.Establishment.establishment_id);

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

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"])
                        {

                            //create strongly typed OrderItems                  
                            try
                            {
                                var order = new Order();
                                theOrders.Add(order.Create(prop));
                            }
                            catch (Exception ex)
                            {

                                //do nothing, just process the next order
                                //maybe write an error log?
                            }
                        }

                        NextURLToPullFrom = foo["meta"]["next"];
                    }
                    else
                    {
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }
                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom));

            return theOrders;
        }


        public async Task<List<Order>> CreateOrders_Time(DateTime startQueryDateTime, DateTime EndQueryDateTime, List<Order> theOrders)
        {       

            HttpClient client = CreateHttpClient();

            string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.Order_Time).First()).WebserviceURL;
            NextURLToPullFrom = String.Format(NextURLToPullFrom, startQueryDateTime.ToString("yyyy-MM-dd"), EndQueryDateTime.ToString("yyyy-MM-ddTHH:mm:ss"), this.Establishment.establishment_id);

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

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"])
                        {

                            //create strongly typed OrderItems                  
                            try
                            {
                                var order = new Order();
                                theOrders.Add(order.Create(prop));
                            }
                            catch (Exception ex)
                            {

                                //do nothing, just process the next order
                                //maybe write an error log?
                            }
                        }

                        NextURLToPullFrom = foo["meta"]["next"].ToString();
                    }
                    else
                    {
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                    }
                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom));

            return theOrders;
        }


        public async Task<RevelOrderandOrderItemWrapper> PopulateOrderAndItemWrapper(RevelOrderandOrderItemWrapper wrapper)
        {
            //check wrapper type and know what collection to initialise
            if(wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full) || wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Order))
            {
                await this.CreateOrders(wrapper.StartDate, wrapper.EndDate, wrapper.Orders);
            }
            if (wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full) ||
                wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.OrderItem))
            {
                await this.CreateOrderItems(wrapper.StartDate, wrapper.EndDate, wrapper.OrderItems);
            }


            //time based params
            if (wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full_Time) || wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Order_Time))
            {
                await this.CreateOrders_Time(wrapper.StartDate, wrapper.EndDate, wrapper.Orders);
            }

            if (wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.Full_Time) ||
                wrapper.Type.Equals(RevelOrderandOrderItemWrapper.WrapperType.OrderItem_Time))
            {
                await this.CreateOrderItems_Time(wrapper.StartDate, wrapper.EndDate, wrapper.OrderItems);
            }

            return wrapper;
        }


        public async Task<RevelProductAndCategoryWrapper> CreateProductsAndCategories(RevelProductAndCategoryWrapper wrapper)
        {
            await this.CreateProductCategories(wrapper.ProductCategories);
            await this.CreateProducts(wrapper.Products);

            wrapper.CreateProductCategoriesDictionary();

            return wrapper;
        }


        //NO ESTABLISHMENT

        public async Task<List<Order>> CreateOrdersNoEstablishment(DateTime StartDate, DateTime EndDate)
        {
            List<Order> theOrders = new List<Order>();

            //just use any Establishment, the method dooesn't use it
            HttpClient client = this.CreateHttpClient();

            string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.NOEST_Order_Time).First()).WebserviceURL;

            NextURLToPullFrom = String.Format(NextURLToPullFrom, StartDate.ToString("yyyy-MM-ddTHH:mm:ss"), EndDate.ToString("yyyy-MM-ddTHH:mm:ss"));

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

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);

                        foreach (var prop in foo["objects"])
                        {

                            //create strongly typed OrderItems                  
                            try
                            {
                                theOrders.Add(new Order(prop));
                            }
                            catch (Exception ex)
                            {

                                //do nothing, just process the next order
                                //maybe write an error log?
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
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount < 2);

            return theOrders;
        }

        public async Task<List<OrderItem>> CreateOrderItemsNoEstablishment(DateTime startDate, DateTime endDate)
        {
            List<OrderItem> theOrderItems = new List<OrderItem>();

            HttpClient client = this.CreateHttpClient();

            string NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.NOEST_OrderItem_Time).First()).WebserviceURL;


            NextURLToPullFrom = String.Format(NextURLToPullFrom, startDate.ToString("yyyy-MM-ddTHH:mm:ss"), endDate.ToString("yyyy-MM-ddTHH:mm:ss"));

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

                    if (response.IsSuccessStatusCode)
                    {

                        dynamic foo = JObject.Parse(successfulResponse);



                        foreach (var prop in foo["objects"])
                        {
                            //test for voided / test0
                            /*var voided = prop["voided_reason"].ToString();

                            if (voided == "" || voided == null)
                            {*/
                                //create strongly typed OrderItems                  
                            try
                            {
                                theOrderItems.Add(new OrderItem(prop));
                            }
                            catch (Exception ex)
                            {
                                
                                throw ex;
                            }
                            /*}*/
                        }

                        NextURLToPullFrom = foo["meta"]["next"];
                    }

                    else
                    {
                        NextURLToPullFrom = thisURLInCaseOfFailure;
                        failCount += 1;
                    }

                }
                catch (Exception exception)
                {
                    //we've caugth the exception, but we're not going to do anything
                    var wellWereHere = "We've had an exception - probably a timeout!";
                    throw exception;
                }


            } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount <= 2);

            return theOrderItems;
        }



        public async Task<List<Product>> CreateProductsNoEstablishment()
        {

            List<Product> products = new List<Product>();

            try
            {
                

                int failCount = 0;

                HttpClient client = CreateHttpClient();
                string NextURLToPullFrom = "";

                NextURLToPullFrom = (FactoryURLs.Where(x => x.RevelObjectType == RevelObjectType.NOEST_Product).First()).WebserviceURL;

              
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
                            prod.resource_uri = (string)item["resource_uri"];
                            prod.price = Convert.ToDecimal(RevelHelper.CheckIfJSONZeroAndReturnZeroDecimalString((string)item["price"]));
                            //   sku = (string)item["sku"];
                            //   tax_included = (bool)item["tax_included"];
                            //   tax = (string)item["tax"];
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

                    }
                    else
                    {
                        NextURLToPullFrom = URLinCaseRequestTimesOut;
                        failCount += 1;
                    }

                } while (NextURLToPullFrom != "" && !String.IsNullOrWhiteSpace(NextURLToPullFrom) && failCount <= 2 );



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





        public async Task<List<ProductCategory>> CreateProductCategoriesNoEstablishment()
        {
            var failcount = 0;
            try
            {

                List<ProductCategory> productCategories = new List<ProductCategory>();

                HttpClient client = CreateHttpClient();

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

                  /*          prodCategory.active = Convert.ToBoolean((string)cat["active"]);
                            prodCategory.brand = (string)cat["brand"];
                  */          prodCategory.productcategory_id = (int)cat["id"];
                            prodCategory.name = (string)cat["name"];
                  /*          prodCategory.parent = (string)cat["parent"];
                            prodCategory.parent_id =
                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                    (string)cat["parent"]);
*/
                            prodCategory.establishment = (string)cat["establishment"];
                            prodCategory.establishment_id =
                                RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                    (string)cat["establishment"]);

/*
                            prodCategory.subcategories = new List<ProductCategory>(
                            );
*/



                            //nested list for subcats
                            foreach (var nestedCat in cat["subcategories"].Children())
                            {
                                ProductCategory anotherCategory = new ProductCategory();

//                                anotherCategory.brand = (string)nestedCat["brand"];
                                anotherCategory.productcategory_id = (int)nestedCat["id"];
                                anotherCategory.name = (string)nestedCat["name"];
/*
                                anotherCategory.parent = (string)nestedCat["parent"];
                                anotherCategory.parent_id =
*/
  /*                                  RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                        (string)nestedCat["parent"]);
*/
                                anotherCategory.establishment = (string)nestedCat["establishment"];
                                anotherCategory.establishment_id =
                                    RevelHelper.ConvertJSONPrimaryKeyFromURIToIntegerPrimaryKey(
                                        (string)nestedCat["establishment"]);

/*
                                anotherCategory.subcategories = new List<ProductCategory>(
                                );


                                prodCategory.subcategories.Add(anotherCategory);
*/

                            }

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


        
    }
}
