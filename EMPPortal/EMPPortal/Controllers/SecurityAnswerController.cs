using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class SecurityAnswerController : Controller
    {
        // GET: SecurityAnswer
        public ActionResult Index(string status)
        {
            ViewBag.status = status;
            return View();
        }

        // GET: SecurityAnswer
        public ActionResult mysecuritychanges(string status)
        {
            ViewBag.status = status;
            return View();
        }
    }
}