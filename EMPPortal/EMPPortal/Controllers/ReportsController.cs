using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult FeeReport()
        {
            return View();
        }

        public ActionResult NoBankAppSubmission()
        {
            return View();
        }

        public ActionResult LastLoginInfo()
        {
            return View();
        }

        public ActionResult OpenEnrollment()
        {
            return View();
        }
        public ActionResult NewEnrollment()
        {
            return View();
        }
        public ActionResult StaleEnrollment()
        {
            return View();
        }
        public ActionResult ClosedEnrollment()
        {
            return View();
        }

        public ActionResult EnrollmentStatus()
        {
            return View();
        }
    }
}