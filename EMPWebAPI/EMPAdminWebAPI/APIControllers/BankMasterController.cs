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
using EMPAdmin.Transactions.Bank;
using EMPAdmin.Transactions.Bank.DTO;

using EMPAdminWebAPI.Filters;
namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class BankMasterController : ApiController
    {
        BankMasterService _BankMasterService = new BankMasterService();

        // GET: api/BankMaster
        [ResponseType(typeof(BankMasterDTO))]
        public IHttpActionResult GetBankMasters()
        {
            var banks = _BankMasterService.GetAllBankMaster();
            if (banks == null)
            {
                return NotFound();
            }

            return Ok(banks);
        }

        [ResponseType(typeof(BankMasterDTO))]
        public async Task<IHttpActionResult> GetBankMasters(Guid id)
        {
            var banks = await _BankMasterService.GetBankDetailsById(id);
            if (banks == null)
            {
                return NotFound();
            }

            return Ok(banks);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(BankMasterDTO))]
        public async Task<IHttpActionResult> PostUserMaster(BankMasterDTO _Dto)
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

            int result = await _BankMasterService.Save(_Dto, id, EntityStateId);
            _Dto.Id = id;
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
        public async Task<IHttpActionResult> PutUserMaster(Guid id, BankMasterDTO _Dto)
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

            Guid result = await _BankMasterService.SaveStatus(_Dto, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}