using square.pipeline.fourth.com.Extensions;
using Square;
using Square.Refunds;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace square.pipeline.fourth.com.Services
{
    public class RefundsService : BaseService
    {
        public RefundsService(string accessToken, string baseUrl = null) : base(accessToken, baseUrl)
        {
        }

        public async Task<IEnumerable<PaymentRefund>> GetRefundsForLocationByDateTimeUTC(
            string locationId,
            DateTime startTimeUTC,
            DateTime endTimeUTC)
        {
            try
            {
                var refundsToReturn = new List<PaymentRefund>();
                var refundsResponse = await _client.Refunds.ListAsync(new ListRefundsRequest
                {
                    BeginTime = startTimeUTC.ToSquareDateTime(),
                    EndTime = endTimeUTC.ToSquareDateTime(),
                    LocationId = locationId,
                    Limit = 100,
                    SortOrder = "ASC"
                });

                if (refundsResponse != null)
                {
                    await foreach (var refund in refundsResponse)
                    {
                        refundsToReturn.Add(refund);
                    }
                }

                return refundsToReturn;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't get refunds from Square", ex);
            }
        }
    }
}
