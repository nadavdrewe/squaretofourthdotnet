using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class MenuType
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public virtual ICollection<Menu> Menus { get; set; }

    }

   
}
