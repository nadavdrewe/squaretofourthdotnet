using MailChimp;
using MailChimp.Types;
using Revel._808nd.com.Classes;
using System;
using System.Collections.Generic;

namespace Web.Grind._808nd.MailChimp
{
    public class MailChimpGrind
    {
        public int PushCardSignUp(Customer customer)
        {
            var apiKey = Environment.GetEnvironmentVariable("MAILCHIMP_API_KEY");
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("MAILCHIMP_API_KEY must be configured before syncing card sign-ups.");
            }

            try
            {
                var mailChimp = new MCApi(apiKey, true);
                var merges = new List.Merges();
                merges.Add("FNAME", customer.FirstName);
                merges.Add("LNAME", customer.LastName);

                var options = new List.SubscribeOptions
                {
                    DoubleOptIn = false,
                    ReplaceInterests = false,
                    SendWelcome = false
                };

                mailChimp.ListSubscribe("48704573d1", customer.Email, merges, options);
            }
            catch (Exception)
            {
                // Preserve the legacy no-throw behavior for failures returned by Mailchimp.
            }

            return 0;
        }
    }
}
