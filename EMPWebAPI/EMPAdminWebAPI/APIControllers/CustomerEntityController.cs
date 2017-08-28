using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;
using System.Web.Mvc;
using EMPAdmin.Transactions.SalesYear;
using EMPAdmin.Transactions.SalesYear.DTO;

using EMPAdminWebAPI.Filters;
using EMPAdmin.Transactions.Entity.DTO;
using EMPAdmin.Transactions.Entity;

namespace EMPAdminWebAPI.APIControllers
{
    [TokenAuthorization]
    public class CustomerEntityController : ApiController
    {
        SalesYearService _SalesYearService = new SalesYearService();

        [ResponseType(typeof(SalesYearDTO))]
        public IHttpActionResult GetAllSalesYearMasters()
        {
            var Sales = _SalesYearService.GetAllEntity();
            if (Sales == null)
            {
                return NotFound();
            }

            return Ok(Sales);
        }
    }
}
