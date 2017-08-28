using System;
using System.Data.Entity;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

using EMPPortal.Transactions.Account.Model;
using EMPPortal.Transactions.Account;
using EMPPortalWebAPI.Filters;
using EMPPortal.Transactions.OfficeManagementTransactions;

namespace EMPPortalWebAPI.APIControllers
{
    // [TokenAuthorization]
    public class ChangePasswordController : ApiController
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

        public ChangePasswordService _ChangePasswordService = new ChangePasswordService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(ChangePasswordModel))]
        public IHttpActionResult GetAllChangePassword()
        {
            var user = _ChangePasswordService.GetAll();
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
            // return user;
        }

        // GET: api/UserMasters/5

        [ResponseType(typeof(ChangePasswordModel))]
        public async Task<IHttpActionResult> GetChangePassword(Guid id)
        {
            var data = await _ChangePasswordService.GetById(id);

            if (data.Id == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPost]
        [ActionName("initial")]
        // POST: api/UserMasters
        [ResponseType(typeof(ChangePasswordModel))]
        public IHttpActionResult PostChangePassword(ChangePasswordModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = _ChangePasswordService.Save(dto.EMPPassword, dto.Id);

            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpPost]
        [ActionName("update")]
        // POST: api/UserMasters
        [ResponseType(typeof(ChangePasswordModel))]
        public IHttpActionResult PostChangePasswordUpdate(ChangePasswordModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool result = _ChangePasswordService.SaveMyPassword(dto.CurrentPassword, dto.EMPPassword, dto.Id);
            if (result)
            {
                OfficeManagementService update = new OfficeManagementService();
                update.UpdateOfficeManagement(dto.Id, "");
            }

            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ActionName("reset")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostResetChangePassword(ChangePasswordModel dto)
        {
            Guid uid = dto.UserId ?? Guid.Empty;

            bool result = _ChangePasswordService.ResetPassword(dto.EMPPassword, dto.EMPUserId);
            if (result)
            {
                Guid custid = _ChangePasswordService.GetCustomerIdByUserId(dto.EMPUserId);
                if (custid != Guid.Empty)
                {
                    OfficeManagementService update = new OfficeManagementService();
                    update.UpdateOfficeManagement(custid, "");
                }
            }
            if (!result)
            {
                return NotFound();
            }

            return Ok(result);
        }


        [HttpPost]
        [ActionName("ResetPassword")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult ResetPasswordFromAdmin(ChangePasswordModel dto)
        {
            Guid UserId, CustomerOfficeId;

            if (!Guid.TryParse(dto.UserId.ToString(), out UserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(dto.CustomerOfficeId.ToString(), out CustomerOfficeId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            bool result = _ChangePasswordService.ResetPasswordFromAdmin(dto.UserId ?? Guid.Empty, dto.CustomerOfficeId ?? Guid.Empty);

            if (!result)
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