using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.Configuration.DTO;
using EMPPortal.Transactions.Configuration;
using System.Threading.Tasks;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;
namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class ConfigurationController : ApiController
    {
        CustomerConfigStatusService _CustomerConfigStatusService = new CustomerConfigStatusService();

        /// <summary>
        /// This method is used to Get the Sub Site Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IQueryable<CustomerConfigStatusDTO>))]
        public IHttpActionResult Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = _CustomerConfigStatusService.GetById(id);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Sub Site Configuration Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
      //  [HttpPost]
        [ActionName("ConfigurationStatus")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostBankService(CustomerConfigStatusDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _CustomerConfigStatusService.CustomerConfigStatusSave(oDto);
            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ActionName("Save")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostConfigurationSatus(string CustomerId, string UserId, string SiteMapID, string resettype, string ActiveLinkSiteMapID, string BankId, string status = "done")
        {

            Guid GUserId, GCustomerId, GSitemapID,GBankId;


            if (!Guid.TryParse(UserId, out GUserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }


            if (!Guid.TryParse(CustomerId, out GCustomerId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(SiteMapID, out GSitemapID))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(BankId, out GBankId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }


            var result = _CustomerConfigStatusService.SaveConfigurationSatus(GCustomerId, GUserId, GSitemapID, resettype, ActiveLinkSiteMapID, GBankId, status);
            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ActionName("ResetActivation")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostResetActivation(string CustomerId, string UserId, string ActiveLinkSiteMapID)
        {
            Guid GUserId, GCustomerId;

            Guid GActiveLinkSitemapID;

            if (!Guid.TryParse(UserId, out GUserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(CustomerId, out GCustomerId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }


            if (!Guid.TryParse(ActiveLinkSiteMapID, out GActiveLinkSitemapID))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = _CustomerConfigStatusService.ResetConfigurationSatus(GCustomerId, GUserId, GActiveLinkSitemapID);

            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost]
        [ActionName("ActiveAccount")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostActivateCustomer(string CustomerId, string UserId)
        {
            Guid GUserId, GCustomerId;

            if (!Guid.TryParse(UserId, out GUserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(CustomerId, out GCustomerId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = _CustomerConfigStatusService.ActivateCustomer(GCustomerId, GUserId);

            if (result == false)
            {
                return NotFound();
            }

            return Ok(result);
        }


        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateEFINAfterApproved")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UpdateEFINAfterApproved(string CustomerId, string UserId,int OldEFIN)
        {
            Guid GUserId, GCustomerId;

            if (!Guid.TryParse(UserId, out GUserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(CustomerId, out GCustomerId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = _CustomerConfigStatusService.UpdateEFINAfterApproved(GCustomerId, GUserId, OldEFIN);
            return Ok(result);
        }

        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        [ActionName("checkApprovedBankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult GetBankEnrollmentStatus(string CustomerId)
        {
            Guid  GCustomerId;

            if (!Guid.TryParse(CustomerId, out GCustomerId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = _CustomerConfigStatusService.GetBankEnrollmentStatusByUser(GCustomerId);
            return Ok(result);
        }

        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        /// 
        [HttpPost]
        [ActionName("GetSOSOMEActivation")]
        [ResponseType(typeof(int))]
        public IHttpActionResult SetSOSOMEActivation(string CustomerId, string UserId)
        {
            Guid GUserId, GCustomerId;

            if (!Guid.TryParse(UserId, out GUserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(CustomerId, out GCustomerId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = _CustomerConfigStatusService.SetSOSOMEActivation(GCustomerId, GUserId);
            return Ok(result);
        }


        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateFeeAfterApproved")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult checkUpdateFeeAfterApproved(string CustomerId, string UserId)
        {
            Guid GUserId, GCustomerId;

            if (!Guid.TryParse(UserId, out GUserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            if (!Guid.TryParse(CustomerId, out GCustomerId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = _CustomerConfigStatusService.UpdateFeeAfterApproved(GCustomerId, GUserId);
            return Ok(result);
        }
    }
}
