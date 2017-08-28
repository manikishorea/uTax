using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using EMP.Core.Token.DTO;
using EMP.Core.Token;

namespace EMPPortalWebAPI.Filters
{
    public class TokenAuthorizationAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string Token = "Token";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //  Get API key provider
            //var provider = filterContext.ControllerContext.Configuration
            //.DependencyResolver.GetService(typeof(ITokenService)) as ITokenService;
          //  var header = filterContext.Request.Headers.SingleOrDefault(x => x.Key == "Token");

            //dynamic result = new JsonObject();

            //var valid = header.Value != null || ApiKeyManager.ResetTimer(header.Value.First());

            //if (!valid)
            //{
            //    result.Message = "Invalid Authorization Key";
            //    result.Location = "/api/Authentication";
            //    context.Response = new HttpResponseMessage<JsonValue>(result, HttpStatusCode.Forbidden);
            //}

            TokenService TokenService = new TokenService();

            if (filterContext.Request.Headers.Contains(Token))
            {
                //HttpContext.Request.ServerVariables.Get("HTTP_X_MY_CUSTOM_HEADER_NAME");
                var tokenValue = filterContext.Request.Headers.GetValues(Token).First();

                // Validate Token
                if (TokenService != null && !TokenService.ValidateToken(tokenValue))
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized) { ReasonPhrase = "Token Expired" };
                    filterContext.Response = responseMessage;
                }
            }
            else
            {
                filterContext.Response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }

            base.OnActionExecuting(filterContext);
        }
    }
    //public static string GetClientIpAddress(this HttpRequestMessage request)
    //{
    //    if (request.Properties.ContainsKey("MS_HttpContext"))
    //    {
    //        return IPAddress.Parse(((HttpContextBase)request.Properties["MS_HttpContext"]).Request.UserHostAddress).ToString();
    //    }
    //    return null;
    //}
}