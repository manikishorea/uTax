using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using EMP.Core.DataMigration.DTO;
using EMP.Core.DataMigration;

namespace EMPPortalWebAPI.APIControllers
{
    public class ArchiveController : ApiController
    {
        public DataMigrationService _DataMigrationService = new DataMigrationService();

        /// <summary>
        /// This method is used to get the sub site office fee config details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ActionName("SalesYears")]
        [ResponseType(typeof(IQueryable<SalesYearDTO>))]
        public IHttpActionResult GetAllSalesYears(string Id)
        {
            Guid GGroupId = Guid.Empty;
            if (!Guid.TryParse(Id, out GGroupId))
            {
                return NotFound();
            }

            var result = _DataMigrationService.GetArchiveSalesYears(GGroupId);
            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet]
        [ActionName("CustomerInfo")]
        [ResponseType(typeof(ArchiveMainSiteDataModel))]
        public IHttpActionResult GetMainSite(string SYGrpID,string SalesYearId, bool IsAddEFIN = false)
        {
            Guid GSalesYearId, GSYGrpID;
            if (!Guid.TryParse(SYGrpID, out GSYGrpID))
            {
                return NotFound();
            }

            if (!Guid.TryParse(SalesYearId, out GSalesYearId))
            {
                return NotFound();
            }

            ArchiveCustomerModel _ArchiveDataModel = new ArchiveCustomerModel();
            var dbresule = _DataMigrationService.GetCustomerBySalesYearID(GSYGrpID, GSalesYearId, IsAddEFIN);

            if (dbresule == null)
            {
                return NotFound();
            }

            return Ok(dbresule);
        }
        

        [HttpGet]
        [ActionName("MainSite")]
        [ResponseType(typeof(ArchiveMainSiteDataModel))]
        public IHttpActionResult GetMainSite(string Id)
        {
            Guid GUserId;
            if (!Guid.TryParse(Id, out GUserId))
            {
                return NotFound();
            }

            ArchiveMainSiteDataModel _ArchiveDataModel = new ArchiveMainSiteDataModel();
            var dbresule = _DataMigrationService.GetArchiveDataMainSite(GUserId);

            if (dbresule == null)
            {
                return NotFound();
            }

            return Ok(dbresule);
        }

        [HttpGet]
        [ActionName("Enrollment")]
        [ResponseType(typeof(ArchiveEnrollmentDataModel))]
        public IHttpActionResult GetEnrollment(string Id,string parentid)
        {
            Guid GUserId;
            if (!Guid.TryParse(Id, out GUserId))
            {
                return NotFound();
            }

            Guid GParentId;
            if (!Guid.TryParse(parentid, out GParentId))
            {
                return NotFound();
            }

            var dbresule = _DataMigrationService.GetArchiveDataEnrollment(GUserId, GParentId);

            if (dbresule == null)
            {
                return NotFound();
            }

            return Ok(dbresule);
        }

        [HttpGet]
        [ActionName("SubSite")]
        [ResponseType(typeof(ArchiveMainSiteDataModel))]
        public IHttpActionResult GetSubSite(string Id, string parentid)
        {
            Guid GUserId;
            if (!Guid.TryParse(Id, out GUserId))
            {
                return NotFound();
            }

            Guid GParentId;
            if (!Guid.TryParse(parentid, out GParentId))
            {
                return NotFound();
            }

            var dbresule = _DataMigrationService.GetArchiveDataSubSite(GUserId, GParentId);

            if (dbresule == null)
            {
                return NotFound();
            }

            return Ok(dbresule);
        }


        [HttpGet]
        [ActionName("AffiliateEnrollment")]
        [ResponseType(typeof(ArchiveMainSiteDataModel))]
        public IHttpActionResult GetAffiliateProgram(string entityid, string customerid)
        {
            int GEntityId;
            if (!int.TryParse(entityid, out GEntityId))
            {
                return NotFound();
            }

            Guid GCustomerId;
            if (!Guid.TryParse(customerid, out GCustomerId))
            {
                return NotFound();
            }

            var dbresule = _DataMigrationService.GetAffiliateProgram(GEntityId, GCustomerId);

            if (dbresule == null)
            {
                return NotFound();
            }

            return Ok(dbresule);
        }

        [HttpGet]
        [ActionName("subsitebank")]
        [ResponseType(typeof(BankQuestionDTO))]
        public IHttpActionResult banksubsitebankquestion(string id = "")
        {
            Guid entityGuid = Guid.Empty;
            if (!Guid.TryParse(id, out entityGuid))
            {
                return NotFound();
            }
            var data = _DataMigrationService.GetSubSiteBankAndQuestions(entityGuid);
            if (data == null)
            {
                return NotFound();
            }
            return Ok(data);
        }

        [ActionName("getBankFee")]
        [ResponseType(typeof(List<BankFeeDTO>))]
        public IHttpActionResult getBankFee(Guid CustomerId)
        {
            var result = _DataMigrationService.getBankFee(CustomerId);
            return Ok(result);
        }

        [ActionName("getBankEnrollmentStatus")]
        [ResponseType(typeof(string))]
        public IHttpActionResult getBankEnrollmentStatus(Guid CustomerId, Guid bankid)
        {
            var result = _DataMigrationService.getBankEnrollmentStatus(CustomerId, bankid);
            return Ok(result);
        }

    }
}
