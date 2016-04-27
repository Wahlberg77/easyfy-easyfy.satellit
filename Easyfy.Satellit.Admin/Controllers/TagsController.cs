using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Easyfy.Data.RavenDb;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Model.Posts;
using Raven.Client;
using Raven.Client.Linq;

namespace Easyfy.Satellit.Admin.Controllers
{
  public class TagsController : RavenController
  {
    // GET: Tags
    public ActionResult Index(string q)
    {
      List<Post> listoftags;
      if (!string.IsNullOrEmpty(q))
      {
        if (!q.EndsWith("*"))
          q = q + "*";
        listoftags =
          DataSession.Query<Post, Posts_View>()
            .Search(o => o.InternalTags, q, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards)
            .ToList();
      }
      else
      {
        listoftags = DataSession.Query<Post, Posts_View>().ToList();
      }
      return Request.IsAjaxRequest() ? View("_ListOfTags", listoftags) : View(listoftags);
    }
  }
}