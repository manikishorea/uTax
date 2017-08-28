using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //if(Session["UserName"]== null)
            //{
            //    return RedirectToAction("Login","Account");
            //}
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
