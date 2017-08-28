using System;
using System.Configuration;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;

using System.Data.Entity;

using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using EMP.Core.Utilities;

using EMP.Core.Token.DTO;
using EMPEntityFramework.Edmx;
using System.Net;
using System.Net.Http;
using System.Web;

namespace EMP.Core.Token
{
    public class TokenService : ITokenService, IDisposable
    {
        public DatabaseEntities db = new DatabaseEntities();
        public TokenDTO user = new TokenDTO();

        //public UserService(uTaxDBEntities _db, UserDetailDTO _user)
        //{
        //    db = _db;
        //    user = _user;
        //}

        public TokenDTO GenerateToken(Guid userId, string userip = "")
        {
            db = new DatabaseEntities();
            var tokenModel = new TokenDTO();

            if (userId != Guid.Empty)
            {
                string token = Guid.NewGuid().ToString();
                DateTime issuedOn = DateTime.Now;
                DateTime expiredOn = DateTime.Now.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["TokenExpiredOn"].ToString()));
              //  string hostName = Dns.GetHostName();
                string myIP = userip;// Dns.GetHostByName(hostName).AddressList[0].ToString();
             
                var tokendomain = new TokenMaster
                {
                    UserId = userId,
                    AuthToken = token,
                    IssuedOn = issuedOn,
                    ExpiredOn = expiredOn,
                    IPAddress = myIP,
                    StatusCode = EMPConstants.Active
                };

                db.TokenMasters.Add(tokendomain);
                db.SaveChanges();
                db.Dispose();

                tokenModel = new TokenDTO()
                {
                    UserId = userId,
                    IssuedOn = issuedOn,
                    ExpiredOn = expiredOn,
                    AuthToken = token,
                    StatusCode = EMPConstants.Active
                };
            }

            return tokenModel;
        }

        /// <summary>
        /// Method to validate token against expiry and existence in database.
        /// </summary>
        /// <param name="tokenId"></param>
        /// <returns></returns>
        public bool ValidateToken(string tokenId)
        {
            db = new DatabaseEntities();
            var token = db.TokenMasters.Where(t => t.AuthToken == tokenId && t.ExpiredOn > DateTime.Now).FirstOrDefault();
            if (token != null && !(DateTime.Now > token.ExpiredOn))
            {
                token.ExpiredOn = token.ExpiredOn.AddSeconds(Convert.ToDouble(ConfigurationManager.AppSettings["TokenExpiredOn"].ToString()));
                db.Entry(token).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                db.Dispose();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Method to kill the provided token id.
        /// </summary>
        /// <param name="tokenId">true for successful delete</param>
        public bool Kill(string TokenId)
        {
            // GroupRoleMap _GroupRoleMap = new GroupRoleMap();
            try
            {
                db = new DatabaseEntities();
                var data = db.TokenMasters.Where(o => o.AuthToken == TokenId).FirstOrDefault();
                if (data != null)
                {
                    //db.TokenMasters.Remove(data);
                    data.ExpiredOn = data.ExpiredOn.AddMinutes(-Convert.ToDouble(ConfigurationManager.AppSettings["TokenExpiredOn"].ToString()));
                    data.StatusCode = "INA";
                    db.Entry(data).State = EntityState.Modified;
                    db.SaveChanges();
                    db.Dispose();
                }
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete tokens for the specific deleted user
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>true for successful delete</returns>
        public bool DeleteByUserId(Guid userId)
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.TokenMasters.Where(o => o.UserId == userId);
                if (data != null)
                {
                    db.TokenMasters.RemoveRange(data);
                    db.SaveChanges();
                    db.Dispose();
                }

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public void Dispose()
        {
            db.Dispose();
        }

        //using System.Net.Http;
    }
}

public static class HttpRequestMessageExtensions
    {
        private const string HttpContext = "MS_HttpContext";
        private const string RemoteEndpointMessage =
            "System.ServiceModel.Channels.RemoteEndpointMessageProperty";
        private const string OwinContext = "MS_OwinContext";

        public static string GetClientIpAddress(this HttpRequestMessage request)
        {
            // Web-hosting. Needs reference to System.Web.dll
            if (request.Properties.ContainsKey(HttpContext))
            {
                dynamic ctx = request.Properties[HttpContext];
                if (ctx != null)
                {
                    return ctx.Request.UserHostAddress;
                }
            }

            // Self-hosting. Needs reference to System.ServiceModel.dll. 
            if (request.Properties.ContainsKey(RemoteEndpointMessage))
            {
                dynamic remoteEndpoint = request.Properties[RemoteEndpointMessage];
                if (remoteEndpoint != null)
                {
                    return remoteEndpoint.Address;
                }
            }

            // Self-hosting using Owin. Needs reference to Microsoft.Owin.dll. 
            if (request.Properties.ContainsKey(OwinContext))
            {
                dynamic owinContext = request.Properties[OwinContext];
                if (owinContext != null)
                {
                    return owinContext.Request.RemoteIpAddress;
                }
            }

            return null;
        }
    }

