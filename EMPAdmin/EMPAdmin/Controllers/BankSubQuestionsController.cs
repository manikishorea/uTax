using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using EMPAdmin.Filters;

namespace EMPAdmin.Controllers
{
    [SessionCheckAttribute]
    public class BankSubQuestionsController : BaseController
    {
        public ActionResult Index(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return RedirectToAction("/bank/Index");

            ViewBag.Id = Id;
            return View();
        }

        public ActionResult Create(string Id,string BankId,string BankName)
        {
            if (string.IsNullOrEmpty(BankId) && string.IsNullOrEmpty(BankName))
                return RedirectToAction("Index");

            if (string.IsNullOrEmpty(Id))
                Id = Guid.Empty.ToString();

            ViewBag.Id = Id;
            ViewBag.BankId = BankId;
            ViewBag.BankName = BankName;
            return View();
        }
    }
}
