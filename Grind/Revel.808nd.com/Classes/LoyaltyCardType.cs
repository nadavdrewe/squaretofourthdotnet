using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class LoyaltyCardType
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        
        public virtual ICollection<RewardsCardNew> RewardsCardNews { get; set; } 

    }
}
