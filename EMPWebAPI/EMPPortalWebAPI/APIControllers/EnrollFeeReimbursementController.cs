using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.EnrollFeeReimbursement.DTO;
using EMPPortal.Transactions.EnrollFeeReimbursement;
using System.Threading.Tasks;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;
namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class EnrollFeeReimbursementController : ApiController
    {
        EnrollFeeReimbursementService feereimbursementService = new EnrollFeeReimbursementService();

        /// <summary>
        /// This method is used to Get the Fee reimbursement details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(EnrollFeeReimbursementDTO))]
        public async Task<IHttpActionResult> GetEnrollFeeReimbursement(Guid Id,Guid BankId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (Id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = await feereimbursementService.GetEnrollFeeReimbursementById(Id, BankId);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Fee reimbursement details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostFeeReimbursement(EnrollFeeReimbursementDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = feereimbursementService.Save(oDto);

            return Ok(result);
        }
    }
}
