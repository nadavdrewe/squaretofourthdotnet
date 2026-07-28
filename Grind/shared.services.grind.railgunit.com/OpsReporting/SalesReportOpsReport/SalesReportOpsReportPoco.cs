using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace shared.services.grind.railgunit.com.OpsReporting.SalesReportOpsReport
{
    public class SalesReportOpsReportPoco
    {
        public int EstablishmentId { get; set; }
        public string EstablishmentName { get; set; }

        public decimal OverallSales { get; set; }
        public decimal EatInSales { get; set; }
        public decimal TakeAwaySales { get; set; }

        public decimal Discounts { get; set; }

        public decimal Untaxed_Service_Fee { get; set; }

        public decimal Tips { get; set; }
        public decimal VAT { get; set; }
        public decimal House_Account { get; set; }
        public decimal Gift_Card_Purchases { get; set; }
        public decimal Gift_Cards_Used { get; set; }
        public decimal Variance { get; set; }
        public decimal Net_to_Account_For { get; set; }

        public decimal Payments { get; set; }
        public decimal Cash { get; set; }
        public decimal Credit { get; set; }
        public decimal American_Express { get; set; }
        public decimal MasterCard { get; set; }
        public decimal Visa { get; set; }
        public decimal App { get; set; }
        public decimal OtherCredit { get; set; }
        public decimal Custom_Payment { get; set; }
        public decimal Grand_Total { get; set; }

        public decimal Payins { get; set; }
        public decimal Payouts { get; set; }

    }
}
