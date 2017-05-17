using Microsoft.Owin;
using Owin;


[assembly: OwinStartupAttribute(typeof(Journal3.Startup))]
namespace Journal3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
