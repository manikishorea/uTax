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

using EMPAdmin.Transactions.APIIntegrationMaster.DTO;
using EMPAdmin.Transactions.APIIntegrationMaster;
using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class APIIntegrationMasterController : ApiController
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

        public APIIntegrationService _APIIntegrationService = new APIIntegrationService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(APIIntegrationDTO))]
        public IHttpActionResult GetAPIIntegrationeMaster()
        {
            var user = _APIIntegrationService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        
        [ResponseType(typeof(APIIntegrationDTO))]
        public async Task<IHttpActionResult> GetAPIIntegrationMaster(Guid id)
        {
            var data = await _APIIntegrationService.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(APIIntegrationDTO))]
        public async Task<IHttpActionResult> PostAPIIntegrationMaster(APIIntegrationDTO data)
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

            Guid result = await _APIIntegrationService.Save(data, id, EntityStateId);
            data.Id = id;
            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return CreatedAtRoute("DefaultApi", new { id = id }, data);
        }

        // DELETE: api/UserMasters/5
        [ResponseType(typeof(APIIntegrationDTO))]
        public async Task<IHttpActionResult> DeleteAPIIntegratioMaster(Guid id)
        {
            bool result = await _APIIntegrationService.Delete(id);
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