using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class TimesheetEntryService : BaseService
    {

        public static class TimesheetEntryServiceQueries
        {
          
        }


        private static string getTimesheetEntryByDateRange(DateTime createdDateStart, DateTime createdDateEnd)
        {
            var query = "/resources/TimeSheetEntry?format=json&created_date__gt={0}&created_date__lte={1}&limit=0";
            var startdateString = createdDateStart.ToString("yyyy-MM-ddTHH:mm:ss");
            var endDateString = createdDateEnd.ToString("yyyy-MM-ddTHH:mm:ss");

            string webURL = String.Format(query,
                startdateString,
                endDateString);

            return webURL;
        }

        public async Task<IEnumerable<TimeSheetEntry>> GetTimesheetEntriesByDateRange(DateTime createdDateStart, DateTime createdDateEnd)
        {
            var query = getTimesheetEntryByDateRange(createdDateStart, createdDateEnd);
            return await this._webReader.GetRevelWebserviceData<TimeSheetEntry>(new TimeSheetEntry(), query, _genericObjectCreatorFactory);
        }
        
           

        public TimesheetEntryService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {


        }
    }
}
