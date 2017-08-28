using EMPPortal.Transactions.CustomerInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace EMPPortalWebAPI.APIControllers
{
    public class CrosslinkController : ApiController
    {
        CustomerInformationService customerInformationService = new CustomerInformationService();

        [HttpPost]
        [ActionName("CreateNewUser")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult CreateNewUser(CreateNewUserModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = customerInformationService.CreateNewUser(model);
            return Ok(result.Status);
        }

        [HttpPost]
        [ActionName("SendEmailForNewUser")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SendEmailForNewUser(NewUserEmailRequest model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var result = customerInformationService.SendEmailForNewUser(model);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("checkActivationandCreateUser")]
        [ResponseType(typeof(int))]
        public IHttpActionResult checkActivationandCreateUser(Guid CustomerId,Guid UserId)
        {
            var result = customerInformationService.checkActivationandCreateUser(CustomerId, UserId);
            return Ok(result);
        }
    }
}
