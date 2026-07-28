using NUnit.Framework;
using Shouldly;
using square.pipeline.fourth.com.Services;
using Square;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tests.square.pipeline.fourth.com.ClientTests
{
    [TestFixture]
    [Explicit("Requires live Square credentials; use sandbox replay tests for default CI/local verification.")]
    public class OrderServiceTests
    {
        string emailNadzliveApiKey = "";
        string emailNadzToken = "";

        OrdersService SUT;
        List<Location> locations = new List<Location>();
        DateTime startUTC;
        DateTime endUTC;


        [SetUp]
        public async Task Arrange()
        {
            SUT = new OrdersService(emailNadzToken);

            startUTC = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 04, 00, 00);
            endUTC = startUTC.AddDays(1);

        }

        [Test]
        public async Task GetLocations()
        {
            try
            {
                var client = new SquareClient(emailNadzToken);
                var response = await client.Locations.ListAsync();
                locations = response.Locations?.ToList() ?? new List<Location>();

                locations.Count().ShouldBeGreaterThan(0);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Test]
        public async Task GetSomeOrders()
        {
            //get location then orders for those locations
            var client = new SquareClient(emailNadzToken);
            var locResponse = await client.Locations.ListAsync();
            locations = locResponse.Locations?.ToList() ?? new List<Location>();

            locations.Count().ShouldBeGreaterThan(0);

            IEnumerable<Order> ordersResults;
            try
            {
                foreach (var loc in locations)
                {
                    ordersResults = await SUT.GetOrdersForLocationByDateTimeUTC(loc.Id, startUTC, endUTC);
                    ordersResults.Count().ShouldBeGreaterThan(0);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

    }
}
