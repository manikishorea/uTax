using EMPPortal.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EMPPortal.Utilities;
using System.Configuration;
using System.Net.Http;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class CustomerInformationController : Controller
    {
        // GET: CustomerInformation
        public ActionResult Index()
        {

            ViewResult vr = View();

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


            if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            {
                return View();
            }
            else if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SO || Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SOME || Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SOME_SS)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            {
                return RedirectToAction("OfficeInformation", "Enrollment");
            }
            else if (Session["PARENTID"].ToString() != "")
            {
                if (Convert.ToInt32(Session["IsActivationCompleted"]) == 0)
                {
                    return RedirectToAction("Dashboard", "SubSiteOfficeConfiguration");
                }
                else
                {
                    return RedirectToAction("OfficeInformation", "Enrollment");
                }
            }
            else
            {
                if (Convert.ToInt32(Session["IsActivationCompleted"]) == 0)
                {
                    return RedirectToAction("Dashboard", "CustomerInformation");
                }
                else
                {
                    return RedirectToAction("OfficeInformation", "Enrollment");
                }
            }
        }

        public ActionResult Create(string Id)
        {
            //if (string.IsNullOrEmpty(Id))
            //{
            //    Id = Guid.Empty.ToString();
            //}

            //ViewBag.Id = Id;

            //ViewResult vr = View();
            //if (!string.IsNullOrEmpty(Id))
            //{
            //    vr.MasterName = "_EnrollmentLayout";
            //}
            //else
            //{
            //    vr.MasterName = "_Layout";
            //}

            //return vr;


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

            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.Empty.ToString();
            }

            ViewBag.Id = Id;

            return vr;
        }

        public ActionResult ViewCustomer(string Id)
        {
            ViewResult vr = View();
            if (!string.IsNullOrEmpty(Id))
            {
                vr.MasterName = "_Layout";

                int EntityId = 0;
                ViewBag.EntityId = 0;
                if (int.TryParse(Request["EntityId"], out EntityId))
                {
                    ViewBag.EntityId = EntityId;
                }
            }

            return vr;
        }

        // GET: Main Office & Software Identification
        public ActionResult Dashboard()
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
            else if (Session["PARENTID"].ToString() != "")
            {
                return RedirectToAction("SubSiteDashboard", "CustomerInformation");
            }
            else
            {
                return View();
                // return RedirectToAction("Dashboard", "CustomerInformation");
            }

            return View();
        }

        public ActionResult AllCustomerInfo123(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.Empty.ToString();
            }

            ViewBag.Id = Id;
            return View();
        }

        public ActionResult CreateSubSiteInfo(string ParentId)
        {
            if (string.IsNullOrEmpty(ParentId))
            {
                ParentId = Guid.Empty.ToString();
            }
            ViewBag.ParentId = ParentId;
            return View();
        }

        public ActionResult NoPermissions()
        {
            return View();
        }

        public ActionResult Customers(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.Empty.ToString();
            }

            ViewBag.Id = Id;
            return View();
        }

        public ActionResult Customerslazyload(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.Empty.ToString();
            }

            ViewBag.Id = Id;
            return View();
        }

        public ActionResult AllCustomerInfo(string Id)
        {
            if (string.IsNullOrEmpty(Id))
            {
                Id = Guid.Empty.ToString();
            }
            ViewBag.Id = Id;
            return View();
        }

        [HttpPost]
        public ActionResult LoadData(string Parameters, int type, FormCollection collection)
        {
            //Get parameters
            EMPPortal.Models.CustomerInfoNewGrid ocustomerlist = new Models.CustomerInfoNewGrid();
            // get Start (paging start index) and length (page size for paging)            
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();

            //Get Sort columns value
            //var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            //var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();

            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;
           // int totalRecords = 0;

            using (var client = new HttpClient())
            {

                string Token = Session["Token"].ToString();
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Token", Token);

                List<string> lstparameters = Parameters.Split('^').ToList();
                EMPPortal.Models.CustomerInfoNewGrid ocustomer = new EMPPortal.Models.CustomerInfoNewGrid();
                //EMPPortal.Models.CustomerInfoNewGrid
                if (lstparameters.Count > 1)
                {
                    Guid newguid = Guid.Empty;
                    string struserid = lstparameters[0];
                    if (struserid != "")
                    {
                        newguid = new Guid(struserid);
                    }
                    ocustomer.UserId = newguid;
                    ocustomer.Status = lstparameters[1] == "" ? null : lstparameters[1];
                    ocustomer.SiteType = lstparameters[2] == "" ? null : lstparameters[2];
                    ocustomer.BankPartner = lstparameters[3] == "" ? null : lstparameters[3];
                    ocustomer.EnrollmentStatus = lstparameters[4] == "" ? null : lstparameters[4];
                    ocustomer.OnBoardingStatus = lstparameters[5] == "" ? null : lstparameters[5];
                    ocustomer.SearchText = lstparameters[6] == "" ? null : lstparameters[6];
                    ocustomer.SearchType = Convert.ToInt32(lstparameters[7]);
                }
                else
                {
                    if (lstparameters.Count == 1)
                    {
                        Guid newguid = Guid.Empty;
                        string struserid = Parameters;
                        if (struserid != "")
                        {
                            newguid = new Guid(struserid);
                        }
                        ocustomer.UserId = newguid;
                    }
                }

                if (Convert.ToInt32(Session["entityid"].ToString()) != (int)EMPPortalConstants.Entity.uTax)
                {
                    ocustomer.UserType = 1;
                }

                ocustomer.draw = draw;
                ocustomer.start = start;
                ocustomer.length = length;
                ocustomer.Customerlst = null;
                ocustomer.GridType = type;
                //ocustomer.UserId = "";
                var gizmo = ocustomer;// new EMPPortal.Models.CustomerInfoNewGrid { draw = draw, start = start, length = length, Customerlst = null, };
                var response = client.PostAsJsonAsync("api/OfficeManagement/get", gizmo).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = response.Content.ReadAsStringAsync().Result;
                    //dynamic jsonResponse = JsonConvert.DeserializeObject(message);// json.Deserialize(message); //var jss = new JavaScriptSerializer();
                    ocustomerlist = json.Deserialize<EMPPortal.Models.CustomerInfoNewGrid>(message);
                }
            }

            return Json(new { draw = ocustomerlist.draw, recordsFiltered = ocustomerlist.recordsTotal, recordsTotal = ocustomerlist.recordsTotal, data = ocustomerlist.Customerlst }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AddSO()
        {
            return View();
        }
    }
}