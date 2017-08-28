using EMP.Core.ZipCodeService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMP.Core.DTO;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;

namespace EMPPortalWebAPI.APIControllers
{
  //  [TokenAuthorization]
    public class ZipCodeController : ApiController
    {
        public ZipCodeService _ZipCodeService = new ZipCodeService();

        // GET: api/UserMasters
        // [Route("GetAllUser")]
        [ResponseType(typeof(ZipCodeModel))]
        public IHttpActionResult GetAllChangePassword(string id)
        {
            if (string.IsNullOrEmpty(id)) {
                return BadRequest(id);
            }

            var data = _ZipCodeService.GetZipCodeByCode(id);
            //if (data == null)
            //{
            //    return NotFound();
            //}

            return Ok(data);
        }
    }
}
