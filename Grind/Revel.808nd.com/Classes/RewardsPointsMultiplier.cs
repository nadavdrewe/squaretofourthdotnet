using System;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class RewardsPointsMultiplier
    {
        [Key]
        public int id { get; set; }
        public string emailSuffix { get; set; }
        public int multiplier { get; set; }
        public bool active { get; set; }
        public DateTime expiryDate { get; set; }        
    }
}
