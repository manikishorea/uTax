using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Mvc;
using EMPAdmin.Transactions.Fees;
using EMPAdmin.Transactions.Fees.DTO;

using EMPAdminWebAPI.Filters;
namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class FeeMasterController : ApiController
    {
        FeesService _FeeMasterService = new FeesService();

        // GET: api/FeesMaster
        [ResponseType(typeof(FeesDTO))]
        public IHttpActionResult GetFeeMasters()
        {
            var Fees = _FeeMasterService.GetAll();
            if (Fees == null)
            {
                return NotFound();
            }

            return Ok(Fees);
        }

        [ResponseType(typeof(FeesDTO))]
        public async Task<IHttpActionResult> GetFeeMasters(Guid id)
        {
            var fees = await _FeeMasterService.GetById(id);
            if (fees == null)
            {
                return NotFound();
            }

            return Ok(fees);
        }

        // POST: api/FeesMasters
        [ResponseType(typeof(FeesDTO))]
        public async Task<IHttpActionResult> PostUserMaster(FeesDTO _Dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid id = _Dto.Id;
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

            int result = await _FeeMasterService.Save(_Dto, id, EntityStateId);
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

        // PUT: api/FeeMasters/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, FeesDTO _Dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != _Dto.Id)
            {
                return BadRequest();
            }

            int EntityStateId = (int)EntityState.Modified;

            Guid result = await _FeeMasterService.SaveStatus(_Dto, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}