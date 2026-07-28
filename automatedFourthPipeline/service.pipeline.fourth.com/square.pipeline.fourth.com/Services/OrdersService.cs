using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using square.pipeline.fourth.com.Extensions;
using Square;
using Square.Orders;

namespace square.pipeline.fourth.com.Services
{
    /// <summary>
    /// Wraps square orders service
    /// </summary>
    public class OrdersService : BaseService
    {
        public OrdersService(string apiToken, string baseUrl = null) : base(apiToken, baseUrl)
        {
        }

        /// <summary>
        /// Takes Location.Id, and range for time period to query
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="startTimeUTC"></param>
        /// <param name="endTimeUTC"></param>
        public async Task<IEnumerable<Order>> GetOrdersForLocationByDateTimeUTC(string locationId, DateTime startTimeUTC, DateTime endTimeUTC)
        {
            try
            {
                //convert the time to squareTimes
                var startTimeUTCstring = startTimeUTC.ToSquareDateTime();
                var endTimeUTCstring = endTimeUTC.ToSquareDateTime();

                //orders
                var query = new SearchOrdersQuery
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
                                StartAt = startTimeUTCstring,
                                EndAt = endTimeUTCstring
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
                    LocationIds = new List<string> { locationId },
                    Query = query,
                    Limit = 100
                };

                List<Order> ordersToReturn = new List<Order>();

                //get orders
                var orderResponseForLocation = await _client.Orders.SearchAsync(searchParamsQuery);
                if (orderResponseForLocation.Orders != null)
                {
                    var ordersReturnedFromAPI = orderResponseForLocation.Orders;
                    ordersToReturn.AddRange(ordersReturnedFromAPI);
                    //now paginate if there's a cursor
                    if (!String.IsNullOrWhiteSpace(orderResponseForLocation.Cursor))
                    {
                        var currentCursor = orderResponseForLocation.Cursor;
                        while (!String.IsNullOrWhiteSpace(currentCursor))
                        {
                            var subsequentResult = await _client.Orders.SearchAsync(
                                new SearchOrdersRequest
                                {
                                    LocationIds = new List<string> { locationId },
                                    Query = query,
                                    Cursor = currentCursor,
                                    Limit = 100
                                });
                            ordersToReturn.AddRange(subsequentResult.Orders);
                            currentCursor = subsequentResult.Cursor;
                        }
                    }

                    return ordersToReturn;
                }

                return null;

            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't get orders from Square", ex);
            }
        }
    }
}
