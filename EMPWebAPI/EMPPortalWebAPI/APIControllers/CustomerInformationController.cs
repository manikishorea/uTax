using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using EMPPortal.Transactions.CustomerInformation;
using EMPPortalWebAPI.Filters;

namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class CustomerInformationController : ApiController
    {
        private CustomerInformationService customerInformationService = new CustomerInformationService();

        // GET: api/emp_CustomerInformation
        public async Task<List<CustomerInformationModel>> GetAllCustomerInformation()
        {
            string ClientIP = Request.GetClientIpAddress();
            return await customerInformationService.GetAllCustomerInformation();
        }

        [ActionName("GetNewCustomers")]
        [ResponseType(typeof(IQueryable<NewCustomersModel>))]
        public IHttpActionResult GetNewCustomers()
        {
            //string ClientIP = Request.GetClientIpAddress();
            return Ok(customerInformationService.GetNewCustomers());
        }

        // GET: api/emp_CustomerInformation/5
        [ResponseType(typeof(CustomerInformationModel))]
        public async Task<IHttpActionResult> Getemp_CustomerInformation(Guid id)
        {
            var customerInformation = await customerInformationService.GetCustomerInformation(id);
            if (customerInformation == null)
            {
                return NotFound();
            }

            return Ok(customerInformation);
        }

        // POST: api/emp_CustomerInformation
        [ActionName("Save")]
        [ResponseType(typeof(CustomerInformationDisplayDTO))]
        public IHttpActionResult Postemp_CustomerInformation(CustomerInformationModel dto)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}           
            var result = customerInformationService.Save(dto, dto.Id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [ActionName("SubSiteCustomerStatus")]
        [ResponseType(typeof(int))]
        public IHttpActionResult PosUpdate_CustomerInformation(Guid Id, Guid ParentId)
        {
            int result = customerInformationService.UpdateSubSiteCustomerInfo(Id, ParentId);
            if (result <= 0)
            {
                return NotFound();
            }
            return Ok(result);
        }

        [ActionName("CustomerInfoBySalesYear")]
        [ResponseType(typeof(CustomerModel))]
        public async Task<IHttpActionResult> GetCustomerInfoSalyesYearID(string SYGrpId, string SalesYearId)
        {
            Guid GSYGrpId, GSalesYearId;
            if (!Guid.TryParse(SYGrpId.ToString(), out GSYGrpId))
            {
                return NotFound();
            }

            if (!Guid.TryParse(SalesYearId.ToString(), out GSalesYearId))
            {
                return NotFound();
            }

            var customerInformation = await customerInformationService.GetCustomerInformationWithSalyesYearID(GSYGrpId, GSalesYearId);
            if (customerInformation == null)
            {
                return NotFound();
            }
            return Ok(customerInformation);
        }

        //[ActionName("CustomerSearchInfo")]
        //[ResponseType(typeof(CustomerModel))]
        //public IHttpActionResult GetSearchCustomerInformation(Guid UserName, string Status, string SiteType, string BankPartner, string EnrollmentStatus, string OnBoardingStatus, string SearchText, int SearchType)
        //{
        //    var customerInformation = customerInformationService.GetSearchCustomerInformation(UserName, Status, SiteType, BankPartner, EnrollmentStatus, OnBoardingStatus, SearchText, SearchType);
        //    if (customerInformation == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(customerInformation);
        //}

        //[ActionName("CustomerSearchInfoLazyLoad")]
        //[ResponseType(typeof(CustomerModel))]
        //public IHttpActionResult CustomerSearchInfoLazyLoad(Guid UserName, string Status, string SiteType, string BankPartner, string EnrollmentStatus, string OnBoardingStatus, string SearchText, int SearchType, int Count)
        //{
        //    var customerInformation = customerInformationService.CustomerSearchInfoLazyLoad(UserName, Status, SiteType, BankPartner, EnrollmentStatus, OnBoardingStatus, SearchText, SearchType, Count);
        //    if (customerInformation == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(customerInformation);
        //}

        //[ActionName("CustomerAllInfo")]
        //[ResponseType(typeof(CustomerModel))]
        //public IHttpActionResult GetSearchCustomerInformation(Guid UserName)
        //{
        //    var customerInformation = customerInformationService.GetSearchCustomerInformation(UserName);
        //    if (customerInformation == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(customerInformation);
        //}

        [ActionName("RecentlyCreate")]
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult GetRecentlyCreatedDetails(Guid UserID)
        {
            var customerRecentlyCreate = customerInformationService.GetRecentlyCreatedDetails(UserID);
            if (customerRecentlyCreate == null)
            {
                return NotFound();
            }
            return Ok(customerRecentlyCreate);
        }

        [ActionName("RecentlyUpdate")]
        [ResponseType(typeof(CustomerModel))]
        public IHttpActionResult GetRecentlyUpdateDetails(Guid UserID)
        {
            var customerRecentlyUpdate = customerInformationService.GetRecentlyUpdateDetails(UserID);
            if (customerRecentlyUpdate == null)
            {
                return NotFound();
            }
            return Ok(customerRecentlyUpdate);
        }

        [ActionName("CustomerSubSiteInfo")]
        [ResponseType(typeof(CustomerInformationModel))]
        public IHttpActionResult Postemp_SubSiteCustomerInformation(CustomerInformationModel dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            int result = customerInformationService.SaveCustomerSubSiteInfo(dto, dto.Id);

            if (result == 0)
            {
                return NotFound();
            }
            return Ok(result);
        }


        [ActionName("CustomerConfig")]
        [ResponseType(typeof(int))]
        public IHttpActionResult PosSaveCustomerconfigStatus(string CustomerId, string UserId, string SiteMapID)
        {
            Guid uId, CustId;
            if (!Guid.TryParse(UserId.ToString(), out uId))
            {
                return NotFound();
            }

            if (!Guid.TryParse(CustomerId.ToString(), out CustId))
            {
                return NotFound();
            }

            int result = customerInformationService.SaveCustomerconfigStatus(CustId, uId, SiteMapID);

            if (result > 0)
            {
                //customerInformationService.EmpCsrLogger(uId);
            }

            if (result <= 0)
            {
                return NotFound();
            }
            return Ok(result);
        }


        // POST: api/emp_CustomerInformation
        [ActionName("SaveuTaxNotCollectingSVBFee")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult PostSaveUtaxSvbFee(bool uTaxNotCollectingSVBFee, string Id)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}   
            Guid UserId;

            if (!Guid.TryParse(Id, out UserId))
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            var result = customerInformationService.SaveUtaxSVBFee(uTaxNotCollectingSVBFee, UserId);

            if (!result)
            {
                return StatusCode(HttpStatusCode.NotAcceptable);
            }

            return Ok(result);
        }

        //[HttpPost]
        //[ActionName("CustomerSearchInfoNewGrid")]
        //[ResponseType(typeof(CustomerInfoNewGrid))]
        //public IHttpActionResult GetSearchCustomerInformationNewGrid(CustomerInfoNewGrid omodel)
        //{
        //    var customerInformation = customerInformationService.GetSearchCustomerInformationNewGrid(omodel);
        //    if (customerInformation == null)
        //    {
        //        return NotFound();
        //    }
        //    return Ok(customerInformation);
        //}

        [ActionName("GetIsCustomerTaxReturn")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult GetIsCustomerTaxReturn(Guid CustomerId)
        {
            var customerInformation = customerInformationService.GetIsCustomerTaxReturn(CustomerId);
            return Ok(customerInformation);
        }

        [HttpPost]
        [ActionName("HoldUnHold")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult HoldUnHoldCustomer(Guid CustomerId, Guid UserId, string Description)
        {
            var customerInformation = customerInformationService.HoldUnHoldCustomer(CustomerId, UserId, Description);
            return Ok(customerInformation);
        }

        [ActionName("AddSO")]
        [ResponseType(typeof(bool))]
        public bool AddSO(ImportPartner info)
        {
            return customerInformationService.AddSO(info);
        }

    }
}