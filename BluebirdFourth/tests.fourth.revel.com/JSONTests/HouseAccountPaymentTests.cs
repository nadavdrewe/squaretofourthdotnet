using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Classes.ServiceImplemenations;
using NUnit.Core;
using Revel._808nd.com.Models;
using Shouldly;
using Revel._808nd.com.Classes;

namespace tests.fourth.revel.com.JSONTests
{
    [TestFixture]
    public class HouseAccountPaymentTests
    {
        [TestFixture]
        public class RevelTests
        {

            //Artists Residence Tests
            static string APIKey = "54976208ef3e46839189e4310908c62e:c4a2a9664bcf4c1888170a78e8ebbdd08cdd1fc387d447c8b2a08ebc6bd1c0ba";
            static string baseURL = "https://artistresidence.revelup.com/";
            static RevelContext _db = new RevelContext();

            //HousePaymentService SUT = new HousePaymentService(APIKey, baseURL, _db);

            //[Test]
            //public async Task GetSomeHouseAccountPayments()
            //{
            //    var query = HousePaymentService.HousePaymentServiceQueries.getUnpaidAccounts;
            //    var result = await SUT.GetHouseAccountPaymentFromRevel(query);
            //    result.ShouldBeOfType<List<HouseAccountPayment>>();

            //}



        }

        //[TestFixture]
        //public class DBTests
        //{
        //    ////Artists Residence Tests
        //    static string APIKey = "be9685e8ca1847959350571318aa6f0f:da848e35fabd4f41a1bcb59268c3ad1ecef62b6c6f3e4e82a5faf443d0f8242e";
        //    static string baseURL = "https://testshoreditchgrind.revelup.com/";
        //    static RevelContext _db = new RevelContext();

        //    //Artists Residence Tests
        //    //static string APIKey = "54976208ef3e46839189e4310908c62e:c4a2a9664bcf4c1888170a78e8ebbdd08cdd1fc387d447c8b2a08ebc6bd1c0ba";
        //    //static string baseURL = "https://artistresidence.revelup.com/";
        //    //static RevelContext _db = new RevelContext();

        //    HousePaymentService SUT = new HousePaymentService(APIKey, baseURL, _db);

        //    DateTime start = new DateTime(2016, 01, 05);
        //    DateTime end = new DateTime(2016, 03, 06);
        //    //int estId = 1;

        //    [Test]
        //    public async Task SaveSomeHousePaymentsFromRevelInLocal()
        //    {
        //        try
        //        {
        //            var monthsBack = 52;
        //            var startLocal = DateTime.Now.AddMonths(-monthsBack);
        //            //existing should be same results as Revel returns
        //            var existing = _db.HouseAccountPayments
        //                .Where(x => x.created_date >= startLocal)
        //                .ToList();

        //            var result = await SUT.GetUnpaidAccountForLastXMonthsAndReplaceLocal(monthsBack);

        //            if (existing.Count != 0)
        //            {
        //                var c = existing.Count();
        //                var c2 = result.Count();

        //                c2.ShouldBeGreaterThanOrEqualTo(c);
        //            }
        //            else
        //            {

        //            }
        //        }
        //        catch (Exception ex)
        //        {

        //            throw ex;
        //        }
        //    }
        //}



    }
}
