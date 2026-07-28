using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class EmployeeService : BaseService
    {
        public EmployeeService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        public async Task<IEnumerable<Employee>> GetAllEmployees()
        {
            var query = "/resources/Employee/?format=json&limit=0";
            return await this._webReader.GetRevelWebserviceData<Employee>(new Employee(), query, _genericObjectCreatorFactory);
        }
    }
}
