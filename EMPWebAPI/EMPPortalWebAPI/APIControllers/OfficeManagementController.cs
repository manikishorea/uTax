using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.Configuration.DTO;
using EMPPortal.Transactions.Configuration;
using System.Threading.Tasks;
using System.Web.Http.Description;
using EMPPortalWebAPI.Filters;
using EMPPortal.Transactions.OfficeManagementTransactions;

namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class OfficeManagementController : ApiController
    {
        OfficeManagementService _OfficeManagementService = new OfficeManagementService();

        /// <summary>
        /// This method is used to Get the Sub Site Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        [ActionName("update")]
        public IHttpActionResult PostOfficeManagementUpdate(Guid id,string salesyearid)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            //Guid _salesyearid = Guid.Empty;
            //if (!string.IsNullOrEmpty(salesyearid))
            //{
            //    Guid.TryParse(salesyearid, out _salesyearid);
            //}

            var result = _OfficeManagementService.UpdateOfficeManagement(id, salesyearid);

            return Ok(result);
        }


        [HttpPost]
        [ActionName("get")]
        [ResponseType(typeof(CustomerInfoNewGrid))]
        public IHttpActionResult GetOfficeManagement(CustomerInfoNewGrid omodel)
        {
            var customerInformation = _OfficeManagementService.GetOfficeManagement(omodel);
            if (customerInformation == null)
            {
                return NotFound();
            }
            return Ok(customerInformation);
        }

        [ActionName("RecentlyCreate")]
        [ResponseType(typeof(CustomerRecenltyModel))]
        public IHttpActionResult GetRecentlyCreatedDetails(Guid UserID,int Mainentity)
        {
            var customerRecentlyCreate = _OfficeManagementService.GetRecentlyCreatedDetails(UserID, Mainentity);
            if (customerRecentlyCreate == null)
            {
                return NotFound();
            }
            return Ok(customerRecentlyCreate);
        }

        [ActionName("RecentlyUpdate")]
        [ResponseType(typeof(CustomerRecenltyModel))]
        public IHttpActionResult GetRecentlyUpdateDetails(Guid UserID,int  Mainentity)
        {
            var customerRecentlyUpdate = _OfficeManagementService.GetRecentlyUpdateDetails(UserID, Mainentity);
            if (customerRecentlyUpdate == null)
            {
                return NotFound();
            }
            return Ok(customerRecentlyUpdate);
        }
    }
}
