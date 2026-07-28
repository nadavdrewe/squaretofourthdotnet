using System.ComponentModel.DataAnnotations;
using System.Linq;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{
    public class Projection
    {
        [Key]
        public int Id { get; set; }
        public decimal ProjectionFigure { get; set; }

        public virtual ProjectionType ProjectionType { get; set; }
        public virtual Establishment Establishment { get; set; }
        public virtual _445Calendar _445CalendarWeek { get; set; }


        public Projection Get(ProjectionType type, _445Calendar week, GrindContext db)
        {

            return db.Projections
                .FirstOrDefault(x => x._445CalendarWeek.Equals(week) && x.ProjectionType.Equals(type));
           
        }
        
    }
}
