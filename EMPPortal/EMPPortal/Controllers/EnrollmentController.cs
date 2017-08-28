using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class EnrollmentController : Controller
    {
        // GET: Enrollment
        public ActionResult Index()
        {
            return View();
        }

        // GET: Enrollment
        public ActionResult OfficeInformation(string Id = "")
        {
            bool IsSubSiteLayout = false;

            ViewResult vr = View();

            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()) || Session["UserId"].ToString()==Id)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
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

        // GET: Enrollment
        public ActionResult OfficeConfiguration(string Id = "")
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

        // GET: Enrollment
        public ActionResult AffiliateConfiguration(string Id = "")
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

        public ActionResult BankSelectionFeeDetails(string Id = "")
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

        public ActionResult EnrollmentFeeReimbursement(string Id = "")
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
        // GET: Enrollment
        public ActionResult BankEnrollment(string Id = "")
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

        // GET: Enrollment
        public dynamic BankEnrollmentTPG()
        {
            return PartialView("BankEnrollmentTPG");
        }

        // GET: Enrollment
        public dynamic BankEnrollmentRA()
        {
            return PartialView("BankEnrollmentRA");
        }

        // GET: Enrollment
        public dynamic BankEnrollmentRB()
        {
            return PartialView("BankEnrollmentRB");
        }

        public ActionResult EnrollmentSummary(string Id = "")
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

        public ActionResult ViewBankEnrollment()
        {
            return View();
        }

        public ActionResult BankSelection()
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

        // GET: Enrollment
        public dynamic BankObjectsTPG()
        {
            return PartialView("BankObjectsTPG");
        }

        // GET: Enrollment
        public dynamic BankObjectsRA()
        {
            return PartialView("BankObjectsRA");
        }

        // GET: Enrollment
        public dynamic BankObjectsRB()
        {
            return PartialView("BankObjectsRB");
        }



        // GET: Enrollment
        public dynamic ArcBankEnrollmentTPG()
        {
            return PartialView("ArcBankEnrollmentTPG");
        }

        // GET: Enrollment
        public dynamic ArcBankEnrollmentRA()
        {
            return PartialView("ArcBankEnrollmentRA");
        }

        // GET: Enrollment
        public dynamic ArcBankEnrollmentRB()
        {
            return PartialView("ArcBankEnrollmentRB");
        }
    }
}