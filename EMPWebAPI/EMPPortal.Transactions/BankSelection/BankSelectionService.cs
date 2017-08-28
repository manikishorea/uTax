using EMP.Core.Utilities;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.BankSelection.Dto;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.BankSelection
{
    public class BankSelectionService
    {
        DatabaseEntities db = new DatabaseEntities();

        public CustomerBanksResponse getCutomerBanks(Guid CustomerId, int EntityId)
        {
            CustomerBanksResponse res = new CustomerBanksResponse();
            try
            {
                List<string> bankstatus = new List<string>();
                bankstatus.Add(EMPConstants.Submitted);
                bankstatus.Add(EMPConstants.EnrPending);
                bankstatus.Add(EMPConstants.Approved);
                bankstatus.Add(EMPConstants.Rejected);
                bankstatus.Add(EMPConstants.Denied);

                List<CustomerBanks> banks = new List<Dto.CustomerBanks>();
                var enrollments = (from s in db.BankEnrollments
                                   join b in db.BankMasters on s.BankId equals b.Id
                                   join bs in db.EnrollmentBankSelections on s.BankId equals bs.BankId
                                   where s.CustomerId == CustomerId && s.IsActive == true && bankstatus.Contains(s.StatusCode) && s.ArchiveStatusCode != EMPConstants.Archive
                                   && bs.StatusCode == EMPConstants.Active && b.StatusCode == EMPConstants.Active && bs.CustomerId == CustomerId
                                   select new CustomerBanks
                                   {
                                       BankName = b.BankName,
                                       BankId = b.Id,
                                       EnrollId = s.Id,
                                       Default = (bs.BankSubmissionStatus ?? 0),
                                       BankStatus = s.StatusCode
                                   }).ToList();

                foreach (var bank in enrollments)
                {
                    string submissionstatus = "Submitted", approvedstatus = "";

                    bank.IsSubmitted = true;
                    var date = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.EnrollId && x.Status == EMPConstants.SubmittedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (date != null)
                        submissionstatus = "Submitted - " + date.CreatedDate.Value.ToString("dd MMM yyyy");
                    else
                        submissionstatus = "Submitted";

                    bank.IsApproved = bank.BankStatus == EMPConstants.Approved ? true : false;
                    if (bank.IsApproved)
                    {
                        var date1 = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.EnrollId && x.Status == EMPConstants.ApprovedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        if (date1 != null)
                            approvedstatus = "Approved - " + date1.CreatedDate.Value.ToString("dd MMM yyyy");
                        else
                            approvedstatus = "Approved";
                    }
                    else if(bank.BankStatus == EMPConstants.Rejected || bank.BankStatus == EMPConstants.Denied)
                    {
                        var date1 = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.EnrollId && (x.Status == EMPConstants.RejectedService|| x.Status == EMPConstants.DeniedServcce)).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        if (date1 != null)
                            approvedstatus = "Rejected - " + date1.CreatedDate.Value.ToString("dd MMM yyyy");
                        else
                            approvedstatus = "Rejected";
                    }

                    bank.Acceptance = approvedstatus;
                    bank.BankStatus = bank.Default == 1 ? "Default" : "";
                    bank.Submission = submissionstatus;
                }

                #region Old Scenario

                //if (EntityId == (int)EMPConstants.Entity.SO || EntityId == (int)EMPConstants.Entity.SOME || EntityId == (int)EMPConstants.Entity.SOME_SS)
                //{
                //    var data = (from bank in db.BankMasters
                //                join bankent in db.BankEntityMaps on bank.Id equals bankent.BankId
                //                where (bankent.EntityId == EntityId && bank.StatusCode != EMPConstants.InActive)
                //                select bank).Distinct().ToList();

                //    foreach (var itm in data)
                //    {
                //        CustomerBanks bank = new CustomerBanks();
                //        bank.Acceptance = "";
                //        bank.BankName = itm.BankName;
                //        bank.BankStatus = "";
                //        bank.Submission = "";
                //        bank.BankId = itm.Id;
                //        banks.Add(bank);
                //    }
                //}
                //else
                //{
                //    List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                //    EntityHierarchyDTOs = new DropDowns.DropDownService().GetEntityHierarchies(CustomerId);

                //    Guid TopParentId = Guid.Empty;
                //    if (EntityHierarchyDTOs.Count > 0)
                //    {
                //        var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                //        TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
                //    }

                //    var data = (from bank in db.BankMasters
                //                join config in db.SubSiteBankConfigs on bank.Id equals config.BankMaster_ID
                //                where config.emp_CustomerInformation_ID == TopParentId && bank.StatusCode != EMPConstants.InActive
                //                select new
                //                {
                //                    bank,
                //                }).Distinct().ToList();

                //    foreach (var itm in data)
                //    {
                //        CustomerBanks bank = new CustomerBanks();
                //        bank.Acceptance = "";
                //        bank.BankName = itm.bank.BankName;
                //        bank.BankStatus = "";
                //        bank.Submission = "";
                //        bank.BankId = itm.bank.Id;
                //        banks.Add(bank);
                //    }
                //}

                //foreach (var bank in banks)
                //{
                //    string submissionstatus = "Not Submitted", approvedstatus = "", defaultstatus = "";
                //    var banksubstatus = db.BankEnrollments.Where(x => x.BankId == bank.BankId && x.CustomerId == CustomerId && x.IsActive == true).FirstOrDefault();
                //    if (banksubstatus != null)
                //    {
                //        if (banksubstatus.StatusCode == EMPConstants.Submitted || banksubstatus.StatusCode == EMPConstants.Approved || banksubstatus.StatusCode == EMPConstants.Rejected ||
                //            banksubstatus.StatusCode == EMPConstants.Denied || banksubstatus.StatusCode == EMPConstants.EnrPending)
                //        {
                //            bank.IsSubmitted = true;
                //            var date = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == banksubstatus.Id && x.Status == EMPConstants.SubmittedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                //            if (date != null)
                //                submissionstatus = "Submitted - " + date.CreatedDate.Value.ToString("dd MMM yyyy");
                //            else
                //                submissionstatus = "Submitted";
                //        }

                //        if (banksubstatus.StatusCode == EMPConstants.Approved)
                //        {
                //            bank.IsApproved = true;
                //            var date = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == banksubstatus.Id && x.Status == EMPConstants.ApprovedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                //            if (date != null)
                //                approvedstatus = "Approved - " + date.CreatedDate.Value.ToString("dd MMM yyyy");
                //            else
                //                approvedstatus = "Approved";
                //        }
                //        else if (banksubstatus.StatusCode == EMPConstants.Submitted)
                //            approvedstatus = "Pending";
                //    }

                //    var banksel = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.BankId == bank.BankId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                //    if (banksel != null)
                //    {
                //        if (banksel.BankSubmissionStatus.HasValue)
                //        {
                //            defaultstatus = banksel.BankSubmissionStatus.Value == 1 ? "Default" : "";
                //            bank.IsDefault = banksel.BankSubmissionStatus.Value == 1 ? true : false;
                //        }
                //        else
                //            bank.IsDefault = false;
                //    }
                //    else
                //        bank.IsDefault = false;

                //    bank.Acceptance = approvedstatus;
                //    bank.BankStatus = defaultstatus;
                //    bank.Submission = submissionstatus;
                //}

                #endregion

                res.Banks = enrollments;
                res.Status = true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "BankSelectionService/getCutomerBanks", CustomerId);
                res.Status = false;
            }
            return res;
        }

        public bool setDefaultBank(Guid CustomerId, Guid UserId, Guid BankId)
        {

            DropDownService ddService = new DropDownService();
            List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
            EntityHierarchyDTOs = ddService.GetEntityHierarchies(UserId);

            Guid TopParentId = Guid.Empty;

            if (EntityHierarchyDTOs.Count > 0)
            {
                var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
            }

            var result = db.SetDefaultBankSP(CustomerId.ToString(), UserId.ToString(), BankId.ToString());
            var res1 = db.OfficeManagementGridSP(CustomerId.ToString(), CustomerId.ToString(), TopParentId == Guid.Empty ? null : TopParentId.ToString());
            return true;
        }

        public bool CheckUnlock(Guid CustomerId)
        {
            try
            {
                var isunlock = db.CustomerUnlocks.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                if (isunlock == null)
                    return false;
                else
                    return isunlock.IsUnlocked;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "BankSelectionService/CheckUnlock", CustomerId);
                return false;
            }
        }

        public ActivityBankStatusModel getActivityBankStatus(Guid CustomerId)
        {
            ActivityBankStatusModel ActivityBankStatus = new ActivityBankStatusModel();

            db = new DatabaseEntities();
            var result = db.EnrollmentBankSelections.Where(o => o.CustomerId == CustomerId && o.StatusCode == EMPConstants.Active).OrderByDescending(o => o.BankSubmissionStatus).ThenByDescending(o => o.LastUpdatedDate).Take(2).ToList();

            foreach (var item in result)
            {
                if (result.Where(o => o.BankSubmissionStatus == 1).Count() > 0 && result.Count > 1)
                {
                    if (result.Where(o => o.BankSubmissionStatus == 1).Select(o => o.LastUpdatedDate).FirstOrDefault() < result.Where(o => o.BankSubmissionStatus == 0).Select(o => o.LastUpdatedDate).FirstOrDefault())
                    {
                        var quer = (from res in result
                                    join bnk in db.BankMasters on res.BankId equals bnk.Id
                                    select new { bankname = bnk.BankName, DefaultStatus = res.BankSubmissionStatus ?? 0 }).ToList();

                        foreach (var bankitem in quer)
                        {
                            if (bankitem.DefaultStatus == 1)
                            {
                                ActivityBankStatus.ActiveBank = bankitem.bankname;
                            }
                            else
                            {
                                ActivityBankStatus.EditingBank = bankitem.bankname + " (" + item.LastUpdatedDate + ")";
                            }
                        }
                    }
                }
            }

            return ActivityBankStatus;
        }

        public List<Guid> getOtherBankStatus(Guid CustomerId, Guid BankId)
        {
            List<Guid> _banks = new List<Guid>();
            try
            {
                var banks = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.BankId != BankId && x.StatusCode == EMPConstants.Active).ToList();
                foreach (var bank in banks)
                {
                    var status = db.BankEnrollments.Where(x => x.BankId == bank.BankId && x.CustomerId == CustomerId && x.IsActive == true).FirstOrDefault();
                    if (status != null)
                    {
                        if (status.StatusCode == EMPConstants.Approved && bank.BankSubmissionStatus != 1)
                        {
                            _banks.Add(bank.BankId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "BankSelectionService/getOtherBankStatus", CustomerId);
            }
            return _banks;
        }
    }

    public class ActivityBankStatusModel
    {
        public string ActiveBank { get; set; }
        public string EditingBank { get; set; }
    }

}
