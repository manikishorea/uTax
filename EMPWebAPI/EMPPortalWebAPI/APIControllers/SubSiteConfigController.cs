using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.SubSite.DTO;
using EMPPortal.Transactions.SubSite;
using System.Threading.Tasks;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;
namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class SubSiteConfigController : ApiController
    {
        SubSiteConfigService SubSiteConfigService = new SubSiteConfigService();

        /// <summary>
        /// This method is used to Get the Sub Site Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Get")]
        [ResponseType(typeof(SubSiteDTO))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = await SubSiteConfigService.GetById(id);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to Get the Sub Site Configuration Details only Few Fiels by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("Main")]
        [ResponseType(typeof(SubSiteDTO))]
        public async Task<IHttpActionResult> GetFewFields(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = await SubSiteConfigService.GetById(id);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Sub Site Configuration Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
      //  [HttpPost]
        [ActionName("BankService")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostBankService(SubSiteBankServiceDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = SubSiteConfigService.BankServiceSave(oDto);
            if (result == Guid.Empty)
            {
                return NotFound();
            }


            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Sub Site Configuration Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
      //  [HttpPost]
        [ActionName("OnBoardingService")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostOnBoardingService(SubSiteOnBoardingDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = SubSiteConfigService.OnBoardingServiceSave(oDto);
            if (result == Guid.Empty)
            {
                return NotFound();
            }
            return Ok(result);
        }


        /// <summary>
        /// This method is used to post the Sub Site Configuration Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
      //  [HttpPost]
        [ActionName("SupportService")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostSupportService(SubSiteSupportDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = SubSiteConfigService.SupportServiceSave(oDto);
            if (result == Guid.Empty)
            {
                return NotFound();
            }
            return Ok(result);
        }


    }
}
