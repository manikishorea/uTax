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
using EMPAdmin.Transactions.SalesYear;
using EMPAdmin.Transactions.SalesYear.DTO;

using EMPAdminWebAPI.Filters;
using EMPAdmin.Transactions.Entity.DTO;
using EMPAdmin.Transactions.Entity;
using System.Web.Http.Results;
using EMP.Core.DataMigration;

namespace EMPAdminWebAPI.APIControllers
{
    [TokenAuthorization]
    public class ArchiveProcessController : ApiController
    {
        DataMigrationService _DataMigrationService = new DataMigrationService();
        [HttpGet]
        [ResponseType(typeof(int))]
        public IHttpActionResult GetArchiveProcess(string status, string tokenid)
        {
            Guid TokenId;

            if (!Guid.TryParse(tokenid, out TokenId))
            {
                return NotFound();
            }

            if (status == "init")
            {
                int Sales = _DataMigrationService.SetArchiveDataCount(TokenId);

                return Ok(Sales);
            }
            else
            {
                int Sales = _DataMigrationService.SetArchiveDataStatus(TokenId);
                return Ok(Sales);
            }
        }

        [HttpPost]
        [ResponseType(typeof(int))]
        public IHttpActionResult PostArchiveProcess(string tokenid, string salesyearid)
        {
            Guid TokenId;

            if (!Guid.TryParse(tokenid, out TokenId))
            {
                return NotFound();
            }

            Guid SalesYearId;

            if (!Guid.TryParse(salesyearid, out SalesYearId))
            {
                return NotFound();
            }

            bool Sales = _DataMigrationService.SetArchiveData(TokenId, SalesYearId);

            return Ok(Sales);
        }
    }
}
