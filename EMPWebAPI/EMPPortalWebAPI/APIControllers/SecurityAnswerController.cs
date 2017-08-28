using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using EMPPortal.Transactions.Account.Model;
using EMPPortal.Transactions.Account;
using System.Linq;

namespace EMPPortalWebAPI.APIControllers
{
    // [TokenAuthorization]
    public class SecurityAnswerController : ApiController
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

        public SecurityAnswerService _SecurityAnswerService = new SecurityAnswerService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(SecurityQuestionAnswerModel))]
        public IHttpActionResult GetAllSecurityAnswer()
        {
            var user = _SecurityAnswerService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5
        [HttpGet]
        [ResponseType(typeof(SecurityQuestionAnswerModel))]
        public IHttpActionResult GetSecurityAnswers(Guid id)
        {
            var data = _SecurityAnswerService.GetByUser(id);

            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        // POST: api/UserMasters
        [ResponseType(typeof(SecurityQuestionAnswerModel))]
        public IHttpActionResult PostSecurityQuestion(userSecurityAnswerModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            bool result = _SecurityAnswerService.Save(data);

            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // PUT: api/ContactPersonTitleMaster/id
        //[ResponseType(typeof(void))]
        //public async Task<IHttpActionResult> PutUserMaster(Guid id, SecurityQuestionAnswerModel user)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

        //    if (id != user.Id)
        //    {
        //        return BadRequest();
        //    }

        //    int EntityStateId = (int)EntityState.Modified;

        //    Guid result = await _SecurityAnswerService.SaveStatus(user, id, EntityStateId);

        //    if (result == Guid.Empty)
        //    {
        //        return NotFound();
        //    }

        //    return StatusCode(HttpStatusCode.NoContent);
        //}

        // DELETE: api/UserMasters/5
        //[ResponseType(typeof(ContactPersonTitleDTO))]
        //public async Task<IHttpActionResult> DeleteContactPersonTitleMaster(Guid id)
        //{
        //    bool result = await _SecurityAnswerService.Delete(id);
        //    if (result == false)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(result);
        //}

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