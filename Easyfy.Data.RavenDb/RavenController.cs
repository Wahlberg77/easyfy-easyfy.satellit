using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Model.Membership;
using Raven.Abstractions.Logging;
using Raven.Client;
using Microsoft.Practices.ServiceLocation;

namespace Easyfy.Data.RavenDb
{
  public class RavenController : Controller
  {
    private readonly IDocumentSession _dataSession;
    private User _currentUser;
    public readonly ILog Logger;
    private string _blogId;

    public RavenController()
    {
      _dataSession = ServiceLocator.Current.GetInstance<IDocumentSession>();
      //_dataSession = RavenStore.GetSession();

      Logger = LogManager.GetLogger(this.GetType());
    }

    /// <summary>
    /// Checks to see if a view with a certain name exists.
    /// </summary>
    /// <param name="viewName">The name of the view</param>
    /// <returns>True if the view exists, and False if it does not exist</returns>
    protected bool ViewExists(string viewName)
    {
      ViewEngineResult result = ViewEngines.Engines.FindView(ControllerContext, viewName, null);
      return (result.View != null);
    }

    /// <summary>
    /// The database session created for this request.
    /// All changes that need persistance must call SaveChanges() on the DataSession.
    /// </summary>
    public IDocumentSession DataSession { get { return _dataSession; } }

    public string BlogId
    {
      get
      {
        if (String.IsNullOrWhiteSpace(_blogId))
          _blogId = ConfigurationManager.AppSettings["Satellit.BlogId"];

        return _blogId;
      }
      set { _blogId = value; }
    }

    public User CurrentUser
    {
      get
      {

        if (_currentUser != null)
          return _currentUser;

        if (User.Identity.IsAuthenticated)
        {
          if (_currentUser == null)
          {
            using (DataSession.Advanced.DocumentStore.AggressivelyCacheFor(TimeSpan.FromMinutes(60)))
            {
              _currentUser = DataSession.Query<User, Users_View>().SingleOrDefault(u => u.UserName == User.Identity.Name);
            }
            if (_currentUser != null)
              return _currentUser;

            // If we can't find the user that are Authenicated. It must have been deleated. Sign Out and set as visitor.
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut();
          }
        }

        var cookie = Request.Cookies.Get(ConfigurationManager.AppSettings["Site.SessionCookieName"]);
        var visitorId = Guid.NewGuid().ToString().ToUpper();
        if (cookie != null)
        {
          visitorId = cookie.Values["visitor"];
        }
        _currentUser = new User();


        return _currentUser;
      }
      set { _currentUser = value; }
    }

    public ActionResult ExecuteAction(string actionName, string controllerName, object values)
    {
      return ExecuteAction(actionName, controllerName, new RouteValueDictionary(values));
    }

    public ActionResult ExecuteAction(string actionName, string controllerName, RouteValueDictionary values)
    {
      if (!string.IsNullOrEmpty(actionName)) values["action"] = actionName;
      if (!string.IsNullOrEmpty(controllerName)) values["controller"] = controllerName;
      return ExecuteAction(values);
    }

    public ActionResult ExecuteAction(object values)
    {
      return ExecuteAction(new RouteValueDictionary(values));
    }

    public ActionResult ExecuteAction(RouteValueDictionary values)
    {
      return new ExecuteActionResult(values);
    }
  }

  public class ExecuteActionResult : ActionResult
  {
    public ExecuteActionResult(RouteValueDictionary values)
    {
      Values = values;
    }

    public RouteValueDictionary Values { get; set; }

    public override void ExecuteResult(ControllerContext context)
    {
      var data = Values;

      var currentRoute = context.RequestContext.RouteData;
      var routeData = new RouteData(currentRoute.Route, currentRoute.RouteHandler);
      foreach (var item in currentRoute.Values) routeData.Values.Add(item.Key, item.Value);
      foreach (var item in currentRoute.DataTokens) routeData.DataTokens.Add(item.Key, item.Value);
      foreach (var item in data) { routeData.Values[item.Key] = item.Value; }

      var factory = ControllerBuilder.Current.GetControllerFactory();
      var requestContext = new RequestContext(context.HttpContext, routeData);
      var controller = factory.CreateController(requestContext, (string)routeData.Values["controller"]);

      try
      {
        controller.Execute(requestContext);
      }
      finally
      {
        factory.ReleaseController(controller);
      }
    }
  }
}
