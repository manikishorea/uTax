using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.EnrollmentAffiliateConfig.DTO;
using EMPPortal.Transactions.EnrollmentAffiliateConfig;
using System.Web.Http.Description;
using System.Threading.Tasks;

namespace EMPPortalWebAPI.APIControllers
{
    public class EnrollmentAffiliateConfigController : ApiController
    {
        EnrollmentAffiliateConfigService _EnrollmentAffiliateConfigService = new EnrollmentAffiliateConfigService();

        /// <summary>
        /// This method is used to Get the Sub Site Office Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [ActionName("Self")]
        [ResponseType(typeof(EnrollmentAffiliateConfigDTO))]
        public IHttpActionResult GetEnrollmentOfficeConfig(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid UserId;
            if (!Guid.TryParse(id.ToString(), out UserId))
            {
                return NotFound();
            }

            if (UserId == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = _EnrollmentAffiliateConfigService.GetEnrollmentAffiProgConfig(UserId);

            return Ok(result);
        }


        /// <summary>
        /// This method is used to Get the Sub Site Office Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [ActionName("MainConfig")]
        [ResponseType(typeof(EnrollmentAffiliateConfigDTO))]
        public IHttpActionResult GetEnrollmentOfficeMainConfig(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid UserId;
            if (!Guid.TryParse(id.ToString(), out UserId))
            {
                return NotFound();
            }

            if (UserId == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = _EnrollmentAffiliateConfigService.GetEnrollmentAffiProgMainConfig(UserId);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Sub Site Office Configuration Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostEnrollmentOfficeConfig(EnrollmentAffiliateConfigDetailDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _EnrollmentAffiliateConfigService.SaveEnrollmentAffiProgConfig(oDto);

            if (result == -1)
            {
                return NotFound();
            }

            return Ok(true);
        }

    }
}
