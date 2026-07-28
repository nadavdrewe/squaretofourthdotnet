using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(web.fourth.revel.com.Startup))]
namespace web.fourth.revel.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
