using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Index()
        {
            return View();
        }

        // GET: Account
        public ActionResult Logout()
        {
            Session.Abandon();
            Session.Contents.RemoveAll();
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Clear();

            return RedirectToAction("Index", "CustomerLogin");
        }
    }
}