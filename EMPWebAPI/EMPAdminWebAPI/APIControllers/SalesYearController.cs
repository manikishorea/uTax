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
using EMPAdmin.Transactions.SalesYear;
using EMPAdmin.Transactions.SalesYear.DTO;

using EMPAdminWebAPI.Filters;
using EMPAdmin.Transactions.Entity.DTO;
using EMPAdmin.Transactions.Entity;
using System.Web.Http.Results;

namespace EMPAdminWebAPI.APIControllers
{
    [TokenAuthorization]
    public class SalesYearController : ApiController
    {
        SalesYearService _SalesYearService = new SalesYearService();

        // GET: api/SalesYear
        [ResponseType(typeof(SalesYearDTO))]
        public IHttpActionResult GetAllSalesYearMasters()
        {
            var Sales = _SalesYearService.GetAll();
            if (Sales == null)
            {
                return NotFound();
            }

            return Ok(Sales);
        }


        //GET: api/SalesYear 
        [ResponseType(typeof(SalesYearDTO))]
        public async Task<IHttpActionResult> GetSalesYear(Guid id)
        {
            var fees = await _SalesYearService.GetById(id);
            if (fees == null)
            {
                return NotFound();
            }

            return Ok(fees);
        }

        // POST: api/FeesMasters
        [ResponseType(typeof(SalesYearDTO))]
        public IHttpActionResult PostSalesYear(SalesYearDTO _Dto)
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

            Guid result =  _SalesYearService.Save(_Dto, id, EntityStateId);
            _Dto.Id = id;

            if (result == Guid.Empty)
            {
                return StatusCode(HttpStatusCode.Conflict);
            }

            return Ok(_Dto.Id);
        }


        // PUT: api/FeeMasters/id
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, SalesYearDTO _Dto)
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

            Guid result = await _SalesYearService.SaveStatus(_Dto, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }        
    }
}
