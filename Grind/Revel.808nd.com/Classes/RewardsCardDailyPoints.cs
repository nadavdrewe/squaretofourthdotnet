using System;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class RewardsCardDailyPoints 
    {
        [Key]
        public long id { get; set; }
        public DateTime date { get; set; }
        public int total_points_on_date { get; set; }
        public int current_points_on_date { get; set; }
        public string card_number { get; set; }
        public virtual RewardsCardNew RewardsCardNew {get; set; }
      
    }
}
