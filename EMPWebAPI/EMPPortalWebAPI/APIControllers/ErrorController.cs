using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EMP.Core.ExceptionHandle;
using EMP.Core.ExceptionHandle.DTO;

namespace EMPPortalWebAPI.APIControllers
{
    public class ErrorController : ApiController
    {
        [HttpPost]
        [ResponseType(typeof(bool))]
        public IHttpActionResult LogException(ExceptionModel oDto)
        {
            ExceptionHandleService _ExceptionHandleService = new ExceptionHandleService();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var result = _ExceptionHandleService.LogException(oDto);
            return Ok(result);
        }
    }
}
