using System.Net.Http;
using System.Web;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Web.Mvc;
using System.Web.Security;
using System.Data.Entity;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Models;
using EMPAdmin.Utilities;
using EMPAdmin.Filters;
using System.Net;
using System.IO;
using System;
using System.Web.Script.Serialization;
using System.Configuration;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace EMPAdmin.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login

        public ActionResult Login(string returnUrl)
        {
            Session.Clear();
            Session.Abandon();
            Response.Cookies.Clear();
            //Response.Cookies.Add(new HttpCookie("ASP.NET_SessionId", ""));
            //LoginDTO model = new LoginDTO();
            ViewBag.ReturnUrl = returnUrl;
            // FormsAuthentication.SignOut();
            Session.Abandon();
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Login(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                TempData["error"] = "Bad Request";
                return View();
            }
           // string PasswordHash = PasswordManager.HashText(password, "Kensium");
            var result = GetWebAPIAccessToken(username, password);
            if (result.Result == null)
            {
                TempData["error"] = "Invalid Login";
            }

            if (result.Result.Id == Guid.Empty)
            {
                TempData["error"] = "Invalid Login";
            }

            if (result.Result.Id != Guid.Empty)
            {
                //if (model.RememberMe)
                //{
                FormsAuthentication.SetAuthCookie(username, true);
                Response.Cookies["Login"]["UserName"] = username;

                Session.Add("Token", result.Result.Token.AuthToken);
                Session.Add("UserId", result.Result.Id);
                Session.Add("UserName", result.Result.UserName);
                Session.Add("DisplayName", result.Result.Name);

                // uTax.Portal.APIs.RoleMastersController role = new uTax.Portal.APIs.RoleMastersController();
                //  var roles = role.GetRoleMaster(user.Id);
                var roles = GetWebAPIRoles(result.Result.Id);

                StringBuilder sb = new StringBuilder();
                if (result.Result.UserName != "admin")
                {
                    foreach (var item in roles.Result)
                    {
                        sb.AppendFormat("{0}_", item.Id);
                    }

                    string roleIds = sb.ToString().Substring(0, sb.Length - 1);
                    Session.Add("UserRoleId", roleIds);
                    Session.Add("UserRoles", roleIds);

                    var permissions = GetWebAPIPermissions(result.Result.Id, roles.Result);
                    sb = new StringBuilder();
                    foreach (var item in permissions.Result)
                    {
                        sb.AppendFormat("{0}:{1}_", item.SiteMapId, item.PermissionName);
                    }

                    string permissionIds = sb.ToString().Substring(0, sb.Length - 1);
                    Session.Add("UserPermissions", permissionIds);
                }
                else {
                    Session.Add("UserRoleId", 1);
                    Session.Add("UserRoles", 1);
                    Session.Add("UserPermissions", 1);
                }
                return RedirectToAction("Index", "Dashboard");
            }

            return View();
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string eMail)
        {
            return View();
        }


        public LoginModel GetEMPWebAPIAccessToken(string username, string password)
        {
            LoginModel _LoginModel = new LoginModel();
            string EMPAdminWebAPI_uri = ConfigurationManager.AppSettings["EMPAdminWebAPI"];
            string statusCode = "OK";

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var json = new JavaScriptSerializer();

                HttpWebRequest WebReq = (HttpWebRequest)WebRequest.Create(@"http://localhost:9001/api/Account");
                WebReq.Method = "POST";
                WebReq.ContentType = "application/x-www-form-urlencoded";
                string param = "username=" + username + "&password=" + password;
                byte[] buffer = Encoding.ASCII.GetBytes(param);// ("test=postvar&test2=another");
                WebReq.ContentLength = buffer.Length;
                Stream dataStream = WebReq.GetRequestStream();
                dataStream.Write(buffer, 0, buffer.Length);
                dataStream.Close();

                try
                {
                    WebResponse webres = WebReq.GetResponse();
                    statusCode = ((HttpWebResponse)webres).StatusCode.ToString();
                    //Console.WriteLine(((HttpWebResponse)webres).StatusDescription);
                    dataStream = webres.GetResponseStream();
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(responseFromServer);
                    reader.Close();
                    dataStream.Close();
                    webres.Close();
                }
                catch (WebException ex)
                {
                    statusCode = ((HttpWebResponse)ex.Response).StatusCode.ToString();
                    string statusDiscr = ((HttpWebResponse)ex.Response).StatusDescription.ToString();
                }
            }

            return _LoginModel;
        }

        public async Task<LoginModel> GetWebAPIAccessToken(string username, string password)
        {
            LoginModel _LoginModel = new LoginModel(); ;
            using (var client = new HttpClient())
            {

                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPAdminWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // New code:
                //HttpResponseMessage response = await client.GetAsync("api/account/1");
                //if (response.IsSuccessStatusCode)
                //{
                //    Product product = await response.Content.ReadAsAsync > Product > ();
                //    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                //}

                var gizmo = new LoginModel() { UserName = username, Password = password };
                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                //  var obj = new { username = username, password = password };
                // var content = new StringContent(JsonConvert.SerializeObject(obj).ToString(), Encoding.UTF8, "application/json");
                var response = client.PostAsJsonAsync("api/account", gizmo).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    // var pp = response.Content.ReadAsAsync<LoginModel>();
                    //  Dictionary<string, object> dict = json.Deserialize<Dictionary<string, object>>(message);

                    _LoginModel = json.Deserialize<LoginModel>(message);
                    // throw new Exception(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }

                //  Uri gizmoUrl = response.Headers.Location;
                // HTTP PUT
                // gizmo.Price = 80;   // Update price
                //  response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                // HTTP DELETE
                //  response = await client.DeleteAsync(gizmoUrl);


                // response.EnsureSuccessStatusCode();
                //  return response;
            }

            return _LoginModel;
        }

        public async Task<List<RoleModel>> GetWebAPIRoles(Guid UserId)
        {
            List<RoleModel> _Roles = new List<RoleModel>();


            using (var client = new HttpClient())
            {

                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPAdminWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Token", Session["Token"].ToString());
                // New code:
                //HttpResponseMessage response = await client.GetAsync("api/account/1");
                //if (response.IsSuccessStatusCode)
                //{
                //    Product product = await response.Content.ReadAsAsync > Product > ();
                //    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                //}

                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                //  var obj = new { username = username, password = password };
                // var content = new StringContent(JsonConvert.SerializeObject(obj).ToString(), Encoding.UTF8, "application/json");
                var response = client.GetAsync("api/userrolemap/" + UserId).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    // var pp = response.Content.ReadAsAsync<LoginModel>();
                    List<Dictionary<string, object>> dict = json.Deserialize<List<Dictionary<string, object>>>(message);

                    foreach (Dictionary<string, object> Dic in dict)
                    {
                        RoleModel _Role = new RoleModel();
                        _Role.Id = Dic["RoleId"].ToString();
                        _Role.Name = Dic["RoleName"].ToString();
                        _Roles.Add(_Role);
                    }
                    //  _LoginModel = json.Deserialize<LoginModel>(message);
                    // throw new Exception(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }

                //  Uri gizmoUrl = response.Headers.Location;
                // HTTP PUT
                // gizmo.Price = 80;   // Update price
                //  response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                // HTTP DELETE
                //  response = await client.DeleteAsync(gizmoUrl);


                // response.EnsureSuccessStatusCode();
                //  return response;
            }

            return _Roles;
        }

        public async Task<List<SitePermissionModel>> GetWebAPIPermissions(Guid UserId, List<RoleModel> Roles)
        {
            List<SitePermissionModel> _Permissions = new List<SitePermissionModel>();


            using (var client = new HttpClient())
            {

                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPAdminWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Token", Session["Token"].ToString());
                // New code:
                //HttpResponseMessage response = await client.GetAsync("api/account/1");
                //if (response.IsSuccessStatusCode)
                //{
                //    Product product = await response.Content.ReadAsAsync > Product > ();
                //    Console.WriteLine("{0}\t${1}\t{2}", product.Name, product.Price, product.Category);
                //}

                //  response = await client.PostAsJsonAsync("api/products", gizmo);

                var obj = new { UserId = UserId, Roles = Roles };
                // var content = new StringContent(JsonConvert.SerializeObject(obj).ToString(), Encoding.UTF8, "application/json");
                var response = client.PostAsJsonAsync("api/PermissionMasters", obj).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = await response.Content.ReadAsStringAsync();
                    // var pp = response.Content.ReadAsAsync<LoginModel>();
                    List<Dictionary<string, object>> dict = json.Deserialize<List<Dictionary<string, object>>>(message);

                    foreach (Dictionary<string, object> Dic in dict)
                    {
                        SitePermissionModel _Permission = new SitePermissionModel();
                        //   _Permission.Id = Dic["Id"].ToString();
                        _Permission.PermissionId = Dic["PermissionId"].ToString();
                        _Permission.PermissionName = Dic["PermissionName"].ToString();
                        _Permission.SiteMapId = Dic["SiteMapId"].ToString();
                        //    _Permission.SiteName = Dic["SiteName"].ToString();
                        _Permissions.Add(_Permission);
                    }
                    //  _LoginModel = json.Deserialize<LoginModel>(message);
                    // throw new Exception(message);
                }
                else
                {
                    // return await response.Content.ReadAsAsync<U>();
                }

                //  Uri gizmoUrl = response.Headers.Location;
                // HTTP PUT
                // gizmo.Price = 80;   // Update price
                //  response = await client.PutAsJsonAsync(gizmoUrl, gizmo);

                // HTTP DELETE
                //  response = await client.DeleteAsync(gizmoUrl);


                // response.EnsureSuccessStatusCode();
                //  return response;
            }

            return _Permissions;
        }

    }
}