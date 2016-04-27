using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Easyfy.Data.RavenDb;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Model.Posts;
using Raven.Client;
using Raven.Client.Linq;

namespace Easyfy.Satellit.Admin.Controllers
{
  public class PostsController : RavenController
  {
    // GET: Posts
    public ActionResult Index(string q)
    {
      List<Post> listOfPost;
      if (!string.IsNullOrEmpty(q))
      {
        if (!q.EndsWith("*"))
          q = q + "*";
        listOfPost =
          DataSession.Query<Post, Posts_View>()
            .Search(o => o.Title, q, escapeQueryOptions: EscapeQueryOptions.AllowAllWildcards)
            .ToList();
      }

      else
      {
        listOfPost = DataSession.Query<Post, Posts_View>().OrderByDescending(o => o.Created).ToList();
      }

      if (Request.IsAjaxRequest())
        return PartialView("_ListOfPosts", listOfPost);

      return View(listOfPost);
    }

    // GET: Posts/Details/5
    public ActionResult Details(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var post = DataSession.Load<Post>("posts/" + id);

      if (post == null)
      {
        return HttpNotFound();
      }

      return View(post);
    }

    // GET: Posts/Create
    public ActionResult Create(string id)

    {
      var blogs = DataSession.Advanced.LoadStartingWith<Blog>("blogs/" + id);

      ViewBag.blog = new SelectList(blogs, "Id", "BlogName");
      return View();
    }

    // POST: Posts/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Post post)
    {
      try
      {
        if (ModelState.IsValid)
        {
          var listOfTags = SeparateTags(post.Tags);

          post.InternalTags = listOfTags;

          post.FriendlyUrl = SlugConverter.TitleToSlug(post.Title);
          // TODO check if this url is uniqe

          DataSession.Store(post);
          DataSession.SaveChanges();

          TempData["success"] = "Inlägget sparat";
          UpdateModel("Index");
          return RedirectToAction("Index");
        }
      }

      catch (Exception)
      {
        TempData["error"] = "MIsslyckades med att publicera inlägget";
      }

      return View(post);
    }

    private List<string> SeparateTags(string tags)
    {
      var tagArr = tags.Split(',');

      return tagArr.Select(tag => Regex.Replace(tag, @"\s+", "")).ToList();
    }

    // GET: Posts/Edit/5
    public ActionResult Edit(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var post = DataSession.Load<Post>("posts/" + id);
      var blogs = DataSession.Advanced.LoadStartingWith<Blog>("blogs/" + id);
      ViewBag.blog = new SelectList(blogs, "BlogName");
      foreach (var tag in post.InternalTags)
      {
        post.Tags += tag + ", ";
      }

      if (post == null)
      {
        throw new HttpException(404, "Posten du begärde finns inte eller har just blivit borttagen");
      }
      return View(post);
    }

    // POST: Posts/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Post post)
    {
      if (ModelState.IsValid)
      {
        try
        {
          var listOfTags = SeparateTags(post.Tags);

          post.InternalTags = listOfTags;

          DataSession.Store(post, "posts/" + post.Id);
          DataSession.SaveChanges();
          TempData["success"] = string.Format("Uppdaterade posten {0}", post.Title);
          UpdateModel("Index");
          return RedirectToAction("Index");
        }
        catch (Exception)
        {
          TempData["error"] = string.Format("Misslyckades att uppdatera {0}", post.Title);
        }
      }
      return View(post);
    }

    // GET: Posts/Delete/5
    public ActionResult Delete(string id)
    {
      if (id == null)
      {
        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
      }
      var post = DataSession.Load<Post>("posts/" + id);
      if (post == null)
      {
        return HttpNotFound();
      }
      return View(post);
    }

    // POST: Posts/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(string id)
    {
      try
      {
        var post = DataSession.Load<Post>("posts/" + id);
        DataSession.Delete(post);
        DataSession.SaveChanges();

        TempData["success"] = "Inlägget är nu borttagen";
        UpdateModel("Index");

        return RedirectToAction("Index");
      }
      catch (Exception)
      {
        TempData["error"] = "Borttagning av posten misslyckades!";
        RedirectToAction("Delete", new {id});
      }
      return RedirectToAction("Index");
    }
  }
}