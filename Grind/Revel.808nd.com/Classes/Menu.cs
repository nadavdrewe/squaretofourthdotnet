using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class Menu
    {
        [Key]
        public int id { get; set; }
        public DateTime WhenCreated { get; set; }
        public string WhoUploaded { get; set; }
        public virtual MenuType MenuType { get; set; }
        public virtual Establishment Establishment { get; set; }
        public ICollection<MenuFile> MenuFiles { get; set; } 
    }
}