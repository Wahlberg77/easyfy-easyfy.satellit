using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Easyfy.Data.RavenDb;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Admin.Models;
using Easyfy.Satellit.Model.Posts;
using Raven.Client;
using Raven.Client.Linq;

namespace Easyfy.Satellit.Admin.Controllers
{
  public class HomeController : RavenController
  {
    public ActionResult Index()
    {
      var listOfPost = DataSession.Query<Post, Posts_View>().OrderByDescending(o => o.Created).Take(5).ToList();
      var listOfBlogs = DataSession.Query<Blog>().OrderByDescending(o => o.CreatedAt).Take(5).ToList();

      var allNotApprovedCommentsCount =
        DataSession.Query<Post>().ToList().SelectMany(o => o.Comments.Where(x => x.IsPublished == false)).Count();

      var model = new AdminPanelvM
      {
        Posts = listOfPost,
        Blogs = listOfBlogs,
        AllNotApprovedCommentsCount = allNotApprovedCommentsCount
      };
      return View(model);
    }

    public ActionResult About()
    {
      ViewBag.Message = "Your application description page.";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Your contact page.";

      return View();
    }
  }
}