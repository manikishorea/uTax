//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net;
//using System.Net.Http;
//using System.Web;
//using System.Web.Http.Controllers;
//using System.Web.Http.Filters;
//using System.Web.Mvc;
//using EMP.Core.Token;
//using System.Web.Http.Cors;
//using System.Web.Cors;
//using System.Threading.Tasks;
//using System.Threading;

//namespace System.Web.Http.Cors
//{

//    /// <summary>

//    /// Provides an abstraction for getting the <see cref="CorsPolicy"/>.

//    /// </summary>

//    public interface ICorsPolicyProvider

//    {

//        /// <summary>

//        /// Gets the <see cref="CorsPolicy"/>.

//        /// </summary>

//        /// <param name="request">The request.</param>

//        /// <param name="cancellationToken">The cancellation token.</param>

//        /// <returns>The <see cref="CorsPolicy"/>.</returns>

//        Task<CorsPolicy> GetCorsPolicyAsync(HttpRequestMessage request, CancellationToken cancellationToken);

//    }


//}


//public class AllowCorsAttribute : Attribute, ICorsPolicyProvider
//{

//    private string _configName;
//    public AllowCorsAttribute(string name = null)
//    {
//        _configName = name ?? "Default";
//    }

//    public string ConfigName
//    {
//        get { return _configName; }
//    }

//    public Task<CorsPolicy> GetCorsPolicyAsync(
//        HttpRequestMessage request,
//        CancellationToken cancellationToken)
//    {
//        // using (var db = new CorsContext())
//        {
//            //   var origins = db.AllowOrigins.Where(o => o.Name == ConfigName).ToArray();

//            var retval = new CorsPolicy();

//            retval.AllowAnyHeader = true;

//            retval.AllowAnyMethod = true;

//            retval.AllowAnyOrigin = false;

//            //  foreach (var each in origins)

//            {
//                //retval.Origins.Add(each.Origin);
//                retval.Origins.Add("http://localhost:9003");
//            }

//            return Task.FromResult(retval);
//        }

//    }

//}

