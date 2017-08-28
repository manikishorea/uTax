using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using EMP.Core;
using EMP.Core.Token.DTO;
using EMP.Core.Token;
namespace EMPAdminWebAPI.APIControllers
{
    public class AccessTokenController : ApiController
    {
        //private readonly IUserService _UserService;
        //private readonly uTaxDBEntities _db;
        //private readonly UserDetailDTO _user;

        //public UserMastersController(
        //   IUserService UserService, uTaxDBEntities db, UserDetailDTO user)
        //{
        //    _UserService = UserService;
        //    _db = db;
        //    _user = user;
        //}

        public TokenService _TokenService  = new TokenService();
        
        // GET: api/UserMasters
        // [Route("GetAllUser")]

        [HttpPost]
        [ResponseType(typeof(TokenDTO))]
        public IHttpActionResult GetAuthToken(Guid id)
        {
           
            var data = _TokenService.GenerateToken(id);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
            // return user;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
              //  db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}