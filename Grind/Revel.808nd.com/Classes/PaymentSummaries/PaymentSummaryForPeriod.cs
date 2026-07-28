using System;

namespace Revel._808nd.com.Classes.PaymentSummaries
{
    public class PaymentSummaryForPeriod
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }       
        public string Establishment { get; set; }
        
    }
}
