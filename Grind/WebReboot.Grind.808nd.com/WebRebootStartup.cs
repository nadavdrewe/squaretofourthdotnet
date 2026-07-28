using Microsoft.Owin;
using Owin;



[assembly: OwinStartupAttribute(typeof(WebReboot.Grind._808nd.com.WebRebootStartup))]
namespace WebReboot.Grind._808nd.com
{
    public partial class WebRebootStartup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
