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
    public class ForgotAccountController : ApiController
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

        private ForgotAccountService _ForgotAccountService = new ForgotAccountService();

        [HttpGet]
        [ActionName("password")]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetByUserId(string id)
        {
            var data = _ForgotAccountService.GetByUserId(id);
            if (data.Status == -1)
            {
                return NotFound();
            }

            return Ok(data);

        }

        // POST: api/UserMasters
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostSecurityQuestion(userSecurityAnswerModel data)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = _ForgotAccountService.GetByUserSecurityAnswer(data);

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