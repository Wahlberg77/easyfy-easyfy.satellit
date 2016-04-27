using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Easyfy.Data.RavenDb;
using Easyfy.Satellit.Admin.Models;
using Easyfy.Satellit.Model.Posts;

namespace Easyfy.Satellit.Admin.Controllers
{
    public class BlogsController : RavenController
    {
        private EasyfySatellitAdminContext db = new EasyfySatellitAdminContext();

        // GET: Blogs
        public ActionResult Index()
        {
            return View(DataSession.Query<Blog>().ToList());
        }

        // GET: Blogs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blog = DataSession.Load<Blog>("blogs/"+id);

              if (blog == null)
              {
                  return HttpNotFound();
              }
            return View(blog);
        }

        // GET: Blogs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Blogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Blog blog)
        {
            if (ModelState.IsValid)
            {
              blog.CreatedAt = DateTime.UtcNow;
                //if (blog.Banner != null)
                //  {
                //    var photo = new byte[blog.Banner.ContentLength];
                //    blog.Banner.InputStream.Read(photo, 0, blog.Banner.ContentLength);
                //  }
              DataSession.Store(blog);
              DataSession.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(blog);
        }

        // GET: Blogs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blog = DataSession.Load<Blog>(("blogs/" + id));
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Blog blog)
        {
            if (ModelState.IsValid)
            {
                DataSession.Store(blog, "blogs/" + blog.Id);
                DataSession.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(blog);
        }

        // GET: Blogs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var blog = DataSession.Load<Blog>(("blogs/" + id));
            if (blog == null)
            {
                return HttpNotFound();
            }
            return View(blog);
        }

        // POST: Blogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
          var blog = DataSession.Load<Blog>(("blogs/" + id));
            DataSession.Delete(blog);
            DataSession.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
