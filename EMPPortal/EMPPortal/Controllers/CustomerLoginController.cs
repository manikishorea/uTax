using EMPPortal.Filter;
using EMPPortal.Models;
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
    public class CustomerLoginController : Controller
    {
        // GET: ChangePassword
        [AllowAnonymous]
        public ActionResult Index()
        {
            //Session.Clear();
            //Session.Abandon();
            //Response.Cookies.Clear();
            return View();
        }


        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(string UserID, string Password, FormCollection collection)
        {
            if (string.IsNullOrEmpty(UserID) || string.IsNullOrEmpty(Password))
            {
                TempData["error"] = "Bad Request";
                return View();
            }
            var userip = collection["userip"];
            var result = GetWebAPIAccessToken(UserID, Password, userip);

            if (result.Result == null)
            {
                TempData["error"] = "Your EMP username and/or password could not be found or are invalid.";
                return View();
            }

            if (string.IsNullOrEmpty(result.Result.Id))
            {
                TempData["error"] = "Your EMP username and/or password could not be found or are invalid.";
                return View();
            }

            if(result.Result.Id==Guid.Empty.ToString() && !string.IsNullOrEmpty(result.Result.Message))
            {
                TempData["error"] = result.Result.Message;
                return View();
            }

            Session.Add("uTaxNotCollectingSBFee", result.Result.uTaxNotCollectingSBFee);
            Session.Add("EFINStatus", result.Result.EFINStatus);
            Session.Add("entityid", result.Result.EntityId);
            Session.Add("entitydisplayid", result.Result.BaseEntityId);
            Session.Add("Token", result.Result.Token.AuthToken);
            Session.Add("UserName", result.Result.CrossLinkUserId);
            Session.Add("LoginId", result.Result.Id); //Customer Login Information ID (CROSS LINK)
            Session.Add("UserId", result.Result.CustomerOfficeId);//Customer Office Information ID 
            Session.Add("EFIN", result.Result.EFIN);
            Session.Add("SECQUE", result.Result.IsSetSecurityQuestion);
            Session.Add("CHNPSW", result.Result.IsChangedPassword);
            Session.Add("PARENTID", result.Result.ParentID);
            //Session.Add("SUPPARENTID", result.Result.SupParentID);
            Session.Add("SalesYear", result.Result.SalesYearID);
            Session.Add("IsActivationCompleted", result.Result.IsActivationCompleted);
            Session.Add("IsMSOUser", result.Result.IsMSOUser);
            Session.Add("IsEnrollmentSubmit", result.Result.IsEnrollmentSubmit);
            Session.Add("CanSubSiteLoginToEmp", result.Result.CanSubSiteLoginToEmp);
            Session.Add("IsVerified", result.Result.IsVerified);
            Session.Add("EFINOwnerUserId", result.Result.EFINOwnerUserId);
            Session.Add("SubBankId", result.Result.BankId);
            Session.Add("IsHold", result.Result.IsHold);
            // return RedirectToAction("Index", "Home");
            if (!result.Result.IsSetSecurityQuestion)
            {
                //if (!result.Result.CanSubSiteLoginToEmp)
                //    return RedirectToAction("Index", "CustomerLogin");
                //else
                return RedirectToAction("Index", "SecurityAnswer", new { @status = "1" });
            }
            //"8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73"
            if (result.Result.EntityId == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.uTax)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            {
                if (result.Result.IsOfficeMgmt)
                    return RedirectToAction("AllCustomerInfo", "CustomerInformation");
                else if (result.Result.IsnewCustomers)
                    return RedirectToAction("Index", "CustomerInformation");
                else if (result.Result.FeeReport)
                    return RedirectToAction("FeeReport", "Reports");
                else if (result.Result.NoBankApp)
                    return RedirectToAction("NoBankAppSubmission", "Reports");
                else if (result.Result.Enrollstatus)
                    return RedirectToAction("EnrollmentStatus", "Reports");
                else if (result.Result.LoginReport)
                    return RedirectToAction("LastLoginInfo", "Reports");
                else
                    return RedirectToAction("NoPermissions", "CustomerInformation");
            }

            if (result.Result.EntityId == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SO || result.Result.EntityId == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SOME || result.Result.EntityId == (int)EMPPortal.Utilities.EMPPortalConstants.Entity.SOME_SS)// "8f4fca72-3d3c-4ad3-8f1c-1de3d602fd73")
            {
                return RedirectToAction("OfficeInformation", "Enrollment");
            }

            else if (!string.IsNullOrEmpty(result.Result.ParentID))
            {
                if (result.Result.IsActivationCompleted == 0)
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
                if (result.Result.IsActivationCompleted == 0)
                {
                    return RedirectToAction("Dashboard", "CustomerInformation");
                }
                else if (result.Result.IsActivationCompleted == 1)
                {
                    if (result.Result.BankId != Guid.Empty)
                        return RedirectToAction("AllCustomerInfo", "CustomerInformation", new { bankid = result.Result.BankId });
                    else
                        return RedirectToAction("AllCustomerInfo", "CustomerInformation");
                }
                else if (result.Result.IsTaxReturn)
                {
                    return RedirectToAction("OfficeInformation", "Enrollment");
                }
                else
                    return RedirectToAction("Dashboard", "Configuration");
            }
        }

        [SessionCheck]
        public ActionResult Logout()
        {
            using (var client = new HttpClient())
            {
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var response = client.PostAsJsonAsync("api/CustomerLogin/TokenKill?TokenID=" + Session["Token"].ToString(), "").Result;
                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = response.Content.ToString();
                    //_LoginModel = json.Deserialize<LoginModel>(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }
            }
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Clear();
            return RedirectToAction("Index", "CustomerLogin");
        }

        public async Task<LoginModel> GetWebAPIAccessToken(string username, string password, string userip)
        {
            LoginModel _LoginModel = new LoginModel();
            using (var client = new HttpClient())
            {

                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                // client.DefaultRequestHeaders.Add("X-USER-IP", userip);
                // New code:
                //HttpResponseMessage response = await client.GetAsync("api/account/1");
                //if (response.IsSuccessStatusCode)
                //{
                //    Product product = await response.Content.ReadAsAsync > Product > ();
                //    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                //}

                var gizmo = new LoginModel() { EMPUserId = username, EMPPassword = password, userip = userip };
                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                var response = client.PostAsJsonAsync("api/CustomerLogin", gizmo).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    _LoginModel = json.Deserialize<LoginModel>(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }

            }

            return _LoginModel;
        }
    }
}