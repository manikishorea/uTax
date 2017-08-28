using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EMPPortal.Transactions.EnrollmentBankSelectionInfo.DTO;
using EMPPortal.Transactions.EnrollmentBankSelectionInfo;
using System.Web.Http.Description;

namespace EMPPortalWebAPI.APIControllers
{
    public class EnrollmentBankSelectionController : ApiController
    {
        EnrollmentBankSelectionService enrollmentbankselectionService = new EnrollmentBankSelectionService();

        [ActionName("GetBankandFeesInfo")]
        [ResponseType(typeof(IQueryable<EnrollmentBankSelectionDTO>))]
        public IHttpActionResult GetBankandFeesInfo(string entityid = "", string userid = "", string parentid = "")//, string bankid = "")
        {
            int entityGuid = 0;
            if (!int.TryParse(entityid, out entityGuid))
            {
                return NotFound();
            }

            Guid userGuid = Guid.Empty;
            if (!Guid.TryParse(userid, out userGuid))
            {
                return NotFound();
            }

            //Guid GBankId = Guid.Empty;
            //if (!Guid.TryParse(bankid, out GBankId))
            //{
            //    return NotFound();
            //}

            var result = enrollmentbankselectionService.GetBankandFeesInfo(entityGuid, userGuid, parentid);

            return Ok(result);
        }

        [ActionName("SaveBankandFeesInfo")]
        [ResponseType(typeof(IQueryable<bool>))]
        public IHttpActionResult SaveBankandFeesInfo(EnrollmentBankSelectionDTO dto)
        {
            Guid userGuid = Guid.Empty;
            if (!Guid.TryParse(dto.UserId.ToString(), out userGuid))
            {
                return NotFound();
            }

            Guid parentGuid = Guid.Empty;
            if (!Guid.TryParse(dto.CustomerId.ToString(), out parentGuid))
            {
                return NotFound();
            }

            var result = enrollmentbankselectionService.EnrollmentBankSelectSave(dto);

            return Ok(result);
        }

        [ActionName("EnrollmentBankSelection")]
        [ResponseType(typeof(IQueryable<EnrollmentBankSelectionDTO>))]
        public IHttpActionResult GetEnrollmentBankSelection(string userid = "", string Parentid = "", bool IsStaging = false, string bankid = "")
        {
            Guid userGuid = Guid.Empty;
            if (!Guid.TryParse(userid, out userGuid))
            {
                return NotFound();
            }

            Guid parentGuid = Guid.Empty;
            if (!Guid.TryParse(Parentid, out parentGuid))
            {
                return NotFound();
            }

            Guid GBankId = Guid.Empty;
            if (!Guid.TryParse(bankid, out GBankId))
            {
                // return NotFound();
            }

            var result = enrollmentbankselectionService.GetEnrollmentBankSelection(userGuid, parentGuid, IsStaging, GBankId);

            return Ok(result);
        }

        [ActionName("GetBankSelectedByCustomer")]
        [ResponseType(typeof(string))]
        public IHttpActionResult GetBankSelectedByCustomer(Guid CustomerId, Guid bankid)
        {
            var result = enrollmentbankselectionService.GetBankSelectedByCustomer(CustomerId, bankid);
            return Ok(result);
        }

        [ActionName("SaveTPGBankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveTPGBankEnrollment(TPGBankEnrollment enrollment)
        {
            var result = enrollmentbankselectionService.SaveTPGBankEnrollment(enrollment);
            return Ok(result);
        }

        [ActionName("GetTPGBankEnrollment")]
        [ResponseType(typeof(TPGBankEnrollment))]
        public IHttpActionResult GetTPGBankEnrollment(Guid CustomerId, bool IsStaging, Guid bankid)
        {
            var result = enrollmentbankselectionService.GetTPGBankEnrollment(CustomerId, IsStaging, bankid);
            return Ok(result);
        }

        [ActionName("SaveRABankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveRABankEnrollment(RABankEnrollment enrollment)
        {
            var result = enrollmentbankselectionService.SaveRABankEnrollment(enrollment);
            return Ok(result);
        }

        [ActionName("GetRABankEnrollment")]
        [ResponseType(typeof(RABankEnrollment))]
        public IHttpActionResult GetRABankEnrollment(Guid CustomerId, bool IsStaging, Guid bankid)
        {
            var result = enrollmentbankselectionService.GetRABankEnrollment(CustomerId, IsStaging, bankid);
            return Ok(result);
        }

        [ActionName("SaveRBBankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveRBBankEnrollment(RBBankEnrollment enrollment)
        {
            var result = enrollmentbankselectionService.SaveRBBankEnrollment(enrollment);
            return Ok(result);
        }

        [ActionName("GetRBBankEnrollment")]
        [ResponseType(typeof(RBBankEnrollment))]
        public IHttpActionResult GetRBBankEnrollment(Guid CustomerId, bool IsStaging, Guid bankid)
        {
            var result = enrollmentbankselectionService.GetRBBankEnrollment(CustomerId, IsStaging, bankid);
            return Ok(result);
        }

        [ActionName("getFeeLimit")]
        [ResponseType(typeof(RBBankEnrollment))]
        public IHttpActionResult getFeeLimit(Guid CustomerId, Guid ParentId, Guid BankId)
        {
            var result = enrollmentbankselectionService.getFeeLimit(CustomerId, ParentId, BankId);
            return Ok(result);
        }

        [ActionName("saveEnrollmenttoService")]
        [ResponseType(typeof(int))]
        public IHttpActionResult saveEnrollmenttoService(Guid CustomerId, Guid UserId, Guid BankId, Guid Prefer)
        {
            var result = enrollmentbankselectionService.saveEnrollmenttoService(CustomerId, UserId, BankId, Prefer);
            return Ok(result);
        }

        [ActionName("IsAcivated")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult IsAcivated(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.IsAcivated(CustomerId);
            return Ok(result);
        }

        [ActionName("getBankEnrollmentStatus")]
        [ResponseType(typeof(string))]
        public IHttpActionResult getBankEnrollmentStatus(Guid CustomerId, Guid bankid)
        {
            var result = enrollmentbankselectionService.getBankEnrollmentStatus(CustomerId, bankid);
            return Ok(result);
        }

        [ActionName("getEnrollmentStatusInfo")]
        [ResponseType(typeof(BankEnrollmentStatusInfo))]
        public IHttpActionResult getEnrollmentStatusInfo(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.getEnrollmentStatusInfo(CustomerId);
            return Ok(result);
        }

        //[ActionName("IsBankEnrollmentSubmitted")]
        //[ResponseType(typeof(bool))]
        //public IHttpActionResult IsBankEnrollmentSubmitted(Guid CustomerId)
        //{
        //    var result = enrollmentbankselectionService.IsBankEnrollmentSubmitted(CustomerId);
        //    return Ok(result);
        //}

        [ActionName("unlockEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult unlockEnrollment(UnlockEnrollment req)
        {
            var result = enrollmentbankselectionService.unlockEnrollment(req);
            return Ok(result);
        }

        [ActionName("GetUnlockedBanks")]
        [ResponseType(typeof(List<Guid>))]
        public IHttpActionResult GetUnlockedBanks(Guid userid)
        {
            var result = enrollmentbankselectionService.GetUnlockedBanks(userid);
            return Ok(result);
        }

        [ActionName("GetRejectedBanks")]
        [ResponseType(typeof(List<Guid>))]
        public IHttpActionResult GetRejectedBanks(Guid userid)
        {
            var result = enrollmentbankselectionService.GetRejectedBanks(userid);
            return Ok(result);
        }

        [ActionName("SaveNextTPGBankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveNextTPGBankEnrollment(TPGBankEnrollment enrollment)
        {
            var result = enrollmentbankselectionService.SaveNextTPGBankEnrollment(enrollment);
            return Ok(result);
        }

        [ActionName("SaveNextRBBankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveNextRBBankEnrollment(RBBankEnrollment enrollment)
        {
            var result = enrollmentbankselectionService.SaveNextRBBankEnrollment(enrollment);
            return Ok(result);
        }

        [ActionName("SaveNextRABankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult SaveNextRABankEnrollment(RABankEnrollment enrollment)
        {
            var result = enrollmentbankselectionService.SaveNextRABankEnrollment(enrollment);
            return Ok(result);
        }


        [ActionName("GetTPGBankObjects")]
        [ResponseType(typeof(TPGBankEnrollment))]
        public IHttpActionResult GetTPGBankInfo(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.getTPGBankObjectInfo(CustomerId);
            return Ok(result);
        }

        [ActionName("getRABankObjectInfo")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult getRABankObjectInfo(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.getRABankObjectInfo(CustomerId);
            return Ok(result);
        }

        [ActionName("getRBBankObjectInfo")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult getRBBankObjectInfo(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.getRBBankObjectInfo(CustomerId);
            return Ok(result);
        }

        [ActionName("AcceptBankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult AcceptBankEnrollment(Guid CustomerId, Guid UserId, Guid bankid)
        {
            var result = enrollmentbankselectionService.AcceptBankEnrollment(CustomerId, UserId, bankid);
            return Ok(result);
        }

        [ActionName("RejectBankEnrollment")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult RejectBankEnrollment(Guid CustomerId, Guid UserId, Guid bankid)
        {
            var result = enrollmentbankselectionService.RejectBankEnrollment(CustomerId, UserId, bankid);
            return Ok(result);
        }

        [ActionName("SubmitBankApptoXlink")]
        [ResponseType(typeof(XlinkResponseModel))]
        public IHttpActionResult SubmitBankApptoXlink(Guid CustomerId, Guid UserId, Guid bankid)
        {
            var result = enrollmentbankselectionService.SubmitBankApptoXlink(CustomerId, UserId, bankid);
            return Ok(result);
        }

        [ActionName("IsValidRecord")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult IsValidRecord(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.IsValidRecord(CustomerId);
            return Ok(result);
        }

        [ActionName("getAddonSelection")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult getAddonSelection(Guid CustomerId, Guid bankid)
        {
            var result = enrollmentbankselectionService.getAddonSelection(CustomerId, bankid);
            return Ok(result);
        }

        [ActionName("getBankFee")]
        [ResponseType(typeof(List<BankFee>))]
        public IHttpActionResult getBankFee(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.getBankFee(CustomerId);
            return Ok(result);
        }

        [ActionName("UpdateAddonforEnrollment")]
        [ResponseType(typeof(UpdateEnrollmentAddonRes))]
        public IHttpActionResult UpdateAddonforEnrollment(UpdateEnrollmentAddon request)
        {
            var result = enrollmentbankselectionService.UpdateAddonforEnrollment(request);
            return Ok(result);
        }

        [ActionName("UpdateAddon")]
        [ResponseType(typeof(bool))]
        public IHttpActionResult UpdateAddon(Guid CustomerId, Guid UserId, Guid BankId)
        {
            var result = enrollmentbankselectionService.UpdateAddon(CustomerId, UserId, BankId);
            return Ok(result);
        }

        [ActionName("getStagingAddon")]
        [ResponseType(typeof(UpdateEnrollmentAddon))]
        public IHttpActionResult getStagingAddon(Guid CustomerId, Guid BankId)
        {
            var result = enrollmentbankselectionService.getStagingAddon(CustomerId, BankId);
            return Ok(result);
        }

        [ActionName("getApprovedBanks")]
        [ResponseType(typeof(List<ApprovedBanks>))]
        public IHttpActionResult getApprovedBanks(Guid CustomerId, Guid BankId)
        {
            var result = enrollmentbankselectionService.getApprovedBanks(CustomerId, BankId);
            return Ok(result);
        }

        [ActionName("getBankEnrollmentStatusofCustomer")]
        [ResponseType(typeof(CustomerBankStaus))]
        public IHttpActionResult getBankEnrollmentStatusofCustomer(Guid CustomerId)
        {
            var result = enrollmentbankselectionService.getBankEnrollmentStatusofCustomer(CustomerId);
            return Ok(result);
        }
    }
}
