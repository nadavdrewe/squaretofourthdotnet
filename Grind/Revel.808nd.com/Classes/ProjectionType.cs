using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Revel._808nd.com.Classes
{
    public class ProjectionType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Projection> Projections { get; set; } 
    }
}
