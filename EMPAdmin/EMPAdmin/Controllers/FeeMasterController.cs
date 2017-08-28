using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Configuration;
using System.Threading.Tasks;
using EMPAdmin.Filters;

namespace EMPAdmin.Controllers
{
    [SessionCheckAttribute]
    public class FeeMasterController : BaseController
    {
        // GET: BankMaster
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