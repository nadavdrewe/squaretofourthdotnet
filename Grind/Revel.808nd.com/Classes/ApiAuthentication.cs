using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class ApiAuthentication
    {
        [Key]
        public int Id { get; set; }
        public string UserName { get; set; }
        public string ApiKey { get; set; }

    }
}
