using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes
{
    public class MenuPermissions
    {
        [Key]
        public int Id { get; set; }
        public virtual Establishment Establishment { get; set; }
        public virtual MenuType MenuType { get; set; }
    }
}
