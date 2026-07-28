using System;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class Funding
    {
        [Key]
        public int FundingId {get;set;}
        public string Amount { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
