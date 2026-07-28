using Revel._808nd.com.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revel._808nd.com.Classes.Reporting.Caternet
{
    public class CaternetItemSummaryService
    {
        GrindContext _db;

        public CaternetItemSummaryService(GrindContext db)
        {
            _db = db;
        }
        public IEnumerable<CaternetItemSummary> GetSummaryForDateRange(DateTime startRange, DateTime endRange, int establishmentId)
        {
            //get summed items as per RevelUp
            var startParam = new SqlParameter("@startDate", SqlDbType.DateTime);
            startParam.Value = startRange;
            var endParam = new SqlParameter("@endDate", SqlDbType.DateTime);
            endParam.Value = endRange;
            var estParam = new SqlParameter("@establishmentId", SqlDbType.Int);
            estParam.Value = establishmentId;

            //PROC FILTERS OUT VOIDS - WE NEED TO KEEP IN VOID / COMP AMOUNT BUT NOT PURESALES
            //DELETED ARE REMOVED
            var summedItemsIncCompsAndVoids = _db.Database.SqlQuery<CaternetItemSummary>(
                "Revel_GenerateCaternetSummary @startDate, @endDate, @establishmentId", startParam, endParam, estParam).ToList();

            return summedItemsIncCompsAndVoids;

        }
    }
}
