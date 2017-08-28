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

using EMP.Core;
using EMPAdmin.Transactions.Account.DTO;
using EMPAdmin.Transactions.Account;

namespace EMPAdminWebAPI.APIControllers
{
    public class AccountController : ApiController
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

        public AccountService _AccountService = new AccountService();

        //[ResponseType(typeof(AccountDTO))]
        //public IHttpActionResult GetAccountALL()
        //{
        //    var data = _AccountService.IsUserExist("ghanshyamd", "1234");
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(data);
        //}

        //[HttpPost]
        //[ResponseType(typeof(AccountDTO))]
        //public async Task<IHttpActionResult> GetAccount123()
        //{
        //    var data = _AccountService.IsUserExist("ghanshyamd", "1234");
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }

        //    byte[] result = await Request.Content.ReadAsByteArrayAsync();

        //    //return result;

        //    return Ok(data);
        //}


        //[ResponseType(typeof(AccountDTO))]
        //public IHttpActionResult GetAccount12(string userName)
        //{
        //    var data = _AccountService.IsUserExist(userName, "1234");
        //    if (data == null)
        //    {
        //        return NotFound();
        //    }

        //    return Ok(data);
        //}

        [HttpPost]
        [ResponseType(typeof(AccountDTO))]
        public IHttpActionResult GetAccount(AccountDTO account)
        {
            var data = _AccountService.IsUserExist(account.UserName, account.Password);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
        }

        [HttpPost]
        [ResponseType(typeof(AccountDTO))]
        public IHttpActionResult GetAccount(string userName)
        {
            var data = _AccountService.IsUsernameExist(userName);
            if (data == null)
            {
                return NotFound();
            }

            return Ok(data);
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