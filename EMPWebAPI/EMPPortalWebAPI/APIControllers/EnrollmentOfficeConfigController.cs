using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.EnrollmentOfficeConfig.DTO;
using EMPPortal.Transactions.EnrollmentOfficeConfig;
using System.Web.Http.Description;
using System.Threading.Tasks;

namespace EMPPortalWebAPI.APIControllers
{
    public class EnrollmentOfficeConfigController : ApiController
    {
        EnrollmentOfficeConfigService _EnrollmentOfficeConfigService = new EnrollmentOfficeConfigService();

        /// <summary>
        /// This method is used to Get the Sub Site Office Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [ActionName("Self")]
        [ResponseType(typeof(EnrollmentOfficeConfigDTO))]
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

            var result = _EnrollmentOfficeConfigService.GetEnrollmentOfficeConfig(UserId);

            return Ok(result);
        }


        /// <summary>
        /// This method is used to Get the Sub Site Office Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// 
        [ActionName("MainConfig")]
        [ResponseType(typeof(EnrollmentOfficeConfigDTO))]
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

            var result = _EnrollmentOfficeConfigService.GetEnrollmentOfficeMainConfig(UserId);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Sub Site Office Configuration Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostEnrollmentOfficeConfig(EnrollmentOfficeConfigDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            var result = _EnrollmentOfficeConfigService.SaveEnrollmentOfficeConfig(oDto);

            if (result == -1)
            {
                return NotFound();
            }

            return Ok(result);
        }

    }
}
