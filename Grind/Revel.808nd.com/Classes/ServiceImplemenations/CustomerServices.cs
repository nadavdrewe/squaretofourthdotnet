using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.WebserviceReader;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.WebserviceReaderImplementations
{
    public class CustomerService
    {
        public static class CustomerRevelQueries
        {
            public static string generateCustomersSinceDateQuery(DateTime since)
            {
                var sinceFormatted = since.ToString("yyyy-MM-ddTHH:mm:ss");
                return String.Format("/resources/Customer?format=json&created_date__gt={0}&limit=250", sinceFormatted);
            }

            public static string generateAllCustomersQuery()
            {              
                return String.Format("/resources/Customer?format=json&limit=1000");
            }
            public static string generateAllInactiveCustomersQuery()
            {                
                return String.Format("/resources/Customer?format=json&active=false&limit=0");
            }

        }



        private RevelContextBase _db { get; set; }
        protected string RevelAPIKEY { get; set; } = (string)ConfigurationManager.AppSettings["RevelAPIKEY"];
        protected string RevelBaseURL { get; set; } = (string)ConfigurationManager.AppSettings["RevelBaseURL"];

        public CustomerService(RevelContextBase db)
        {
            _db = db;
        }


        public async Task GetAllCustomersSinceLastCustomerAndInsertOrReplace()
        {





        }
        /// <summary>
        /// Gets all customers and 
        /// </summary>
        /// <returns></returns>
        public async Task<int> GetAllCustomersAndInsertNew(DateTime start, DateTime end)
        {

            var revOrg = new Establishment(1, "Grind",
                RevelAPIKEY,
                 new Uri(RevelBaseURL));

            RevelWebserviceDataReader webReader = new RevelWebserviceDataReader(revOrg);
            RevelDBWriter writer = new RevelDBWriter(_db);
            RevelDBReader DBReader = new RevelDBReader(revOrg);


            //get existing from DB and check what we don't have



            List<Customer> existingCustomers = await DBReader.GetRevelType<Customer>();

            List<Customer> webServiceexistingCustomers =

                await webReader.GetRevelWebserviceData(new Customer(),
                 String.Format(new Customer().theAddress, start.ToString("yyyy-MM-ddTHH:mm:ss"),
                    end.ToString("yyyy-MM-ddTHH:mm:ss"))
                );

            IEnumerable<int> webServicecustomerIDs;

            var customerIDsToInsert = GetNewCustomerIDs(existingCustomers, webServiceexistingCustomers,
                out webServicecustomerIDs);

            //does this work????



            List<Customer> customersToInsert = new List<Customer>();

            foreach (var item in customerIDsToInsert)
            {
                Customer customerToInsert = webServiceexistingCustomers.Where(c => c.RevelId == item).FirstOrDefault();
                customersToInsert.Add(customerToInsert);
            }


            if (customersToInsert.Any())
            {

                var howMany = writer.SaveRevelType(customersToInsert);

                return howMany.Result;
            }

            //update existing

            return 0;
        }




        public static IEnumerable<int> GetNewCustomerIDs(List<Customer> existingCustomers, List<Customer> webServiceexistingCustomers,
            out IEnumerable<int> webServicecustomerIDs)
        {
            List<int> existingcustomerIDs = (from customers in existingCustomers
                                             select (int)customers.RevelId).ToList();


            List<int> webcustomerIDs = (from customers in webServiceexistingCustomers
                                        select (int)customers.RevelId).ToList();

            IEnumerable<int> differentIds = webcustomerIDs.Except(existingcustomerIDs);

            webServicecustomerIDs = differentIds;

            return webServicecustomerIDs;



        }



        public async Task<int> UpdateCustomerAddresses(List<Customer> existingCustomers, List<Customer> webserviceCustomers)
        {

            foreach (var webserviceCustomer in webserviceCustomers)
            {
                //try get exisitng customer_revel
                var existingCustomer = existingCustomers.Find(x => x.RevelId == webserviceCustomer.RevelId);

                //try and get customer_revel addresses
                if (existingCustomer.Addresses != null)
                {
                    //cycle through addresses
                    foreach (var address in webserviceCustomer.Addresses)
                    {
                        //if the address doesn't exist, insert new address

                        //else if we cqan find the address..
                        //if any details are different, update that address
                    }

                }
            }

            return 0;
        }

        public async Task<int> UpdateExistingCustomersAndAddresses()
        {

            return 0;


        }

        /// <summary>
        /// Uses email address
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Customer> GetFromEmail(string email)
        {
            var customer = _db.Customers.Where(x => x.Email.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault();

            return customer;

        }


        public async Task<Customer> Get(string email)
        {
            var customer = _db.Customers.Where(x => x.Email.ToLower().Trim() == email.ToLower().Trim()).FirstOrDefault();

            return customer;

        }

        public async Task<Customer> GetFromRevelCustomerURL(string customerURL)
        {
            var customer = _db.Customers.Where(x => x.ResourceUri.ToLower().Trim() == customerURL.ToLower().Trim()).FirstOrDefault();

            return customer;

        }

    }
}
