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
    public class CustomerLoginController : ApiController
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

        public CustomerLoginService _CustomerLoginService = new CustomerLoginService();

        //11212016-fn
        //[HttpPost]
        //[ResponseType(typeof(CustomerLoginModel))]
        //public IHttpActionResult Get(string userid, string password)
        //{
        //    CustomerLoginModel _CustomerLogin = new CustomerLoginModel();
        //    _CustomerLogin.CrossLinkUserId = userid;
        //    _CustomerLogin.EMPPassword = password;
        //    var dbresule = _CustomerLoginService.Get(_CustomerLogin,"");

        //    if (dbresule.Id == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(dbresule);
        //}

        [HttpPost]
        [ResponseType(typeof(CustomerLoginModel))]
        public IHttpActionResult Get(CustomerLoginModel data)
        {
            if (!ModelState.IsValid)
            {

            }
            var dbresule = _CustomerLoginService.IsUserExist(data, data.userip);

            if (dbresule == null)
            {
                return NotFound();
            }
            return Ok(dbresule);
        }

        [ActionName("GetCustomer")]
        [HttpGet]
        [ResponseType(typeof(CustomerLoginModel))]
        public IHttpActionResult GetCustomer(string Id)
        {
            var dbresule = _CustomerLoginService.getCustomerInfoById(Id);

            if (dbresule == null)
            {
                return NotFound();
            }
            return Ok(dbresule);
        }

        [ActionName("ParentCustomerInfo")]
        [HttpGet]
        [ResponseType(typeof(CustomerLoginModel))]
        public IHttpActionResult GetParentCustomerInfo(Guid Id)
        {
            var dbresule = _CustomerLoginService.GetParentCustomerInformation(Id);

            if (dbresule == null)
            {
                return NotFound();
            }
            return Ok(dbresule);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //  db.Dispose();
            }
            base.Dispose(disposing);
        }


        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult TokenKill(string TokenID)
        {
            EMP.Core.Token.TokenService otokeservice = new EMP.Core.Token.TokenService();
            var dbresule = otokeservice.Kill(TokenID);

            if (dbresule == false)
            {
                return NotFound();
            }
            return Ok(dbresule);
        }

    }
}