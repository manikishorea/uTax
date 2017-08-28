using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class ConfigurationController : Controller
    {
        // GET: CustomerInformation
        public ActionResult Index()
        {
            if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            {
                return View();
            }
            else
            {
                return RedirectToAction("MainOffice_SoftwareIdentification", "CustomerInformation");
            }
        }

        // GET: CustomerInformation
        public ActionResult MainOfficeConfiguration(string Id = "")
        {
            ViewResult vr = View();
            if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            {
                vr.MasterName = "_EnrollmentLayout";
            }
            else
            {
                vr.MasterName = "_Layout";
            }
            return vr;
        }

        // GET: Sub Site Configuration
        public ActionResult SubsiteConfiguration(string Id = "")
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
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }

            //if (Convert.ToInt32(Session["entitydisplayid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            //{
            //    vr.MasterName = "_EnrollmentLayout";
            //}
            //else
            //{
            //    vr.MasterName = "_Layout";
            //}
            return vr;
        }
        // GET: Fee Setup Configuration
        public ActionResult FeesetupConfiguration(string Id = "")
        {
            //ViewResult vr = View();
            //if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            //{
            //    vr.MasterName = "_EnrollmentLayout";
            //}
            //else
            //{
            //    vr.MasterName = "_Layout";
            //}

            ViewResult vr = View();
            vr.MasterName = "_Layout";
            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }

            return vr;
        }

        // GET: Service Bureau Transmission
        public ActionResult ServiceBureauTransmission(string Id = "")
        {
            //ViewResult vr = View();
            //if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            //{
            //    vr.MasterName = "_EnrollmentLayout";
            //}
            //else
            //{
            //    vr.MasterName = "_Layout";
            //}

            ViewResult vr = View();
            vr.MasterName = "_Layout";
            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }
            return vr;
        }

        // GET: Service Bureau Transmission
        public ActionResult ActivateInformation(string Id = "")
        {
            //ViewResult vr = View();
            //if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            //{
            //    vr.MasterName = "_EnrollmentLayout";
            //}
            //else
            //{
            //    vr.MasterName = "_Layout";
            //}

            ViewResult vr = View();
            vr.MasterName = "_Layout";
            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }
            return vr;
        }

        public ActionResult Dashboard(string Id = "")
        {
            //ViewResult vr = View();
            //if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            //{
            //    vr.MasterName = "_EnrollmentLayout";
            //}
            //else
            //{
            //    vr.MasterName = "_Layout";
            //}

            ViewResult vr = View();
            vr.MasterName = "_Layout";
            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }
            return vr;
        }

        public ActionResult Create(string Id)
        {
            //ViewResult vr = View();
            //if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            //{
            //    vr.MasterName = "_EnrollmentLayout";
            //}
            //else
            //{
            //    vr.MasterName = "_Layout";
            //}

            ViewResult vr = View();
            vr.MasterName = "_Layout";
            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }

            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.Empty.ToString();
            }

            ViewBag.Id = Id;
            return vr;
        }
    }
}