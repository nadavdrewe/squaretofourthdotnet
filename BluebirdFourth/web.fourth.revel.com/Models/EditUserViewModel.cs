using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;

namespace web.fourth.revel.com.Models
{
    public class EditUserViewModel
    {
        public EditUserViewModel() { }

        // Allow Initialization with an instance of ApplicationUser:
        public EditUserViewModel(IdentityUser user)
        {
            this.UserName = user.UserName;
            
        }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        [Display(Name = "Discriminator")]
        public string Discriminator { get; set; }


        //you might want to implement jobs too, if you want to display them in your index view
    }
}