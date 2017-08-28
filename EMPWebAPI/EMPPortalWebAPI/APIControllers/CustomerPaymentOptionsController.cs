using EMPPortal.Transactions.CustomerPayment;
using EMPPortal.Transactions.CustomerPayment.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

namespace EMPPortalWebAPI.APIControllers
{
    public class CustomerPaymentOptionsController : ApiController
    {
        CustomerPaymentOptionsService paymentService = new CustomerPaymentOptionsService();

        [System.Web.Http.HttpGet]
        [ActionName("GetCustomerPaymentInfo")]
        [ResponseType(typeof(CustomerPaymentInfo))]
        public IHttpActionResult GetCustomerPaymentInfo(Guid UserId, int EntityId, int SiteType,Guid BankId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UserId == Guid.Empty || EntityId <= 0)
            {
                return BadRequest(ModelState);
            }

            var result = paymentService.GetCustomerPaymentInfo(UserId, EntityId, SiteType, BankId);

            return Ok(result);
        }

        [System.Web.Http.HttpGet]
        [ActionName("GetCustomerPaymentInfoSummary")]
        [ResponseType(typeof(CustomerPaymentInfo))]
        public IHttpActionResult GetCustomerPaymentInfoSummary(Guid UserId, int EntityId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UserId == Guid.Empty || EntityId <= 0)
            {
                return BadRequest(ModelState);
            }

            var result = paymentService.GetCustomerPaymentInfoSummary(UserId, EntityId);

            return Ok(result);
        }

        [HttpPost]
        [ActionName("SaveefilePaymentOptions")]
        [ResponseType(typeof(PaymentOptionResponse))]
        public IHttpActionResult PostFeeReimbursement(CustomerPaymentInfo info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = paymentService.Save(info);

            return Ok(result);
        }

        [ActionName("GetCustomerBankDetails")]
        [ResponseType(typeof(PaymetnACH))]
        public IHttpActionResult GetCustomerBankDetails(Guid UserId, int EntityId, Guid CustId,Guid BankId)
        {
            var bakInfo = paymentService.GetCustomerBankDetails(UserId, EntityId, CustId, BankId);
            if (bakInfo == null)
            {
                return NotFound();
            }
            return Ok(bakInfo);
        }

        [HttpPost]
        [ActionName("SaveCreditCardDetails")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveCreditCardDetails(PaymentCreditCardInfo info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = paymentService.SaveCreditCardDetails(info);

            return Ok(result);
        }

        [HttpPost]
        [ActionName("SaveACHDetails")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveACHDetails(PaymetnACH info)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = paymentService.SaveACHDetails(info);

            return Ok(result);
        }
    }
}