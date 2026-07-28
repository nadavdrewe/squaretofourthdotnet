using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ssp.api.grind.com.Requests
{
    public class MutateBalanceRequest
    {
        public string cardNumber { get; set; }
        public int amount { get; set; }
    }
}