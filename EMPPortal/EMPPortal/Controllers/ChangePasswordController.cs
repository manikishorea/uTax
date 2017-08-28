using EMPPortal.Filter;
using EMPPortal.Models;
using EMPPortal.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EMPPortal.Controllers
{

    public class ChangePasswordController : Controller
    {
        // GET: ChangePassword
        [SessionCheck]
        public ActionResult InitialPassword()
        {
            return View();
        }

        // GET: ChangePassword
        [SessionCheck]
        [HttpPost]
        public ActionResult InitialPassword(string NewPassword)
        {
            string UserId = Session["UserId"].ToString();
            var result = GetWebAPIInitial(NewPassword, UserId);


            if (result.Result)
            {
                //if (Session["PARENTID"].ToString() != "")
                //{
                //    return RedirectToAction("Dashboard", "SubSiteOfficeConfiguration");
                //}
                //else
                //{
                //    return RedirectToAction("Index", "CustomerInformation");
                //}

                if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                {
                    return RedirectToAction("Index", "CustomerInformation");
                }
                else if (Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SO || Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SOME || Convert.ToInt32(Session["entityid"].ToString()) == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SOME_SS)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
                {
                    return RedirectToAction("OfficeInformation", "Enrollment");
                }
                else if (Session["PARENTID"].ToString() != "")
                {
                    return RedirectToAction("Dashboard", "SubSiteOfficeConfiguration");
                }
                else
                {
                    return RedirectToAction("Dashboard", "CustomerInformation");
                }
            }
            else
            {
                TempData["error"] = "Password has not changed.";
            }

            return View();
        }

        public async Task<bool> GetWebAPIInitial(string password, string UserId)
        {
            bool _Result = false;
            using (var client = new HttpClient())
            {
                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var gizmo = new LoginModel { EMPPassword = password, Id = UserId };

                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                var response = client.PostAsJsonAsync("api/ChangePassword/initial", gizmo).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    _Result = json.Deserialize<bool>(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }
            }

            return _Result;
        }

        // GET: ChangePassword
        [SessionCheck]
        public ActionResult mypassword()
        {
            return View();
        }

        // GET: ChangePassword
        [SessionCheck]
        [HttpPost]
        public ActionResult mypassword(string CurrentPassword, string NewPassword)
        {
            string UserId = Session["UserId"].ToString();
            var result = GetWebAPIUpdate(CurrentPassword, NewPassword, UserId);

            if (result.Result)
            {
                TempData["success"] = "Password has changed successfully.";
                return RedirectToAction("mypassword", "ChangePassword");
            }
            else
            {
                TempData["error"] = "Password has not changed.";
            }

            return View();
        }

        public async Task<bool> GetWebAPIUpdate(string CurrentPassword, string password, string UserId)
        {
            bool _Result = false;
            using (var client = new HttpClient())
            {
                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var gizmo = new LoginModel { CurrentPassword = CurrentPassword, EMPPassword = password, Id = UserId };

                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                var response = client.PostAsJsonAsync("api/ChangePassword/update", gizmo).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    _Result = json.Deserialize<bool>(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }
            }

            return _Result;
        }

        public ActionResult ResetPassword(string UserId)
        {
            ViewBag.UserId = UserId;
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string NewPassword, string UserId)
        {
            string UserName = Request["UserId"];
            if (!string.IsNullOrEmpty(UserName))
            {
                UserName = UserId;
            }

            var result = GetWebAPIResetPassword(NewPassword, UserId);

            if (result.Result)
            {
                TempData["success"] = "Password has changed successfully.";
                return RedirectToAction("ResetPassword", "ChangePassword", new { UserId = UserName });
            }
            else
            {
                TempData["error"] = "Password has not changed.";
            }

            return View();
        }

        public async Task<bool> GetWebAPIResetPassword(string password, string UserId)
        {
            bool _Result = false;
            using (var client = new HttpClient())
            {
                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var gizmo = new LoginModel { EMPPassword = password, EMPUserId = UserId };

                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                var response = client.PostAsJsonAsync("api/ChangePassword/reset", gizmo).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    _Result = json.Deserialize<bool>(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }
            }

            return _Result;
        }

        public string SkipPassword(Guid CustomerId)
        {
            if (Session["IsActivationCompleted"] != null)
            {
                if (Convert.ToInt32(Session["IsActivationCompleted"]) == 1 && (Convert.ToInt32(Session["entityid"]) == (int)EMPPortalConstants.Entity.MO || Convert.ToInt32(Session["entityid"]) == (int)EMPPortalConstants.Entity.SVB))
                {
                    if (new Guid(Session["SubBankId"].ToString()) == Guid.Empty)
                        return "/CustomerInformation/AllCustomerInfo";
                    else
                        return "/CustomerInformation/AllCustomerInfo?bankid=" + Session["SubBankId"].ToString();
                }
                else if(Convert.ToInt32(Session["entityid"]) == (int)EMPPortalConstants.Entity.MO || Convert.ToInt32(Session["entityid"]) == (int)EMPPortalConstants.Entity.SVB)
                {
                    return "/CustomerInformation/Dashboard";
                }
                else
                {
                    return "/Enrollment/OfficeInformation";
                }
            }
            else
                return "/CustomerInformation/Dashboard";
        }
    }
}