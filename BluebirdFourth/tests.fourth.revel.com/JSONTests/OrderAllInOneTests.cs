using NUnit.Framework;
using Revel._808nd.com.Classes;
using Revel._808nd.com.Classes.ServiceImplemenations;
using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;

namespace tests.fourth.revel.com.JSONTests
{
    [TestFixture]
    public class OrderAllInOneTests
    {
        [TestFixture]
        public class RevelTests
        {
            //Artists Residence Tests
            static string APIKey = "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e";
            static string baseURL = "https://testshoreditchgrind.revelup.com/";
            static RevelContext _db = new RevelContext();

            OrderAllInOneService SUT = new OrderAllInOneService(APIKey, baseURL, _db);

            DateTime start = new DateTime(2016, 01, 05);
            DateTime end = new DateTime(2016, 02, 06);
            int estId = 1;

            [Test]
            public async Task GetSomeOrdersAllInOne()
            {
                var query = OrderAllInOneService.OrderAllInOneServiceQueries.getOrderAllInOneForDateRangeAndEstablishment(start, end, estId);
                var result = await SUT.GetOrderAllInOneFromRevel(query);
                result.ShouldNotBeEmpty();

            }
        }

        [TestFixture]
        public class DBTests
        {
            //Artists Residence Tests
            static string APIKey = "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e";
            static string baseURL = "https://testshoreditchgrind.revelup.com/";
            static RevelContext _db = new RevelContext();

            OrderAllInOneService SUT = new OrderAllInOneService(APIKey, baseURL, _db);

            DateTime start = new DateTime(2016, 01, 05);
            DateTime end = new DateTime(2016, 03, 06);
            int estId = 1;

            [Test]
            public async Task SaveSomeCustomersFromRevelInLocal()
            {
                try
                {
                    var query = OrderAllInOneService.OrderAllInOneServiceQueries.getOrderAllInOneForDateRangeAndEstablishment(start, end, estId);

                    //existing should be same results as Revel returns
                    var existing = _db.OrdersAllInOne.Where(x => x.establishment == "/enterprise/Establishment/" + estId + "/")
                        .Where(x => x.created_date >= start && x.created_date <= end)
                        .ToList();

                    var result = await SUT.GetOrderAllInOneForDateRangeAndEstablishmentAndReplaceLocal(start, end, estId);

                    if (existing.Count != 0)
                    {
                        var c = existing.Count();
                        var c2 = result.Count();

                        c2.ShouldBeGreaterThanOrEqualTo(c);
                    }
                    else
                    {


                    }
                }
                catch (Exception ex)
                {

                    throw ex;
                }
            }
        }
    }
}