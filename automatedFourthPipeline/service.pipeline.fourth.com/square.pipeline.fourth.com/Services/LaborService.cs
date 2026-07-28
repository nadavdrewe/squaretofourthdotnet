using square.pipeline.fourth.com.Extensions;
using Square;
using Square.Labor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace square.pipeline.fourth.com.Services
{
    public class LaborService : BaseService
    {
        public LaborService(string apiToken, string baseUrl = null) : base(apiToken, baseUrl)
        {
        }

        public async Task<IEnumerable<Timecard>> GetTimecardsForLocationByDateTimeUTC(
            string locationId,
            DateTime startTimeUTC,
            DateTime endTimeUTC)
        {
            try
            {
                var query = new TimecardQuery
                {
                    Filter = new TimecardFilter
                    {
                        LocationIds = new List<string> { locationId },
                        Start = new TimeRange
                        {
                            StartAt = startTimeUTC.ToSquareDateTime(),
                            EndAt = endTimeUTC.ToSquareDateTime()
                        }
                    },
                    Sort = new TimecardSort
                    {
                        Field = TimecardSortField.StartAt,
                        Order = SortOrder.Asc
                    }
                };

                var searchRequest = new SearchTimecardsRequest
                {
                    Query = query,
                    Limit = 100
                };

                var timecards = new List<Timecard>();
                var response = await _client.Labor.SearchTimecardsAsync(searchRequest);
                if (response.Timecards != null)
                {
                    timecards.AddRange(response.Timecards);
                }

                var currentCursor = response.Cursor;
                while (!string.IsNullOrWhiteSpace(currentCursor))
                {
                    var subsequentResponse = await _client.Labor.SearchTimecardsAsync(
                        new SearchTimecardsRequest
                        {
                            Query = query,
                            Cursor = currentCursor,
                            Limit = 100
                        });

                    if (subsequentResponse.Timecards != null)
                    {
                        timecards.AddRange(subsequentResponse.Timecards);
                    }

                    currentCursor = subsequentResponse.Cursor;
                }

                return timecards;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't get timecards from Square", ex);
            }
        }
    }
}
