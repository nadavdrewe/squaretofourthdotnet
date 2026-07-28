using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Square;
using Square.Orders;
using Square.Payments;
using Square.TeamMembers;
using System.Linq;
using Shouldly;
using System.Globalization;
using square.pipeline.fourth.com.Extensions;
using System.Threading;

namespace tests.square.pipeline.fourth.com.ClientTests
{
    [TestFixture]
    [Explicit("Requires live Square credentials; use sandbox replay tests for default CI/local verification.")]
    public class BaseTest
    {

        string clientSandboxKey = "";
        string clientSanbboxToken = "";

        string emailNadzliveApiKey = "";
        string emailNadzToken = "";

        SquareClient _client;

        [SetUp]
        public async Task Arrange()
        {
            try
            {
                _client = new SquareClient(emailNadzToken);
            }
            catch (Exception Ex)
            {

                throw;
            }
        }

        [Test]
        public async Task GetLocations()
        {
            try
            {
                var response = await _client.Locations.ListAsync();
                var locations = response.Locations?.ToList() ?? new List<Location>();

                locations.Count().ShouldBeGreaterThan(0);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Test]
        public async Task SeedOrders()
        {
            var rnd = new Random();
            var uniqueId = rnd.Next(200000);

            var locationsResponse = await _client.Locations.ListAsync();
            var locations = locationsResponse.Locations?.ToList() ?? new List<Location>();

            //create new customer
            var cusResult = await _client.Customers.CreateAsync(
                new Square.Customers.CreateCustomerRequest
                {
                    GivenName = "Test",
                    FamilyName = uniqueId.ToString(),
                    EmailAddress = String.Format("test@test{0}.com", uniqueId)
                });

            //seed orders
            foreach (var loc in locations)
            {
                try
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Thread.Sleep(1000);

                        var item1 = new OrderLineItem
                        {
                            Quantity = "1",
                            Name = "Badminton Game",
                            BasePriceMoney = new Money { Amount = 20L, Currency = Currency.Gbp }
                        };

                        var testOrder = new Order
                        {
                            LocationId = loc.Id,
                            CustomerId = cusResult.Customer.Id,
                            LineItems = new List<OrderLineItem> { item1 }
                        };

                        var orderCreateResponse = await _client.Orders.CreateAsync(
                            new CreateOrderRequest
                            {
                                Order = testOrder,
                                IdempotencyKey = Guid.NewGuid().ToString()
                            });
                        orderCreateResponse.ShouldNotBeNull();
                        orderCreateResponse.Order.Id.ShouldNotBeNull();
                        orderCreateResponse.Order.LineItems.Count().ShouldBeGreaterThan(0);
                    }
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }


        [Test]
        public async Task GetOrdersPaymentsAndEmployeesForLocationToday()
        {
            var startDateMarker = DateTime.UtcNow.AddDays(-1);
            var yesterdayStartDateTime = new DateTime(startDateMarker.Year, startDateMarker.Month, startDateMarker.Day, 04, 00, 00);
            var yesterdayEndDateTime = yesterdayStartDateTime.AddDays(1);

            var todayDateTimeUTC = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day, 04, 00, 00);
            var todayDateTimeEndUTC = todayDateTimeUTC.AddDays(1);

            var yesterdayStartDateTimeString = yesterdayStartDateTime.ToSquareDateTime();
            var yesterdayEndDateTimeSTring = yesterdayEndDateTime.ToSquareDateTime();

            var todayDateTimeUTCString = todayDateTimeUTC.ToSquareDateTime();
            var todayDateTimeEndUTCString = todayDateTimeEndUTC.ToSquareDateTime();

            var randomDateStart = new DateTime(2019, 08, 08);
            var randomDateEnd = DateTime.UtcNow;

            var manuallySetDateTimeStartString = randomDateStart.ToSquareDateTime();
            var manuallySetDateTimeEndString = DateTime.Now.ToSquareDateTime();
            var startOFfset = new DateTimeOffset(randomDateStart, new TimeSpan());
            var endOFfset = new DateTimeOffset(randomDateEnd, new TimeSpan());

            var locResponse = await _client.Locations.ListAsync();
            var locations = locResponse.Locations?.ToList() ?? new List<Location>();

            foreach (var location in locations)
            {
                try
                {
                    List<Order> ordersToReturn = new List<Order>();

                    //PAYMENTS
                    var startOffsetString = startOFfset.DateTime.ToRFC339UTCDateTime();
                    var endOffsetString = endOFfset.DateTime.ToRFC339UTCDateTime();

                    var payments = new List<Payment>();
                    await foreach (var payment in await _client.Payments.ListAsync(
                        new ListPaymentsRequest
                        {
                            BeginTime = manuallySetDateTimeStartString,
                            EndTime = manuallySetDateTimeEndString
                        }))
                    {
                        payments.Add(payment);
                    }

                    //orders
                    var orderQuery = new SearchOrdersQuery
                    {
                        Filter = new SearchOrdersFilter
                        {
                            StateFilter = new SearchOrdersStateFilter
                            {
                                States = new List<OrderState> { OrderState.Completed }
                            },
                            DateTimeFilter = new SearchOrdersDateTimeFilter
                            {
                                ClosedAt = new TimeRange
                                {
                                    StartAt = manuallySetDateTimeStartString,
                                    EndAt = manuallySetDateTimeEndString
                                }
                            }
                        },
                        Sort = new SearchOrdersSort
                        {
                            SortField = SearchOrdersSortField.ClosedAt,
                            SortOrder = SortOrder.Asc
                        }
                    };

                    var searchParamsQuery = new SearchOrdersRequest
                    {
                        LocationIds = new List<string> { location.Id },
                        Query = orderQuery,
                        Limit = 100
                    };

                    var orderResponseForLocation = await _client.Orders.SearchAsync(searchParamsQuery);
                    var ordersReturned = orderResponseForLocation.Orders;
                    ordersReturned.Count().ShouldBeGreaterThan(0);
                    ordersToReturn.AddRange(ordersReturned);

                    if (!String.IsNullOrWhiteSpace(orderResponseForLocation.Cursor))
                    {
                        var currentCursor = orderResponseForLocation.Cursor;
                        while (!String.IsNullOrWhiteSpace(currentCursor))
                        {
                            var subsequentResult = await _client.Orders.SearchAsync(
                                new SearchOrdersRequest
                                {
                                    LocationIds = new List<string> { location.Id },
                                    Query = orderQuery,
                                    Cursor = currentCursor,
                                    Limit = 100
                                });
                            ordersToReturn.AddRange(subsequentResult.Orders);
                            currentCursor = subsequentResult.Cursor;
                        }
                    }

                    var ordered = ordersToReturn.OrderByDescending(x => x.CreatedAt).ToList();
                    ordersToReturn.Count().ShouldBeGreaterThan(0);

                    //list team members
                    var teamMemberResponse = await _client.TeamMembers.SearchAsync(
                        new SearchTeamMembersRequest());
                    var teamMembers = teamMemberResponse.TeamMembers;

                    //list products
                    var catalog = new List<CatalogObject>();
                    await foreach (var item in await _client.Catalog.ListAsync(new Square.Catalog.ListCatalogRequest()))
                    {
                        catalog.Add(item);
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
