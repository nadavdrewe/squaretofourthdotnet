using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class CustomerGroupCustomerService : BaseService
    {
        public CustomerGroupCustomerService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        private static class CustomerGroupCustomerQueries
        {
            public static string getAllCustomerGroupCustomers { get; } = "/resources/CustomerGroupCustomers/?format=json";
        }

        public async Task<IEnumerable<CustomerGroupCustomer>> GetAllCustomerGroupCustomers()
        {
            return await this._webReader.GetRevelWebserviceData<CustomerGroupCustomer>(new CustomerGroupCustomer(), CustomerGroupCustomerService.CustomerGroupCustomerQueries.getAllCustomerGroupCustomers, _genericObjectCreatorFactory);
        }
    }
}
