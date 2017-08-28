using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Routing;
using System.Web.Http.ExceptionHandling;

namespace EMPPortalWebAPI
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            // config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));
            // Web API routes
            //  var corsAttribute = new EnableCorsAttribute("*", "*", "*");

            //config.MapHttpAttributeRoutes();

            // config.Routes.MapHttpRoute(
            //   name: "ActionApi",
            //   routeTemplate: "api/{controller}/{action}/{id}",
            //   defaults: new { id = RouteParameter.Optional }
            //);

            config.Routes.MapHttpRoute(
            name: "ActionApi",
            routeTemplate: "api/{controller}/{action}/{id}",
            defaults: new { id = RouteParameter.Optional }
        //  constraints: new { id = @"^(?=.*[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}])(?=.*\d).+$" }//@"^[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}]?$" }
        //@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$"
        );

            config.Routes.MapHttpRoute(
                      name: "DefaultApi",
                      routeTemplate: "api/{controller}/{id}",
                      defaults: new { id = RouteParameter.Optional }
                  //  constraints: new { id = @"^(?=.*[{(]?[0-9A-F]{8}[-]?([0-9A-F]{4}[-]?){3}[0-9A-F]{12}[)}])(?=.*\d).+$" }
                  );

            // config.Routes.MapHttpRoute(
            //           name: "DefaultApi",
            //           routeTemplate: "api/{controller}/{id}",
            //           defaults: new { id = RouteParameter.Optional }
            //       );



           // config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());


            // config.Routes.MapHttpRoute(
            //     name: "DefaultApi",
            //     routeTemplate: "api/{controller}/{id}",
            //     defaults: new { id = RouteParameter.Optional }
            // );

            // config.Routes.MapHttpRoute(
            //    name: "DefaultApi1",
            //    routeTemplate: "api/{action}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            //config.Routes.MapHttpRoute("DefaultApiWithId", "api/{controller}/{id}", new { id = RouteParameter.Optional }, new { id = @"\d+" });
            //config.Routes.MapHttpRoute("DefaultApiWithAction", "api/{controller}/{action}");
            //config.Routes.MapHttpRoute("DefaultApiGet", "api/{controller}", new { action = "Get" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Get) });
            //config.Routes.MapHttpRoute("DefaultApiPost", "api/{controller}", new { action = "Post" }, new { httpMethod = new HttpMethodConstraint(HttpMethod.Post) });
            config.EnableCors();
        }
    }
}
