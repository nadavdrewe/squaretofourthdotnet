using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using Thinktecture.IdentityServer.Core;
using Thinktecture.IdentityServer.Core.Services.InMemory;

namespace IdService.grind._808nd.com.Config
{
    public static class Users
    {

        public static List<InMemoryUser> Get()
        {
            return new List<InMemoryUser>()
            {
                new InMemoryUser()
                {
                    Enabled = true,
                    Username = "Nadav",
                    Password = "secret",
                    Subject = "1",

                    Claims = new[]
                    {
                        new Claim(ClaimTypes.GivenName, "Nadav"),
                        new Claim(ClaimTypes.Surname, "Drewe"),
                        new Claim(ClaimTypes.Role, "MobileReadUser"),
                        new Claim(ClaimTypes.Role, "MobileWriteUser"),
                        new Claim(ClaimTypes.Role, "WebWriteUser"),
                        new Claim(ClaimTypes.Role, "WebReadUser"),


                    }
                }


            };
        }

    }
}