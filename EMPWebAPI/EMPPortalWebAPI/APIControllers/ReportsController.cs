using EMPPortal.Transactions.FeeReimbursement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using EMPPortal.Transactions.ReportsService;
using EMPPortalWebAPI.Filters;
using EMPPortal.Transactions.Reports.DTO;
namespace EMPPortalWebAPI.APIControllers
{
    [TokenAuthorization]
    public class ReportsController : ApiController
    {
        ReportsService oReportsService = new ReportsService();

        [HttpGet]
        [ActionName("FeeSetupReport")]
        [ResponseType(typeof(IQueryable<FeeSetupReportDTO>))]
        public IHttpActionResult FeeSetupReport(string UserId)
        {
            var result = oReportsService.GetFeeSetUpReport(UserId);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("AllMainCustomers")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult AllMainCustomers()
        {
            var result = oReportsService.GetCustomerMain();
            return Ok(result);
        }

        [HttpGet]
        [ActionName("AllSubCustomers")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult AllSubCustomers(string ParentIds)
        {
            var result = oReportsService.GetCustomerSub(ParentIds);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("NoBankAppSubmission")]
        [ResponseType(typeof(IQueryable<FeeSetupReportDTO>))]
        public IHttpActionResult GetNoBankAppSubmissionReport(string UserId)
        {
            var result = oReportsService.GetNoBankAppSubmissionReport(UserId);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("AllNoBankSubmissionCustomerMain")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult GetNoBankSubmissionCustomerMain()
        {
            var result = oReportsService.GetNoBankSubmissionCustomerMain();
            return Ok(result);
        }


        [HttpGet]
        [ActionName("LastLoginInfo")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult GetLastLoginReport(Guid UserID)
        {
            var result = oReportsService.GetLastLoginReport(UserID);
            return Ok(result);
        }


        [HttpGet]
        [ActionName("AllNewEnrollmentCases")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult GetAllNewEnrollmentCases(Guid UserID)
        {
            var result = oReportsService.GetAllNewEnrollmentCases(UserID);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("AllCloseEnrollmentCases")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult GetAllCloseEnrollmentCases(Guid UserID)
        {
            var result = oReportsService.GetAllCloseEnrollmentCases(UserID);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("StaleEnrollmentCases")]
        [ResponseType(typeof(IQueryable<CustomerDTO>))]
        public IHttpActionResult GetStaleEnrollmentCases(Guid UserID)
        {
            var result = oReportsService.GetStaleEnrollmentCases(UserID);
            return Ok(result);
        }

        [ActionName("getEnrollmentsList")]
        [ResponseType(typeof(List<EnrollmentStatusReport>))]
        public IHttpActionResult getEnrollmentsList(string strguid)
        {
            var result = oReportsService.getEnrollmentsList(strguid);
            return Ok(result);
        }
    }
}