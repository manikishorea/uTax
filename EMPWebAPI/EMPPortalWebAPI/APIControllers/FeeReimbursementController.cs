using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.FeeReimbursement.DTO;
using EMPPortal.Transactions.FeeReimbursement;
using System.Threading.Tasks;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;
namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class FeeReimbursementController : ApiController
    {
        FeeReimbursementService feereimbursementService = new FeeReimbursementService();

        /// <summary>
        /// This method is used to Get the Fee reimbursement details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(FeeReimbursementDTO))]
        public async Task<IHttpActionResult> GetFeeReimbursement(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = await feereimbursementService.GetFeeReimbursementById(id);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Fee reimbursement details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostFeeReimbursement(FeeReimbursementDTO oDto)
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
