using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.WebPages;
using Easyfy.Data.RavenDb;
using Easyfy.Data.RavenDb.Indexes;
using Easyfy.Satellit.Admin.Models;
using Easyfy.Satellit.Model.Comment;
using Easyfy.Satellit.Model.Posts;
using Raven.Client;

namespace Easyfy.Satellit.Admin.Controllers
{
  public class CommentsController : RavenController
  {
    public ActionResult Index()
    {
      var posts = DataSession.Query<Comments_ByPosts.CommentBlah, 
        Comments_ByPosts>().ProjectFromIndexFieldsInto<Comments_ByPosts.CommentBlah>()
          .Where(o=>o.IsPublished == false)
            .ToList();

      var model = new CommentVm {MyTables = posts};

      return View(model);
    }

    //POST: Create Comment
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Comment comment)
    {
      try
      {

        if (ModelState.IsValid)
        {
          DataSession.Store(comment);
          DataSession.SaveChanges();

          TempData["success"] = "Kommentaren skickad för granskning";
          return RedirectToAction("Index");
        }
      }

      catch (Exception)
      {
        TempData["error"] = "MIsslyckades med att publicera inlägget";
      }
      return View("Index");
    }

    //POST: Delete Comment
    public ActionResult DeleteConfirmed(string postId, string commentId)
    {
      var post = DataSession.Load<Post>((Post.Idprefix + postId));

      for (var i = 0; i < post.Comments.Count; i++)
      {
        if (post.Comments[i].Id == commentId)
        {
          post.Comments.RemoveAt(i);
        }
      }
      DataSession.SaveChanges();
      UpdateModel("Index");
      return RedirectToAction("Index");
    }

    //Kommentaren godkänns och publiceras på bloggen. 
    public ActionResult Confirm(string postId, string commentId)
    {
      var post = DataSession.Load<Post>((Post.Idprefix + postId));

      foreach (var t in post.Comments.Where(t => t.Id == commentId))
      {
        t.IsPublished = true;
      }
      DataSession.SaveChanges();
      UpdateModel("Index");
      return RedirectToAction("Index");
    }
  }
}