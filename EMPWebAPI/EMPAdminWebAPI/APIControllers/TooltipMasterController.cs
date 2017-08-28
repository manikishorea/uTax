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

using EMPAdmin.Transactions.Tooltip.DTO;
using EMPAdmin.Transactions.Tooltip;
using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class TooltipMasterController : ApiController
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

        public TooltipService _TooltipService = new TooltipService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(TooltipDTO))]
        public IHttpActionResult GetTooltipMaster()
        {
            var user = _TooltipService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        
        [ResponseType(typeof(TooltipDTO))]
        public async Task<IHttpActionResult> GetTooltipMaster(Guid id)
        {
            var data = await _TooltipService.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(TooltipDTO))]
        public async Task<IHttpActionResult> PostTooltipMaster(TooltipDTO data)
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

            Guid result = await _TooltipService.Save(data, id, EntityStateId);
            data.Id = id;

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return CreatedAtRoute("DefaultApi", new { id = id }, data);
        }

        // PUT: api/Tooltip/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, TooltipDTO user)
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

            Guid result = await _TooltipService.SaveStatus(user, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        // DELETE: api/UserMasters/5
        [ResponseType(typeof(TooltipDTO))]
        public async Task<IHttpActionResult> DeleteTooltipMaster(Guid id)
        {
            bool result = await _TooltipService.Delete(id);
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