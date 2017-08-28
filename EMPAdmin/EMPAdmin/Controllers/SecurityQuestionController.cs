using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPAdmin.Controllers
{
    public class SecurityQuestionController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                Id = Guid.Empty.ToString();

            ViewBag.Id = Id;
            return View();
        }
    }
}
