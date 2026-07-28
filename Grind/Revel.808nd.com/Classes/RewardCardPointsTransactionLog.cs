using System;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class RewardCardPointsTransactionLog : IPointsLoggable
    {
        [Key]
        public int id { get; set; }
        public int orginal_points_total { get; set; }
        public int orginal_points_current { get; set; }
        public int new_points_total { get; set; }
        public int new_points_current { get; set; }
        public int pointsAdded { get; set; }
        public int pointSetToRefreshInBucket { get; set; }
        public int multiplier { get; set; }
        public string card_number { get; set; }
        public DateTime WhenCreated { get; set; }

    }
}
