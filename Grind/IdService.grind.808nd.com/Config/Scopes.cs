using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Thinktecture.IdentityServer.Core.Models;

namespace IdService.grind._808nd.com.Config
{
    public static class Scopes
    {

        public static IEnumerable<Scope> Get()
        {

            return new[]
            {
                StandardScopes.OpenId,
                StandardScopes.Profile,

                //scope for API
                new Scope
                {
                    Enabled = true,
                    Name = "roles",
                    Type = ScopeType.Identity,
                    DisplayName = "Roles",
                    Description = "The roles you are in",
                    Claims = new List<ScopeClaim>
                    {
                        new ScopeClaim("role")

                    }
                },
                
                    new Scope
                    {
                    Enabled = true,
                    Name = "grindapi",
                    Type = ScopeType.Resource,
                    Emphasize = false,
                    DisplayName = "Grind API Scope",
                    Claims = new List<ScopeClaim>
                    {
                                                new ScopeClaim("role")
                    }
                    
                    }
                
            };


        }
    }
}