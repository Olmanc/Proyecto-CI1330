using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProyectoDeInge.Startup))]
namespace ProyectoDeInge
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
