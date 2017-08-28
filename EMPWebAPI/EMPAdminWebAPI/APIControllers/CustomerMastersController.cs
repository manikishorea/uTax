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
using EMPAdmin.Transactions.Customer.DTO;
using EMPAdmin.Transactions.Customer;

using EMPAdminWebAPI.Filters;
using System.Web.Http.Cors;

namespace EMPAdminWebAPI.APIControllers
{
    
    [TokenAuthorization]
    public class CustomerMastersController : ApiController
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

        public CustomerService _customerService = new CustomerService();

        [ResponseType(typeof(CustomerDTO))]
        public IHttpActionResult GetCustomerMasters()
        {
            var customer = _customerService.GetAllCustomer();
            if (customer == null)
            {
                return NotFound();
            }
            return Ok(customer);
        }

        [ResponseType(typeof(CustomerDTO))]
        public async Task<IHttpActionResult> GetCustomerMaster(Guid id)
        {
            var customer = await _customerService.GetCustomer(id);

            if (customer.Id == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }
    }
}