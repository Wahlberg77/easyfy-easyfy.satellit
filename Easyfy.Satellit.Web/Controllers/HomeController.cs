using System;
using System.Linq;
using System.Web.Mvc;
using Easyfy.Data.RavenDb;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Model.Comment;
using Easyfy.Satellit.Model.Posts;
using Easyfy.Satellit.Web.Models;

namespace Easyfy.Satellit.Web.Controllers
{
  public class HomeController : RavenController
  {
    //Index Blog
    public ActionResult Index()
    {
      
      var listOfPosts =
        DataSession.Query<Post, Posts_View>()
          .Where(o => o.BlogReference == BlogId && o.Status == PostStatus.Active)
          .ToList();

      var blogs = DataSession.Load<Blog>(BlogId);

      var mainPostVm = new MainPostVm { Posts = listOfPosts, BlogRef = BlogId, Blogs = blogs };

      //_Layout för att visa samma på alla bloggar. 
      ViewBag.BlogName = blogs.BlogName;
      ViewBag.Description = blogs.Description;
      ViewBag.Url = blogs.Url;

      return View(mainPostVm);
    }

    //SinglePost!
    public ActionResult Details(string friendlyUrl)
    {
      var post = DataSession.Query<Post, Posts_View>().FirstOrDefault(o => o.FriendlyUrl == friendlyUrl);

      if(post == null)
        return HttpNotFound();

      var listOfPosts = DataSession.Query<Post, Posts_View>().Where(o => o.BlogReference == post.BlogReference).ToList();

      var singlePostVm = new SinglePostVm { Posts = listOfPosts, BlogRef = post.BlogReference, Post = post };

      return View("SinglePost",singlePostVm);
    }


    public ActionResult About()
    {
      ViewBag.Message = "Om oss";

      return View();
    }

    public ActionResult Contact()
    {
      ViewBag.Message = "Kontakta oss";

      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult SaveComment(string friendlyUrl, SinglePostVm vm)
    {
      try
      {
        var post = DataSession.Load<Post>(Post.Idprefix + friendlyUrl);

        post.Comments.Add(new Comment
        {
          Id = Guid.NewGuid().ToString(),
          IsPublished = false,
          Title = vm.Comment.Title,
          Content = vm.Comment.Content,
          Author = vm.Comment.Author,
          Email = vm.Comment.Email,
          Url = vm.Comment.Url
        });
        DataSession.SaveChanges();
        TempData["success"] = "Kommentaren skickad för granskning";

        return Redirect("/posts/"+post.FriendlyUrl);
      }
      catch (Exception)
      {
        TempData["error"] = "Kommentaren har inte skickats för granskning, försök igen.";
      }
      return RedirectToAction("Index");
    }
  }
}