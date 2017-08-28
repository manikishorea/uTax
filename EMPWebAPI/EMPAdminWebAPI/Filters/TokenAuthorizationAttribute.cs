using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Mvc;
using EMP.Core.Token;
using System.Web.Http.Cors;
using System.Web.Cors;
using System.Threading.Tasks;
using System.Threading;

namespace EMPAdminWebAPI.Filters
{
    public class TokenAuthorizationAttribute : System.Web.Http.Filters.ActionFilterAttribute
    {
        private const string Token = "Token";

        public override void OnActionExecuting(HttpActionContext filterContext)
        {
            //  Get API key provider
            //var provider = filterContext.ControllerContext.Configuration
            //.DependencyResolver.GetService(typeof(ITokenService)) as ITokenService;
            //var header = filterContext.Request.Headers.SingleOrDefault(x => x.Key == "Token");

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



    //    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    //    public class MyCorsPolicyAttribute : Attribute, ICorsPolicyProvider
    //    {
    //        private CorsPolicy _policy;

    //        public MyCorsPolicyAttribute()
    //        {
    //            // Create a CORS policy.
    //            _policy = new CorsPolicy
    //            {
    //                AllowAnyMethod = true,
    //                AllowAnyHeader = true,
    //                AllowAnyOrigin = true
    //            };

    //            _policy.Origins.Add("http://localhost:9003");
    //            // Add allowed origins.
    //            //  _policy.Origins.Add("http://myclient.azurewebsites.net");
    //            //  _policy.Origins.Add("http://www.contoso.com");

    //            return Task.FromResult(_policy);
    //        }

    //        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request)
    //        {
    //            return Task.FromResult(_policy);
    //        }
    //    }

    //    public interface ICorsPolicyProvider
    //    {
    //        Task<CorsPolicy> GetCorsPolicyAsync(
    //    HttpRequestMessage request
    //  //  CancellationToken cancellationToken
    //);
    //    }

    //    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    //    public class MyCorsPolicyAttribute : Attribute, ICorsPolicyProvider
    //    {
    //        private CorsPolicy _policy;

    //        public MyCorsPolicyAttribute()
    //        {
    //            // Create a CORS policy.
    //            _policy = new CorsPolicy
    //            {
    //                AllowAnyMethod = true,
    //                AllowAnyHeader = true
    //            };

    //            // Add allowed origins.
    //            //   _policy.Origins.Add("http://myclient.azurewebsites.net");
    //            //   _policy.Origins.Add("http://www.contoso.com");
    //            _policy.AllowAnyHeader = true;
    //            _policy.AllowAnyOrigin = true;
    //            _policy.AllowAnyMethod = true;
    //        }

    //        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request)
    //        {
    //            return Task.FromResult(_policy);
    //        }

    //        public Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    //        {
    //            throw new NotImplementedException();
    //        }
    //    }


    //    public interface ICorsPolicyProviderFactory
    //    {
    //        Task<CorsPolicy> GetCorsPolicyProvider(
    //    HttpRequestMessage request
    //);
    //    }

    //    public class CorsPolicyFactory : ICorsPolicyProviderFactory
    //    {
    //        ICorsPolicyProvider _provider = new MyCorsPolicyProvider();

    //        public ICorsPolicyProvider GetCorsPolicyProvider(HttpRequestMessage request)
    //        {
    //            return _provider;
    //        }
    //    }



    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
   
}
