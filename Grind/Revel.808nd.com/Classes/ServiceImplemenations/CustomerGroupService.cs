using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class CustomerGroupService : BaseService
    {
        public CustomerGroupService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        private static class CustomerGroupQueries
        {
            public static string getAllCustomerGroups { get; } = "/resources/CustomerGroup/?format=json";
        }

        public async Task<IEnumerable<CustomerGroup>> GetAllCustomerGroups()
        {
           return await this._webReader.GetRevelWebserviceData<CustomerGroup>(new CustomerGroup(), CustomerGroupService.CustomerGroupQueries.getAllCustomerGroups, _genericObjectCreatorFactory);
        }
    }
}
