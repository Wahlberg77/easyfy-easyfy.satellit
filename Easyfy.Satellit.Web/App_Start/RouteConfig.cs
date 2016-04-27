using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Easyfy.Satellit.Web
{
  public class RouteConfig
  {
    public static void RegisterRoutes(RouteCollection routes)
    {
      routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

      routes.MapRoute(
          name: "About",
          url: "about",
          defaults: new { controller = "Home", action = "About" }
      );

      routes.MapRoute(
          name: "Contact",
          url: "contact",
          defaults: new { controller = "Home", action = "Contact" }
      );

      // Mappa upp 
      routes.MapRoute(name: "BlogUrl", url: "posts/{friendlyUrl}",
                  defaults: new { controller = "Home", action = "Details" });


      routes.MapRoute(name: "Default", url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
    }
  }
}
