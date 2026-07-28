using core.lightspeed.com.Models.Financial.Receipts;
using System;
using System.Collections.Generic;
using System.Text;

namespace domain.pipeline.fourth.com.Services.LightspeedResto.FourthSalesApi
{
    public class LsRestoLocationDataset
    {
        public string LocationCompanyId { get; set; }
        public IEnumerable<Receipt> receipts { get; set; } //includes item, payment and modifier
        
    }
}
