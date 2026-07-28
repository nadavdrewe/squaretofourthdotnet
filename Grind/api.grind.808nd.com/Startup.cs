using System;
using System.Collections.Generic;
using System.Linq;
using Constants.Grind._808nd.com;
using Microsoft.Owin;
using Owin;
using Thinktecture.IdentityServer.AccessTokenValidation;

/*[assembly: OwinStartup(typeof(api.grind._808nd.com.Startup))]*/

namespace api.grind._808nd.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);


           /* app.UseIdentityServerBearerTokenAuthentication(new IdentityServerBearerTokenAuthenticationOptions
            {
                Authority = GrindConstants.IdSrv,
                RequiredScopes = new[] {""}

            });*/
        }
    }
}
