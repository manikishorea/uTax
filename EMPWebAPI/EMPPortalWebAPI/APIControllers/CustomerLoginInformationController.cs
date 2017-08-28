using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EMPPortal.Transactions.CustomerLoginInformation;
using EMPPortalWebAPI.Filters;

namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class CustomerLoginInformationController : ApiController
    {
        private CustomerLoginInformationService customerLoginInformationService = new CustomerLoginInformationService();

        // GET: api/emp_CustomerInformation
        public List<CustomerLoginInformationModel> GetAllCustomerLoginInformation()
        {
           return  customerLoginInformationService.GetAllCustomerLoginInformation();
        }

        // GET: api/emp_CustomerInformation/5
        //[ResponseType(typeof(CustomerLoginInformationModel))]
        public IHttpActionResult GetCustomerInformation(Guid id)
        {
            var customerInformation = customerLoginInformationService.GetCustomerLoginInformation(id);
            if (customerInformation == null)
            {
                return NotFound();
            }

            return Ok(customerInformation);
        }

        // POST: api/emp_CustomerInformation
        [ResponseType(typeof(CustomerLoginInformationModel))]
        public IHttpActionResult Postemp_CustomerLoginInformation(CustomerLoginInformationModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid newguid;
            if (!string.IsNullOrEmpty(dto.Id))
            {
                if (!Guid.TryParse(dto.Id, out newguid))
                {
                    return BadRequest(ModelState);
                }
            }

            int result = customerLoginInformationService.Save(dto);

            if (result == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }


        public bool CustomerLoginInformationExists(Guid id)
        {
            return customerLoginInformationService.CustomerLoginInformationExists(id);
        }
    }
}
