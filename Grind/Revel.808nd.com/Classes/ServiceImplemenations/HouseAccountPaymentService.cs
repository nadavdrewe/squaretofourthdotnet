using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;
using System.Data.Entity;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    public class HouseAccountPaymentService : BaseService
    {
        /// <summary>
        /// Contains query strings for Revel Queries
        /// </summary>
        public static class HouseAccountPaymentServiceQueries
        {
            public static string getUnpaidAccounts = "resources/HouseAccountPayment/?format=json&is_paid=false&limit=800";
            public static string getUnBilledAccounts = "resources/HouseAccountPayment/?format=json&is_billed=false&limit=800";
            public static string getAllAccounts = "resources/HouseAccountPayment/?format=json&limit=900";

            public static string getAccountPaymentsForCustomer(int revelCustomerId)
            {
                return String.Format("resources/HouseAccountPayment/?format=json&customer={0}", revelCustomerId);
            }

            public static string getUnpaidAccountsForLastXMonths(int monthsToSubtract = 2)
            {

                var today = DateTime.Now.AddMonths(-monthsToSubtract);
                var ourDate = new DateTime(today.Year, today.Month, today.Day, 03, 00, 00).ToString("yyyy-MM-ddTHH:mm:ss");

                var query = getUnpaidAccounts + String.Format("&created_date__gte={0}", ourDate);
                return query;
            }

            public static string getUnBilledAccountsForLastXMonths(int monthsToSubtract = 2)
            {
                var today = DateTime.Now.AddMonths(-monthsToSubtract);
                var ourDate = new DateTime(today.Year, today.Month, today.Day, 03, 00, 00).ToString("yyyy-MM-ddTHH:mm:ss");

                var query = getUnBilledAccounts + String.Format("&created_date__gt={0}", ourDate);
                return query;
            }

            public static string getAllAccountsInDateRange(DateTime startDate, DateTime endDate)
            {
                var startDateString = startDate.ToString("yyyy-MM-ddTHH:mm:ss");
                var endDateString = endDate.ToString("yyyy-MM-ddTHH:mm:ss");
                var query = getAllAccounts + String.Format("&created_date__gte={0}&created_date__lte={1}", startDateString, endDateString);

                return query;
            }

        }


        public HouseAccountPaymentService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {
        }

        public async Task<IEnumerable<HouseAccountPayment>> GetHouseAccountPaymentFromRevel(string query)
        {
            return await this._webReader.GetRevelWebserviceData<HouseAccountPayment>(new HouseAccountPayment(), query, _genericObjectCreatorFactory);

        }


        public async Task<IEnumerable<HouseAccountPayment>> GetUnpaidAccountForLastXMonthsAndReplaceLocal(int monthsBack)
        {
            var query = HouseAccountPaymentService.HouseAccountPaymentServiceQueries.getUnpaidAccountsForLastXMonths(monthsBack);
            var newItems = await GetHouseAccountPaymentFromRevel(query);

            if (newItems.Count() > 0)
            {
                var earliestDateInOrders = newItems.Min(x => x.created_date);
                var latestDateInOrders = newItems.Max(x => x.created_date);

                var existingItems = _db.HouseAccountPayments
                    //.Where(x => x.establishment == "/enterprise/Establishment/" + establishmentId + "/")
                    .Where(x => x.created_date >= earliestDateInOrders && x.created_date <= latestDateInOrders)
                    .ToList();

                ((DbSet<HouseAccountPayment>)_db.HouseAccountPayments).RemoveRange(existingItems);
                ((DbSet<HouseAccountPayment>)_db.HouseAccountPayments).AddRange(newItems);
                _db.SaveChanges();
            }

            return newItems;
        }


        public int ReplaceOrInsert(IEnumerable<HouseAccountPayment> accountPayments)
        {
            try
            {
                var toReplace = new List<HouseAccountPayment>();
                foreach (var account in accountPayments)
                {
                    var existing = _db.HouseAccountPayments.FirstOrDefault(x => x.id == account.id);
                    if (existing != null)
                    {
                        toReplace.Add(existing);
                    }
                }

                 ((DbSet<HouseAccountPayment>)_db.HouseAccountPayments).RemoveRange(toReplace);
                ((DbSet<HouseAccountPayment>)_db.HouseAccountPayments).AddRange(accountPayments);
                return _db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't save HouseAccountPayments to local DB", ex);
            }
        }



    }
}

