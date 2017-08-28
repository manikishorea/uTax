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

using EMPAdmin.Transactions.Bank.DTO;
using EMPAdmin.Transactions.Bank;

using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class BankSubQuestionController : ApiController
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

        public BankSubQuestionService _BankSubQuestionService = new BankSubQuestionService();

        [ResponseType(typeof(BankSubQuestionDTO))]
        public IHttpActionResult GetAllBankSubQuestion()
        {
            var user = _BankSubQuestionService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        [ResponseType(typeof(BankSubQuestionDTO))]
        public async Task<IHttpActionResult> GetBankSubQuestion(Guid id)
        {
            var data = await _BankSubQuestionService.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [ResponseType(typeof(BankSubQuestionDTO))]
        public async Task<IHttpActionResult> PostBankSubQuestion(BankSubQuestionDTO data)
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
                EntityStateId = (int)EntityState.Added;
            }
            else
            {
                EntityStateId = (int)EntityState.Modified;
            }

            Guid result = await _BankSubQuestionService.Save(data, id, EntityStateId);
            data.Id = id;
            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return CreatedAtRoute("DefaultApi", new { id = id }, data);
        }

        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutUserMaster(Guid id, BankSubQuestionDTO _Dto)
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

            Guid result = await _BankSubQuestionService.SaveStatus(_Dto, id, EntityStateId);

            if (result == Guid.Empty)
            {
                return NotFound();
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(BankSubQuestionDTO))]
        public async Task<IHttpActionResult> DeleteBankSubQuestion(Guid id)
        {
            bool result = await _BankSubQuestionService.Delete(id);
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