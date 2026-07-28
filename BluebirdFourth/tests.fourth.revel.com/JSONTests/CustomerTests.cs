using NUnit.Framework;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Classes.WebserviceReaderImplementations;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Revel._808nd.com.Classes;

namespace tests.fourth.revel.com.JSONTests
{

    [TestFixture]
    public class CustomerTests
    {
        [TestFixture]
        public class RevelTests
        {
            //Shoreditch Grind Tests
            static string APIKey = "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e";
            static string baseURL = "https://testshoreditchgrind.revelup.com/";
            static RevelContext _db = new RevelContext();

            CustomerServiceV2 SUT = new CustomerServiceV2(APIKey, baseURL, _db);

            DateTime start = new DateTime(2016, 01, 01);
            DateTime end = new DateTime(2017, 06, 01);
            int estId = 1;

            //[Test]
            //public async Task GetSomeCustomersByDate()
            //{
            //    var query = CustomerServiceV2.CustomerServiceV2ServiceQueries.getCustomerByDateRange(start, end);
            //    var result = await SUT.GetCustomersFromRevel(query);
            //    result.ShouldNotBeEmpty();
            //}

            //[Test]
            //public async Task GetSomeCustomersByEmail()
            //{
            //    var query = CustomerServiceV2.CustomerServiceV2ServiceQueries.getCustomerByEmail("stage@stage.com");
            //    var result = await SUT.GetCustomersFromRevel(query);
            //    result.ShouldNotBeEmpty();
            //}

            //[Test]
            //public async Task GetSomeCustomersByLastName()
            //{
            //    var query = CustomerServiceV2.CustomerServiceV2ServiceQueries.getCustomerByLastname("stage");
            //    var result = await SUT.GetCustomersFromRevel(query);
            //    result.ShouldNotBeEmpty();
            //}



        //    [Test]
        //    public async Task GetACustomerByRevelId()
        //    {
        //        var query = CustomerServiceV2.CustomerServiceV2ServiceQueries.getCustomerByRevelID(1);
        //        var result = await SUT.GetCustomer(query);
        //        result.ShouldBeOfType<Customer>();


        //        [Test]
        //    public async Task GetSomeCustomersByEmail()
        //    {
        //        var query = CustomerServiceV2.CustomerServiceV2ServiceQueries.getCustomerByDateRange(start, end);
        //        var result = await SUT.GetCustomersFromRevel(query);
        //        result.ShouldNotBeEmpty();
        //    }

        //}

            [TestFixture]
            public class DBTests
            {
                
            }

        }
    }
}
