using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(employeeredirect.grind.railgunit.com.Startup))]
namespace employeeredirect.grind.railgunit.com
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
