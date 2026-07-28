using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revel._808nd.com.Models;
using Revel._808nd.com.Classes.WebserviceReader;
using System.Data.Entity;

namespace Revel._808nd.com.Classes.ServiceImplemenations
{
    /// <summary>
    /// Co9mbined DB / API SErvice class
    /// </summary>
    public class HouseAccountService : BaseService
    {


        public HouseAccountService(string RevelAPIKEY, string RevelBaseURL, RevelContextBase db) : base(RevelAPIKEY, RevelBaseURL, db)
        {

        }



        /// <summary>
        /// REVEL SERVICE
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>


        public async Task<IEnumerable<HouseAccount>> GetHouseAccountsFromRevel(string query)
        {
            try
            {
                var accounts = await _webReader.GetRevelWebserviceData<HouseAccount>(new HouseAccount(), query, _genericObjectCreatorFactory);
                return accounts;
            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't get HouseAccounts from Revel", ex);
            }
        }


        /// <summary>
        /// LOCAL DB SERVICE
        /// </summary>
        /// <param name="accounts"></param>
        /// <returns></returns>
        public int ReplaceOrInsert(IEnumerable<HouseAccount> accounts)
        {
            try
            {
                var toReplace = new List<HouseAccount>();
                foreach (var account in accounts)
                {
                    var existing = _db.HouseAccounts.FirstOrDefault(x => x.resource_uri.Trim() == account.resource_uri.Trim());
                    if (existing != null)
                    {
                        toReplace.Add(existing);
                    }
                }

                ((DbSet<HouseAccount>)_db.HouseAccounts).RemoveRange(toReplace);
                ((DbSet<HouseAccount>)_db.HouseAccounts).AddRange(accounts);
                return _db.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception("Couldn't save HouseAccount to local DB", ex);
            }
        }


        public HouseAccount GetIncludePaymentsAndCustomer(string revelResourceUri)
        {

            var account = ((DbSet<HouseAccount>)_db.HouseAccounts).FirstOrDefault(x => x.resource_uri.Trim() == revelResourceUri.Trim());

            if (account == null)
            {
                throw new Exception("Couldn't find an account in the local DB");
            }

            // var payments = _db.HouseAccountPayments.Where(x=>x.)
            var customer = _db.Customers.FirstOrDefault(x => x.ResourceUri.Trim() == account.customer.Trim());

            account.Customer = customer;

            return account;
        }



        public static class HouseAccountServiceQueries
        {
            public static string getAllAccounts()
            {
                return "resources/HouseAccount/?format=json&limit=900";
            }

            public static string getAccountsForCustomer(int revelCustomerId)
            {
                return String.Format("resources/HouseAccount/?format=json&limit=800&customer={0}", revelCustomerId);
            }

            public static string getAccountsGreaterThanID(int id)
            {
                return String.Format("/resources/HouseAccount?format=json&id__gt={0}&limit=800", id);
            }

            public static string getAccountsThatAreEnabled()
            {
                return String.Format("resources/HouseAccount/?format=json&limit=800&enabled=true");
            }





        }
    }
}
