﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMPAdmin.Filters;

namespace EMPAdmin.Controllers
{
    [SessionCheckAttribute]
    public class TooltipController : Controller
    {
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        // GET: User/Create
        public ActionResult Create(string Id)
        {
            if (string.IsNullOrEmpty(Id))
              return  RedirectToAction("index");
               // Id = Guid.Empty.ToString();

            ViewBag.Id = Id;
            return View();
        }
        
    }
}
