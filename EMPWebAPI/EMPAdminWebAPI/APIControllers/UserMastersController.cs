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

using EMPAdmin.Transactions.User.DTO;
using EMPAdmin.Transactions.User;
using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
   // [AllowCors("UserMasters")]
   // 
    [TokenAuthorization]
    public class UserMastersController : ApiController
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

        public UserService _UserService = new UserService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(UserDTO))]
        public IHttpActionResult GetUserMasters()
        {
            var user = _UserService.GetAllUser();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        
        [ResponseType(typeof(UserDetailDTO))]
        public async Task<IHttpActionResult> GetUserMaster(Guid id)
        {
            var user = await _UserService.GetUser(id);

            if (user.Id == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(UserDetailDTO))]
        public async Task<IHttpActionResult> PostUserMaster(UserDetailDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //db.UserMasters.Add(userMaster);

            //try
            //{
            //    await db.SaveChangesAsync();
            //}
            //catch (DbUpdateException)
            //{
            //    if (UserMasterExists(userMaster.Id))
            //    {
            //        return Conflict();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            Guid id = user.Id;
            int EntityStateId = 0;
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                // user.Id = id;
                EntityStateId = (int)EntityState.Added;
            }
            else
            {
                EntityStateId = (int)EntityState.Modified;
            }

            int result = await _UserService.SaveUser(user, id, EntityStateId);
            user.Id = id;

            if (result == -1)
            {
                return StatusCode(HttpStatusCode.NotModified);
            }
            else if (result == 0)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }
            return Ok(id);
        }

        // PUT: api/UserMasters/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, UserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != user.Id)
            {
                return BadRequest();
            }

            int EntityStateId = (int)EntityState.Modified;

            Guid result = await _UserService.SaveStatus(user, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //  db.Dispose();
            }
            base.Dispose(disposing);
        }

        // GET: api/PermissionMasters/5
        [ResponseType(typeof(UserDetailDTO))]
        [HttpPost]
        public IHttpActionResult ResetPassword(Guid id, string password)
        {
            var user = _UserService.ResetPassword(password, id);
            if (!user)
            {
                return NotFound();
            }

            return Ok(user);
        }


    }
}