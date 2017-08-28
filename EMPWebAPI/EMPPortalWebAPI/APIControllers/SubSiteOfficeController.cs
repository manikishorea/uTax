using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.SubSiteOfficeConfiguration.DTO;
using EMPPortal.Transactions.SubSiteOfficeConfiguration;
using System.Web.Http.Description;
using System.Threading.Tasks;

namespace EMPPortalWebAPI.APIControllers
{
    public class SubSiteOfficeController : ApiController
    {
        SubSiteOfficeConfigService subsiteofficeconfigService = new SubSiteOfficeConfigService();

        /// <summary>
        /// This method is used to Get the Sub Site Office Configuration Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("SubSiteOfficeConfig")]
        [ResponseType(typeof(SubSiteOfficeConfigDTO))]
        public async Task<IHttpActionResult> Get(Guid id, Guid parentId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = await subsiteofficeconfigService.GetSubSiteOfficeConfigById(id, parentId);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Sub Site Office Configuration Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [ActionName("SaveSubSiteOfficeConfig")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostOnBoardingService(SubSiteOfficeConfigDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = subsiteofficeconfigService.SaveSubSiteOfficeConfigInfo(oDto);
            if (result == Guid.Empty)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// This method is used to Get the Customer Details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("GetCustomerNote")]
        [ResponseType(typeof(IQueryable<CustomerNotesDTO>))]
        public IHttpActionResult GetCustomer(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = subsiteofficeconfigService.GetCustomerNotesById(id);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Customer Notes Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [ActionName("SaveCustomerNotes")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostCustomerNotes(CustomerNotesDTO oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = subsiteofficeconfigService.SaveCustomerNoteInfo(oDto);
            if (result == Guid.Empty)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("OfficeBankFeeConfig")]
        [ResponseType(typeof(IQueryable<SubSiteBankFeeConfigDTO>))]
        public IHttpActionResult GetSubSiteOfficeBankFeeConfig(Guid id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id == Guid.Empty)
            {
                return BadRequest(ModelState);
            }

            var result = subsiteofficeconfigService.GetSubSiteBankFeeById(id);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to post the Sub Site office fee Details
        /// </summary>
        /// <param name="oDto"></param>
        /// <returns></returns>
        [ActionName("SaveOfficeBankFeeConfig")]
        [ResponseType(typeof(Guid))]
        public IHttpActionResult PostSubSiteOfficeFeeConfig(List<SubSiteBankFeeConfigDTO> oDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (oDto.ToList().Count == 0)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            int result = subsiteofficeconfigService.SaveSubSiteBankFeeConfig(oDto); //SaveSubSiteBankFeeConfigInfo
            if (result == -1)
            {
                return NotFound();
            }
            return Ok(result);
        }

        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        [ActionName("ValidEFINUser")]
        [ResponseType(typeof(IQueryable<SubSiteBankFeeConfigDTO>))]
        public bool GetUserValidOrnot(string OwnID, string ParentID, Guid CustomerId)
        {
            if (string.IsNullOrEmpty(OwnID))
            {
                return false;
            }

            if (string.IsNullOrEmpty(ParentID))
            {
                return false;
            }

            return subsiteofficeconfigService.GetValidUserID(OwnID, ParentID, CustomerId);
        }
        
        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        [ActionName("ValidEFINUserFromuTax")]
        [ResponseType(typeof(IQueryable<SubSiteBankFeeConfigDTO>))]
        public bool GetUserValidOrnotFromUtax(string OwnID, string ParentID, Guid CustomerId)
        {
            if (string.IsNullOrEmpty(ParentID))
            {
                return false;
            }

            return subsiteofficeconfigService.GetValidUserIDFromuTax(OwnID, ParentID, CustomerId);
        }

        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        [ActionName("BankFeesConfig")]
        [ResponseType(typeof(IQueryable<SubSiteBankFeeConfigDTO>))]
        public IHttpActionResult GetSubSiteOfficeBankFee(Guid UserId)
        {
            var result = subsiteofficeconfigService.SubSiteOfficeBankFee(UserId);

            return Ok(result);
        }

        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>        
        /// <returns></returns>
        [ActionName("SubSiteFeeConfigAfterApproved")]
        [ResponseType(typeof(List<ApprovedBankAndAddOnFeeDTO>))]
        public IHttpActionResult GetSubSiteFeeConfigUpdateAfterApprove(Guid UserId)
        {
            var result = subsiteofficeconfigService.GetSubSiteFeeConfigUpdateAfterApprove(UserId);
            return Ok(result);
        }

      
        [ActionName("GetAccountId")]
        [ResponseType(typeof(string))]
        public string GetAccountId(Guid Id)
        {
            return subsiteofficeconfigService.GetAccountId(Id);
        }
    }
}
