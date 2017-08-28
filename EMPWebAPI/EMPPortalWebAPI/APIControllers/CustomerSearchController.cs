using System.Web.Http;
using EMPPortalWebAPI.Filters;
using EMPPortal.Transactions.Customer;
using System;

namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class CustomerSearchController : ApiController
    {
        private CustomerService _CustomerService = new CustomerService();

        // GET: api/emp_CustomerInformation
        public IHttpActionResult GetCustomerSearch(string pageno,string maxcount)
        {
            int pageno1 = Convert.ToInt32(pageno);
            int maxcount1 = Convert.ToInt32(maxcount);

            return Ok(_CustomerService.GetCustomerSearch(pageno1, maxcount1));
        }
    }
}