using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class ScheduledTaskFactory
    {
        private GrindContext _db;


        public ScheduledTaskFactory(GrindContext context)
        {
            _db = context;
        }

        public ScheduledTaskLog Get(int revelEstablishmentID, DateTime logFireDate)
        {

            return
                _db.ScheduledTaskLogs.FirstOrDefault(
                    x =>
                        x.Establishment.Equals(revelEstablishmentID) &&
                        ((DateTime)x.FireTime).Year == logFireDate.Year
                        && ((DateTime)x.FireTime).Month == logFireDate.Month
                        && ((DateTime)x.FireTime).Day == logFireDate.Day);
        }

        public void Create(int revelEstablishmentID, DateTime logFireDate, string estName = "", string message = "No message was set",
            int result = 0, string logType = "No logtype was set")
        {

            _db.ScheduledTaskLogs.Add(new ScheduledTaskLog
            {
                Establishment = revelEstablishmentID,
                LogType = logType,
                FireTime = logFireDate,
                Message = message,
                EstablishmentName = estName,
                Result = result
            });

            _db.SaveChanges();

        }


    }
}
