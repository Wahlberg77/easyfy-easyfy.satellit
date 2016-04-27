using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SandladanWebb.Startup))]
namespace SandladanWebb
{
  public partial class Startup
  {
    public void Configuration(IAppBuilder app)
    {
      ConfigureAuth(app);
    }
  }
}
