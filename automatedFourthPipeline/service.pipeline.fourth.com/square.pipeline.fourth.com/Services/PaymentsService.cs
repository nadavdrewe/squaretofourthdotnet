using square.pipeline.fourth.com.Extensions;
using Square;
using Square.Payments;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace square.pipeline.fourth.com.Services
{
    /// <summary>
    /// Replaces V1TransactionService - uses V2 Payments API
    /// </summary>
    public class PaymentsService : BaseService
    {
        public PaymentsService(string accessToken, string baseUrl = null) : base(accessToken, baseUrl)
        {
        }

        public async Task<IEnumerable<Payment>> GetPaymentsForLocationByDateTimeUTC(string locationId,
            DateTime startTimeUTC, DateTime endTimeUTC)
        {
            try
            {
                //convert the time to squareTimes
                var startTimeUTCstring = startTimeUTC.ToSquareDateTime();
                var endTimeUTCstring = endTimeUTC.ToSquareDateTime();

                List<Payment> paymentsToReturn = new List<Payment>();

                //get payments
                var paymentsResponse = await _client.Payments.ListAsync(
                    new ListPaymentsRequest
                    {
                        BeginTime = startTimeUTCstring,
                        EndTime = endTimeUTCstring,
                        LocationId = locationId,
                        Limit = 100,
                        SortOrder = "ASC"
                    });

                if (paymentsResponse != null)
                {
                    await foreach (var payment in paymentsResponse)
                    {
                        paymentsToReturn.Add(payment);
                    }

                    return paymentsToReturn;
                }

                throw new Exception("Payments were zero for this Square locationId: " + locationId);
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't get payments from Square", ex);
            }
        }
    }
}
