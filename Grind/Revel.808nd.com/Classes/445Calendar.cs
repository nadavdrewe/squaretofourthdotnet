using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes
{
    public class _445Calendar
    {
        [Key]
        public int Id { get; set; }

        public DateTime StartDate { get; set; }

        public ICollection<Projection> Projections { get; set; }

        public static _445Calendar GetCurrentWeek(GrindContext db)
        {
            return
                db._445Calendar.Include(c => c.Projections)
                    .OrderBy(x => x.StartDate)
                    .Where(x => x.StartDate <= DateTime.Today)
                    .OrderByDescending(x => x.StartDate)
                    .FirstOrDefault();
        }

    }

}
