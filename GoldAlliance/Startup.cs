using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GoldAlliance.Startup))]
namespace GoldAlliance
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
