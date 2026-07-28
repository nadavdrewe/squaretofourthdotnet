using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class WifiLogin
    {
        [Key]
        public int Id { get; set; }
        public string FirstName {get;set;}
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Site { get; set; }
        public DateTime? LoginDate { get; set; }

        public WifiLogin()
        {            
            LoginDate = DateTime.Now;  
        }
    }
}
