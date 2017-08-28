using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    public class PaymentOptionsController : Controller
    {
        // GET: PaymentOptions
        public ActionResult efile(string Id = "")
        {
            bool IsSubSiteLayout = false;

            ViewResult vr = View();

            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        IsSubSiteLayout = true;
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }

            if (!IsSubSiteLayout)
            {
                vr.MasterName = "_Layout";
            }
            return vr;
        }

        public ActionResult OutstandingBalance(string Id = "")
        {
            bool IsSubSiteLayout = false;

            ViewResult vr = View();

            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        IsSubSiteLayout = true;
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }

            if (!IsSubSiteLayout)
            {
                vr.MasterName = "_Layout";
            }
            return vr;
        }
    }
}