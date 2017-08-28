using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using EMPPortal.Transactions.Account.Model;
using EMPPortal.Transactions.Account;
namespace EMPPortalWebAPI.APIControllers
{
    // [TokenAuthorization]
    public class ResetPasswordController : ApiController
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

        public ResetPasswordService _ResetPassword = new ResetPasswordService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(ResetPasswordModel))]
        public IHttpActionResult GetAllResetPassword()
        {
            var user = _ResetPassword.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        
        [ResponseType(typeof(ResetPasswordModel))]
        public async Task<IHttpActionResult> GetResetPassword(Guid id)
        {
            var data = await _ResetPassword.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(ResetPasswordModel))]
        public IHttpActionResult PostResetPassword(ResetPasswordModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            Guid id = data.Id;
            int EntityStateId = 0;
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
                //user.Id = id;
                EntityStateId = (int)EntityState.Added;
            }
            else
            {
                EntityStateId = (int)EntityState.Modified;
            }

            Guid result = _ResetPassword.Save(data, id, EntityStateId);
            data.Id = id;

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return CreatedAtRoute("DefaultApi", new { id = id }, data);
        }

        // PUT: api/ResetPassword/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutResetPassword(Guid id, ResetPasswordModel pwd)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != pwd.Id)
            {
                return BadRequest();
            }

            int EntityStateId = (int)EntityState.Modified;

            Guid result = await _ResetPassword.SaveStatus(pwd, id, EntityStateId);

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
        
    }
}