using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Easyfy.Satellit.Web.Startup))]
namespace Easyfy.Satellit.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
