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

using EMPAdmin.Transactions.SecurityQuestion.DTO;
using EMPAdmin.Transactions.SecurityQuestion;

using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class SecurityQuestionMasterController : ApiController
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

        public SecurityQuestionService _SecurityQuestionService = new SecurityQuestionService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(SecurityQuestionDTO))]
        public IHttpActionResult GetSecurityQuestionMaster()
        {
            var user = _SecurityQuestionService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        
        [ResponseType(typeof(SecurityQuestionDTO))]
        public async Task<IHttpActionResult> GetSecurityQuestionMaster(Guid id)
        {
            var data = await _SecurityQuestionService.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(SecurityQuestionDTO))]
        public async Task<IHttpActionResult> PostSecurityQuestionMaster(SecurityQuestionDTO data)
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

            Guid result = await _SecurityQuestionService.Save(data, id, EntityStateId);
            data.Id = id;
            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return CreatedAtRoute("DefaultApi", new { id = id }, data);
        }

        // PUT: api/UserMasters/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, SecurityQuestionDTO user)
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

            Guid result = await _SecurityQuestionService.SaveStatus(user, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/UserMasters/5
        [ResponseType(typeof(SecurityQuestionDTO))]
        public async Task<IHttpActionResult> DeleteSecurityQuestionMaster(Guid id)
        {
            bool result = await _SecurityQuestionService.Delete(id);
            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
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