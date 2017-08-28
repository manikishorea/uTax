using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace EMPAdmin.Filters
{
    // [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    //public class SessionCheckAttribute : ActionFilterAttribute
    public class SessionCheckAttribute : System.Web.Mvc.ActionFilterAttribute
    {
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext1)
        {

            string controllerName = filterContext1.ActionDescriptor.ControllerDescriptor.ControllerName.ToLower();
            HttpSessionStateBase session = filterContext1.HttpContext.Session;
            string cookieHeader = filterContext1.HttpContext.Request.Headers["Cookie"];

            if (session.Count <= 0 || cookieHeader == null) //|| cookieHeader.IndexOf("ASP.NET_SessionId") >= 0
            {
                var url = new UrlHelper(filterContext1.RequestContext);
                var loginUrl = url.Content("~/Account/Login");
                filterContext1.HttpContext.Response.Redirect(loginUrl, true);
            }

            base.OnActionExecuting(filterContext1);
        }
    }
}