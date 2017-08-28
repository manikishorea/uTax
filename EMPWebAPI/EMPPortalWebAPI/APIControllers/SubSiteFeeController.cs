using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EMPPortal.Transactions.SubSiteFees.DTO;
using EMPPortal.Transactions.SubSiteFees;
namespace EMPPortalWebAPI.APIControllers
{
    public class SubSiteFeeController : ApiController
    {
        SubSiteFeeService subsitefeeService = new SubSiteFeeService();

        /// <summary>
        /// This method is used to Get the Fee reimbursement details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(IQueryable<SubSiteFeesDTO>))]
        public IHttpActionResult GetSubSiteFees(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = subsitefeeService.GetSubSiteFeeById(id);

            return Ok(result);
        }


        /// <summary>
        /// This method is used to post the Fee reimbursement details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Fees")]
        [ResponseType(typeof(int))]
        public IHttpActionResult PostSubSiteFees(SubSiteFeesDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid UserId;

            if (!Guid.TryParse(oDto.UserId.ToString(), out UserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }


            Guid GRefId;

            if (!Guid.TryParse(oDto.refId.ToString(), out GRefId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = subsitefeeService.Save(oDto);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Fee reimbursement details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("CustomerAssociateFees")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostCustomerAssociateFees(TransmitterFeeDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = subsitefeeService.SaveCustomerAssociatedFees(oDto);
            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Fee reimbursement details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("UpdateCustomerAssociateFees")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UpdateCustomerAssociateFees(TransmitterFeeDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = subsitefeeService.UpdateCustomerAssociatedFees(oDto);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("IsEnrollmentSubmit")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult IsEnrollmentSubmitData(Guid Id)
        {
            var result = subsitefeeService.IsEnrollmentSubmit(Id);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("IsSalesYearBankLst")]
        [ResponseType(typeof(IQueryable<BankDateCrossDTO>))]
        public IHttpActionResult SalesYearBank(Guid Id)
        {
            var result = subsitefeeService.IsSalesYearBank(Id);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("getFeeLink")]
        [ResponseType(typeof(IQueryable<BankDateCrossDTO>))]
        public IHttpActionResult getFeeLink(Guid CustomerId)
        {
            return Ok(subsitefeeService.GetFeeLink(CustomerId));
        }
    }
}
