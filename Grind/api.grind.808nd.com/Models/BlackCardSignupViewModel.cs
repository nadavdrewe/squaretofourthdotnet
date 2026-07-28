using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.grind._808nd.com.Models
{
    public class BlackCardSignupViewModel
    {
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public string dob { get; set; }
        public string cardNumber { get; set; }
        public string cardNumberConfirm { get; set; }
    }
}
