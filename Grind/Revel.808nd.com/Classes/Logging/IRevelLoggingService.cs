using System;
using System.Threading.Tasks;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.Logging
{
    interface IRevelLoggingService
    {
        Task<bool> Log(RevelContextBase db, string detail, DateTime date, string logType, string result, string message, int brand = 0, string brandName = "", int establishment = 0, string establishmentName = "", int itemCount = 0, decimal totalMoney = 0.00M, string logtype = "", DateTime? containerStart = null);
        

 

    }
}
