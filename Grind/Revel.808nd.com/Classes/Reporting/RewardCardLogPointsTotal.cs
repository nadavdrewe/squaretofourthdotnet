
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting
{
    public class RewardCardLogPointsTotal
    {
        public DateTime DateTime { get; set; }
        public string Grind { get; set; }
        public int NumberOfOrders { get; set; }
        public int NumberOfLogsAddingPoints { get; set; }
        public int NumberOfLogsRedeemingPoints { get; set; }
        public decimal LogsAddingPointsAsPercentageOfOrders { get; set; }
        public decimal LogsRedeemingPointsAsPercentageOfOrders { get; set; }
        public decimal TotalNumberOfPointsAdded { get; set; }
        public decimal TotalNumberOfPointsRedeemed { get; set; }

    }
}
