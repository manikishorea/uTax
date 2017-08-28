using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.MainOffice.DTO;
using EMPPortal.Transactions.MainOffice;
using System.Threading.Tasks;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;
namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class MainOfficeController : ApiController
    {
        MainOfficeConfigService mainofficeconfigService = new MainOfficeConfigService();

        /// <summary>
        /// This method is used to Get the main office details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [ResponseType(typeof(MainOfficeDTO))]
        public async Task<IHttpActionResult> GetMainOffice(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = await mainofficeconfigService.GetMainOfficeById(id);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the main office details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostMainOffice(MainOfficeDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = mainofficeconfigService.Save(oDto);

            return Ok(result);
        }


    }
}

