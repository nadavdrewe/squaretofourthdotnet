using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com
{
    public class BlackCardSignup
    {
        [Key]
        public int Id { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string email { get; set; }
        public DateTime? dob { get; set; }
        public string cardNumber { get; set; }
        public bool valid { get; set; }
        public bool created { get; set; }
        public DateTime? WhenCreated { get; set; }
    }
}
