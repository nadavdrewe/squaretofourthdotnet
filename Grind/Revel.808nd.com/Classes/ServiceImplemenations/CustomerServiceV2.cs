using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes.Utility;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class CustomerServiceV2 : BaseService
    {
        public CustomerServiceV2(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        public static class CustomerServiceV2ServiceQueries
        {
             static string getCustomerByRevelID(int id)
            {
                var query = String.Format("/resources/Customer/{0}/?format=json", id);
                return query;
            }

             static string getCustomerByEmail(string email)
            {
                var query = String.Format("/resources/Customer/?format=json&email={0}&limit=0", email);
                return query;
            }

             static string getCustomerByLastname(string lastName)
            {
                var query = String.Format("/resources/Customer/?format=json&last_name={0}&limit=0", lastName);
                return query;
            }

            public static string getCustomerByDateRange(DateTime createdDateStart, DateTime createdDateEnd)
            {
                var query = "/resources/Customer?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
                var startdateString = createdDateStart.ToRevelDate();
                var endDateString = createdDateEnd.ToRevelDate();

                string webURL = String.Format(query,
                    startdateString,
                    endDateString);

                return webURL;
            }


        }

        public async Task<IEnumerable<Customer>> GetCustomersFromRevel(string query)
        {
            return await this._webReader.GetRevelWebserviceData<Customer>(new Customer(), query, _genericObjectCreatorFactory);
        }

        public async Task<Customer> GetCustomer(string query)
        {
            return await this._webReader.GetRevelWebserviceItem<Customer>(new Customer(), query, _genericObjectCreatorFactory);
        }

                        
    }
}
