using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OnlineEventsMarketingApp.Startup))]
namespace OnlineEventsMarketingApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
