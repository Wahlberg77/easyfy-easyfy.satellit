using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Easyfy.Satellit.Model;
using Easyfy.Satellit.Model.Posts;

namespace SandladanWebb.Controllers
{
  public class HomeController : Controller
  {
    public ActionResult Index()
    {
      ViewBag.Title="Sandlådor";
      
      return View();
    }
  }
}