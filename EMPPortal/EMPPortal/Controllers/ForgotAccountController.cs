using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace EMPPortal.Controllers
{
    public class ForgotAccountController : Controller
    {
        // GET: ChangePassword
        public ActionResult Index()
        {
            return View();
        }

        // GET: ChangePassword
        public ActionResult Password()
        {
            ViewBag.UserId = "";
            ViewBag.Status = -1;
            return View();
        }

        // GET: ChangePassword
        [HttpPost]
        public ActionResult Password(string UserId)
        {
            ForgotAccountModel status = GetWebAPIForUserId(UserId);
            ViewBag.Status = Convert.ToInt32(status.Status);
            ViewBag.UserId = UserId;
            if (ViewBag.Status == -1)
            {
                TempData["error"] = "Your EMP UserId could not be found.";
            }

            if (ViewBag.Status == 1)
            {
                ViewBag.Question1 = status.SecurityQuestions[0].Id;
                ViewBag.QuestionText1 = status.SecurityQuestions[0].Question;
                ViewBag.Question2 = status.SecurityQuestions[1].Id;
                ViewBag.QuestionText2 = status.SecurityQuestions[1].Question;
                ViewBag.Question3 = status.SecurityQuestions[2].Id;
                ViewBag.QuestionText3 = status.SecurityQuestions[2].Question;
            }

            if (ViewBag.Status == 0)
            {
                TempData["success"] = "Your password is your transmission password. If you do not know your transmission password, or need further assistance, please contact Customer Support.";
            }

            if (ViewBag.Status == -2)
            {
                TempData["error"] = "The UserID specified is valid but we see that no Security Questions have been configured yet. Please contact the Administrator to have your password reset.";
            }


            return View();
        }

        // GET: ChangePassword
        public ActionResult UserId()
        {
            return View();
        }


        public ForgotAccountModel GetWebAPIForUserId(string userid)
        {
            ForgotAccountModel ForgotAccountMod = new ForgotAccountModel();
           // string Status = "";
            using (var client = new HttpClient())
            {
                // New code:
                string APIUrl = ConfigurationManager.AppSettings["EMPPortalWebAPI"].ToString();

                client.BaseAddress = new Uri(APIUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var gizmo = new { id = userid };

                var response = client.GetAsync("api/ForgotAccount/Password?Id=" + userid).Result;

                if (response.IsSuccessStatusCode)
                {
                    var json = new JavaScriptSerializer();
                    string message = response.Content.ReadAsStringAsync().Result;
                    ForgotAccountModel dict = json.Deserialize<ForgotAccountModel>(message);
                    if (dict != null)
                    {
                        ForgotAccountMod.Id = dict.Id;
                        ForgotAccountMod.Status = dict.Status;
                        ForgotAccountMod.SecurityQuestions = new List<SecurityQuestionModel>();

                        foreach (SecurityQuestionModel Dic in dict.SecurityQuestions)
                        {
                            ForgotAccountMod.SecurityQuestions.Add(Dic);
                        }
                    }
                }
                else
                {
                    ForgotAccountMod.Status = -1;
                }
            }

            return ForgotAccountMod;
        }
    }

    public class ForgotAccountModel
    {
        public System.Guid Id { get; set; }
        public int Status { get; set; }
        public string StatusCode { get; set; }
        public List<SecurityQuestionModel> SecurityQuestions { get; set; }
    }

    public class SecurityQuestionModel
    {
        public System.Guid Id { get; set; }
        public string Question { get; set; }
        public string Description { get; set; }
    }
}