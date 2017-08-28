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

using EMPAdmin.Transactions.ContactPersonTitle.DTO;
using EMPAdmin.Transactions.ContactPersonTitle;

using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
   // 
    [TokenAuthorization]
    public class ContactPersonTitleMasterController : ApiController
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

        public ContactPersonTitleService _ContactPersonTitleService = new ContactPersonTitleService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(IQueryable<ContactPersonTitleDTO>))]
        public IHttpActionResult GetContactPersonTitleMaster()
        {
            var user = _ContactPersonTitleService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        
        [ResponseType(typeof(ContactPersonTitleDTO))]
        public async Task<IHttpActionResult> GetContactPersonTitleMaster(Guid id)
        {
            var data = await _ContactPersonTitleService.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(ContactPersonTitleDTO))]
        public async Task<IHttpActionResult> PostContactPersonTitleMaster(ContactPersonTitleDTO data)
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

            int result = await _ContactPersonTitleService.Save(data, id, EntityStateId);
            data.Id = id;
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

        // PUT: api/ContactPersonTitleMaster/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, ContactPersonTitleDTO user)
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

            Guid result = await _ContactPersonTitleService.SaveStatus(user, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/UserMasters/5
        [ResponseType(typeof(ContactPersonTitleDTO))]
        public async Task<IHttpActionResult> DeleteContactPersonTitleMaster(Guid id)
        {
            bool result = await _ContactPersonTitleService.Delete(id);
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