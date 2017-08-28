using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class CustomerLoginInformationController : Controller
    {
        // GET: CustomerLoginInformation
        public ActionResult Index(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                return RedirectToAction("Index", "CustomerInformation");
            }

            int EntityId = 0;
            ViewBag.EntityId = 0;
            if (int.TryParse(Request["EntityId"], out EntityId))
            {
                ViewBag.EntityId = EntityId;
            }

            ViewBag.CustomerOfficeId = Id.ToString();
            return View();
        }
    }
}