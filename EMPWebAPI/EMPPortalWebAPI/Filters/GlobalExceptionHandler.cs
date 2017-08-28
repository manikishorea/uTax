using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ExceptionHandling;
using System.Web.Http.ModelBinding;
using System.Web.Http.Results;

namespace EMPPortalWebAPI
{
    public class GlobalExceptionHandler
    {
        protected override BadRequestErrorMessageResult BadRequest(string modelState)
        {
            EMPPortal.Core.Utilities.ExceptionLogger.LogException("Bad Request", "", null);
            return base.BadRequest(modelState);
        }

        //public InvalidModelStateResult uBadRequest(GlobalExceptionModel modelState)
        //{
        //    EMPPortal.Core.Utilities.ExceptionLogger.LogException("Bad Request", "", null);
        //    return base.BadRequest(modelState);
        //}

        //public InvalidModelStateResult uNotFound(GlobalExceptionModel modelState)
        //{
        //    EMPPortal.Core.Utilities.ExceptionLogger.LogException("Bad Request", "", null);
        //    return base.BadRequest(modelState);
        //}
    }

    public class GlobalExceptionModel : ModelStateDictionary
    {
        public string methodname { get; set; }
    }
}