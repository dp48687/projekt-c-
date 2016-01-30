using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(KaladontServerSide.Startup))]
namespace KaladontServerSide
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }

    }
}