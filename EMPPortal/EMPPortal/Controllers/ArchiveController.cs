using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class ArchiveController : Controller
    {
        // GET: Archive
        public ActionResult Index()
        {
            ViewResult vr = View();
            vr.MasterName = "_Layout";
            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        vr.MasterName = "_ArchiveLayout";
                    }
                }
            }
            return vr;
           // return View();
        }

        // GET: Archive
        public ActionResult SubSite()
        {
            ViewResult vr = View();
            vr.MasterName = "_Layout";
            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        vr.MasterName = "_ArchiveLayout";
                    }
                }
            }
            return vr;
            //return View();
        }
    }
}