using EMPPortal.Transactions.BankSelection.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EMPPortal.Transactions.BankSelection;

namespace EMPPortalWebAPI.APIControllers
{
    public class BankSelectionController : ApiController
    {
        BankSelectionService objService = new BankSelectionService();

        [ActionName("getCutomerBanks")]
        [ResponseType(typeof(CustomerBanksResponse))]
        public IHttpActionResult getCutomerBanks(Guid CustomerId,int EntityId)
        {
            var result = objService.getCutomerBanks(CustomerId, EntityId);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("setDefaultBank")]
        [ResponseType(typeof(CustomerBanksResponse))]
        public IHttpActionResult setDefaultBank(Guid CustomerId, Guid UserId, Guid BankId)
        {
            objService= new BankSelectionService();
            var result = objService.setDefaultBank(CustomerId, UserId, BankId);
            return Ok(result);
        }

        [ActionName("CheckUnlock")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult CheckUnlock(Guid CustomerId)
        {
            var result = objService.CheckUnlock(CustomerId);
            return Ok(result);
        }


        [HttpPost]
        [ActionName("getActivityBankStatus")]
        [ResponseType(typeof(CustomerBanksResponse))]
        public IHttpActionResult getActivityBankStatus(Guid CustomerId)
        {
            var result = objService.getActivityBankStatus(CustomerId);
            return Ok(result);
        }

        [ActionName("getOtherBankStatus")]
        [ResponseType(typeof(List<Guid>))]
        public IHttpActionResult getOtherBankStatus(Guid CustomerId,Guid BankId)
        {
            var result = objService.getOtherBankStatus(CustomerId, BankId);
            return Ok(result);
        }
    }
}
