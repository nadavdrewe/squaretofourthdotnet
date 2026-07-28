using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Constants.Grind._808nd.com;
using Thinktecture.IdentityServer.Core.Models;

namespace IdService.grind._808nd.com.Config
{
    public static class Clients
    {
        public static IEnumerable<Client> Get()
        {

            return new[]
            {
                new Client
                {
                    Enabled = true,
                    ClientName = "Grind Client MVC (Hybrid Flow",
                    ClientId = "mvc", 
                    Flow = Flows.Hybrid,
                    RequireConsent = true,

                    RedirectUris = new List<string>
                    {
                        GrindConstants.GrindAPIURL
                    }
                       
                },

            new Client
            {
                Enabled = true,
                ClientId = "native",
                Flow = Flows.Implicit,
                RequireConsent = true,
                
                RedirectUris = new List<string>
                {                    
                    GrindConstants.GrindAPIURL
                },
                 
               

            }

            };


        }




    }
}