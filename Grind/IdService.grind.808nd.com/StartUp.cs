using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Constants.Grind._808nd.com;
using IdService.grind._808nd.com.Config;
using Microsoft.Owin;
using Owin;
using System.Web;
using System.Security.Cryptography.X509Certificates;
using Thinktecture.IdentityServer.Core.Configuration;

[assembly: OwinStartup(typeof(IdService.grind._808nd.com.Startup))]
namespace IdService.grind._808nd.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/identity", idservApp =>

                idservApp.UseIdentityServer(new IdentityServerOptions
                {
                    SiteName = "Grind IdentityServer",
                    IssuerUri = GrindConstants.IdentitySrvrUri,
                    SigningCertificate = GenerateX509Certificate2(),

                    Factory = InMemoryFactory.Create(
                    users: Users.Get(),
                    clients: Clients.Get(),
                    scopes: Scopes.Get()
                    )
                    

                })
            );

        }


        private X509Certificate2 GenerateX509Certificate2()
        {
            return new X509Certificate2(
                string.Format(@"{0}\bin\idsrv3test.pfx",
                AppDomain.CurrentDomain.BaseDirectory), "1"
                );

        }

    }
}
