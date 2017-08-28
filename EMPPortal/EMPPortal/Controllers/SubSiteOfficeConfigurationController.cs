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
using Newtonsoft.Json;
using System.IO;

namespace EMPPortal.Controllers
{
    [SessionCheck]
    public class SubSiteOfficeConfigurationController : Controller
    {
        // GET: SubSite
        //public ActionResult Dashboard()
        //{
        //    return View();
        //}

        public ActionResult Dashboard(string Id = "")
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

        public ActionResult Create(string Id = "", string type = "")
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

            if (!string.IsNullOrEmpty(Id))
            {
                Guid UserId;
                if(Guid.TryParse(Id,out UserId)){
                    Id = UserId.ToString();
                }
                else
                {
                    Id = Guid.Empty.ToString();
                }
            }
            else
            {
                Id = Guid.Empty.ToString();
            }

           
           
            return vr;
        }

        public ActionResult SubSiteOfficeConfig(string Id = "")
        {
          //  bool IsSubSiteLayout = false;
            bool IsSubSiteEFINAllow = false;

            ViewResult vr = View();

            if (Request["entitydisplayid"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        //IsSubSiteLayout = true;
                        vr.MasterName = "_EnrollmentLayout";
                        IsSubSiteEFINAllow = GetWebAPISubSiteConfig(Request["Id"].ToString()).Result;
                        vr.MasterName = "_EnrollmentLayout";
                    }
                }
            }

            //if (!IsSubSiteLayout)
            //{
            //    vr.MasterName = "_Layout";
            //    IsSubSiteEFINAllow = GetWebAPISubSiteConfig(Session["PARENTID"].ToString()).Result;
            //}

            ViewBag.IsSubSiteEFIN = IsSubSiteEFINAllow;
            return vr;
        }

        public ActionResult SubSiteOfficeFeeConfig(string Id = "")
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

        public ActionResult CustomerNotes(string Id = "")
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

        public ActionResult ActivateMyAccount(string Id = "")
        {
            bool IsSubSiteLayout = false;
            bool IsSubSiteEFINAllow = false;

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
                        IsSubSiteEFINAllow = GetWebAPISubSiteConfig(Request["Id"].ToString()).Result;
                    }
                }
            }


            if (!IsSubSiteLayout)
            {
                vr.MasterName = "_Layout";
                IsSubSiteEFINAllow = GetWebAPISubSiteConfig(Session["PARENTID"].ToString()).Result;
            }
            ViewBag.IsSubSiteEFIN = IsSubSiteEFINAllow;
            return vr;
        }

        public ActionResult ArchiveInformation(string Id = "")
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

        private async Task<bool> GetWebAPISubSiteConfig(string parentid)
        {
            bool IsSubSiteEFINAllow = false;
            string IsSubSiteEFINAllow1 = string.Empty;
            using (var client = new HttpClient())
            {
                string Token = Session["Token"].ToString();
                Guid Id;
                Guid.TryParse(parentid, out Id);
                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Token", Token);

                var response = client.GetAsync("api/SubSiteConfig/Main/" + Id).Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    dynamic jsonResponse = JsonConvert.DeserializeObject(message);// json.Deserialize(message);
                    if (jsonResponse == null)
                        IsSubSiteEFINAllow1 = "";
                    else
                        IsSubSiteEFINAllow1 = jsonResponse.IsSubSiteEFINAllow;

                    var jss = new JavaScriptSerializer();
                    // string SRjson = new StreamReader(context.Request.InputStream).ReadToEnd();
                    //   Dictionary<string, string> sData = jss.Deserialize<Dictionary<string, string>>(message);
                    //   string _Name = sData["IsSubSiteEFINAllow"].ToString();

                    //  _Model = json.Deserialize<string[]>(message);
                }

                // var gizmo = new LoginModel() { CrossLinkUserId = username, EMPPassword = password };
                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                // var response1 = client.PostAsJsonAsync("api/CustomerLogin", gizmo).Result;

                //if (response1.IsSuccessStatusCode)
                //{
                //    var json = new JavaScriptSerializer();
                //    string message = await response1.Content.ReadAsStringAsync();
                //    _LoginModel = json.Deserialize<LoginModel>(message);
                //}
                //else
                //{
                //    // return await response.Content.ReadAsAsync<U>();
                //}
            }

            if (bool.TryParse(IsSubSiteEFINAllow1, out IsSubSiteEFINAllow))
            {
                return IsSubSiteEFINAllow;
            }
            else
            {
                return false;
            }
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

        public ActionResult ActivateAccount(string Id = "")
        {
            bool IsSubSiteLayout = false;
            bool IsSubSiteEFINAllow = false;
            ViewResult vr = View();

            if (Request["entitydisplayid"] != null && Request["ParentId"] != null)
            {
                int entitydisid = 0;
                if (int.TryParse(Request["entitydisplayid"], out entitydisid))
                {
                    if (Convert.ToInt32(Session["entitydisplayid"].ToString()) != Convert.ToInt32(Request["entitydisplayid"].ToString()))// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                    {
                        IsSubSiteLayout = true;
                        vr.MasterName = "_EnrollmentLayout";
                        IsSubSiteEFINAllow = GetWebAPISubSiteConfig(Request["ParentId"].ToString()).Result;
                    }
                }
            }

            if (!IsSubSiteLayout)
            {
                vr.MasterName = "_Layout";
                IsSubSiteEFINAllow = GetWebAPISubSiteConfig(Session["PARENTID"].ToString()).Result;
            }
            ViewBag.IsSubSiteEFIN = IsSubSiteEFINAllow;

            return vr;
        }
    }
}