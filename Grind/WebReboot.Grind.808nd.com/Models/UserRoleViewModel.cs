using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Web.Grind._808nd.com.Models
{
    public class UserRoleViewModel
    {
        public AspNetRole Role {get;set;}
        public string UserName { get; set; }

        //this is userid 
        public string Id { get; set; }
    }
}