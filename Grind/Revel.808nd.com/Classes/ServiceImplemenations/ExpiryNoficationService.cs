using System;
using System.Collections.Generic;
using System.Linq;
using mailservice.railgunit.com;
using Revel._808nd.com.Interfaces;
using Revel._808nd.com.Models;

namespace Revel._808nd.com.Classes.ServiceImplementaitons
{
    public class ExpiryNoficationService
    {

        public IEnumerable<T> GetInstancesPastExpiryDate<T>(IEnumerable<T> cards) where T : IExpire
        {
            var cardsWithExpiry = cards.Where(x => x.ExpiryDate != null).ToList();
            var cardsExpired = cardsWithExpiry.Where(x => ((DateTime)x.ExpiryDate).Date <= DateTime.Now.Date).ToList();
            return cardsExpired;
        }




        public NotificationResult NotifyExpired<T>(IEnumerable<T> thingsThatHaveExpired, string message, string messageTitle, IEnumerable<string> emailAddresses, RevelContextBase db) where T : IExpire, IIdentifiable
        {
            try
            {
                foreach (var thing in thingsThatHaveExpired)
                {
                    var customer = db.Customers.FirstOrDefault(x => x.LicNumber.Trim() == thing.Identifier.Trim());
                    if (customer != null)
                    {
                        var customerMEssage = String.Format("Email:{0} | Name: {1} {2}", customer.Email, customer.FirstName, customer.LastName);
                        message += "<h3>Identifier: " + thing.Identifier + " on the " + thing.ExpiryDate + " | " + customerMEssage + "<br/>";
                    }
                    else
                    {
                        message += "<h3>Identifier: " + thing.Identifier + " on the " + thing.ExpiryDate + "<br/>";
                    }

                }

              //  var mailservice = new GmailLessSecureMailService("grindandco808@gmail.com", "teenpunks23", emailAddresses, "Grind: Red Cards have expired", message);

             //   mailservice.SendEmail();
                return NotificationResult.OK;

            }
            catch (Exception ex)
            {

                throw new Exception("Unable to notify", ex);
            }
        }


    }


    public interface IExpire
    {
        DateTime? ExpiryDate { get; }
    }
}
