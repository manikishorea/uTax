using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.EnrollmentBankSelectionInfo.DTO;
using EMPEntityFramework.Edmx;
using System.Globalization;
using System.Data.SqlTypes;
using EMP.Core.Utilities;
using EMPPortal.Transactions.CrosslinkService;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using EMPPortal.Transactions.DropDowns.DTO;
using EMPPortal.Transactions.DropDowns;
using MoreLinq;

namespace EMPPortal.Transactions.EnrollmentBankSelectionInfo
{
    public class EnrollmentBankSelectionService
    {
        DatabaseEntities db = new DatabaseEntities();
        CrosslinkWS17SoapClient _apiObj = new CrosslinkWS17SoapClient();

        public IQueryable<EnrollmentBankSelectionDTO> GetBankandFeesInfo(int entityid, Guid userid, string ParentId)
        {
            try
            {
                db = new DatabaseEntities();

                var data1 = (from fee in db.FeeMasters
                             join feeent in db.FeeEntityMaps on fee.Id equals feeent.FeeId
                             where feeent.EntityId == entityid && fee.FeeTypeId != 2
                             select new { fee, feeent });

                var data = (from fee in db.FeeMasters
                            join feeent in db.FeeEntityMaps on fee.Id equals feeent.FeeId
                            join cust in db.CustomerAssociatedFees on fee.Id equals cust.FeeMaster_ID
                            where feeent.EntityId == entityid && fee.FeeTypeId == 2 && cust.emp_CustomerInformation_ID == userid
                            select new { fee.FeesFor, cust.Amount }).ToList();

                var dataa = (from s in data
                             group s by s.FeesFor into g
                             select new
                             {
                                 feesfor = g.Key,
                                 sum = g.Sum(x => x.Amount)
                             }).ToList();

                List<EnrollmentBankSelectionDTO> ldtlist = new List<EnrollmentBankSelectionDTO>();
                foreach (var itm in data1.ToList())
                {
                    EnrollmentBankSelectionDTO odt = new EnrollmentBankSelectionDTO();
                    if (itm.fee.FeesFor == (int)EMP.Core.Utilities.EMPConstants.FeesFor.Others)
                    {
                        if (itm.fee.FeeTypeId == 1)
                            odt.Name = itm.fee.Name; //EMP.Core.Utilities.EMPConstants.FeesFor.Others.ToString();
                        else
                            continue;
                    }
                    else if (itm.fee.FeesFor == (int)EMP.Core.Utilities.EMPConstants.FeesFor.SVBFees)
                        odt.Name = "Service Bureau Fees";
                    else
                        odt.Name = "Transmission Fees"; //EMP.Core.Utilities.EMPConstants.FeesFor.TransmissionFees.ToString();
                    decimal idatval = dataa.Where(a => a.feesfor == itm.fee.FeesFor).Select(a => a.sum).FirstOrDefault();

                    odt.FeeFor = itm.fee.FeesFor ?? 0;
                    odt.Amount = (decimal)itm.fee.Amount + idatval;
                    odt.FeeCategoryID = 1;
                    ldtlist.Add(odt);
                }

                foreach (var itm in db.BankMasters) //.Where(o => o.Id == bankid))
                {
                    EnrollmentBankSelectionDTO odt = new EnrollmentBankSelectionDTO();
                    odt.Name = itm.BankName;
                    odt.Amount = itm.BankServiceFees ?? 0;
                    odt.FeeCategoryID = 0;
                    odt.Id = itm.Id.ToString();
                    ldtlist.Add(odt);
                }

                return ldtlist.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetBankandFeesInfo", userid);
                return new List<EnrollmentBankSelectionDTO>().AsQueryable();
            }
        }

        public List<BankFee> getBankFee(Guid CustomerId)
        {
            try
            {
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (customer != null)
                {
                    DropDownService ddService = new DropDownService();
                    List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                    EntityHierarchyDTOs = ddService.GetEntityHierarchies(CustomerId);
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    Guid TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                    bool isMso = false;
                    if (CustomerId != TopParentId)
                    {
                        var parentcustomer = db.emp_CustomerInformation.Where(x => x.Id == TopParentId).FirstOrDefault();
                        isMso = parentcustomer.IsMSOUser ?? false;
                    }
                    else
                    {
                        return new List<DTO.BankFee>();
                    }

                    if (isMso)
                    {
                        var bankfees = (from s in db.SubSiteBankFeesConfigs
                                        where s.emp_CustomerInformation_ID == CustomerId //&& s.BankMaster_ID == BankId
                                        group s by s.BankMaster_ID into g
                                        select new
                                        {
                                            BankId = g.Key.ToString(),
                                            SvbAmount = g.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees_MSO).FirstOrDefault(),
                                            TransAmount = g.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees_MSO).FirstOrDefault()
                                        }).ToList();
                        var list = (from s in bankfees
                                    select new BankFee
                                    {
                                        BankId = s.BankId,
                                        SvbAmount = s.SvbAmount ?? 0,
                                        TransAmount = s.TransAmount ?? 0
                                    }).ToList();
                        return list;
                    }
                    else
                    {
                        var bankfees = (from s in db.SubSiteBankFeesConfigs
                                        where s.emp_CustomerInformation_ID == CustomerId //&& s.BankMaster_ID == BankId
                                        group s by s.BankMaster_ID into g
                                        select new BankFee
                                        {
                                            BankId = g.Key.ToString(),
                                            SvbAmount = g.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees).FirstOrDefault(),
                                            TransAmount = g.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees).FirstOrDefault()
                                        }).ToList();
                        return bankfees;
                    }
                }
                else
                    return new List<DTO.BankFee>();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getBankFee", CustomerId);
                return new List<BankFee>();
            }
        }

        public List<EnrollmentBankSelectionDTO> BankFee(Guid ParentId, Guid Id)
        {
            try
            {
                List<EnrollmentBankSelectionDTO> EnrollmentBankSelectionDTOList = new List<EnrollmentBankSelectionDTO>();

                var SubSiteFeeConfig = db.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == ParentId).ToList();

                foreach (var item in SubSiteFeeConfig)
                {
                    EnrollmentBankSelectionDTO CustomerAssociatedFees = new EnrollmentBankSelectionDTO();

                    List<SubSiteBankFeesConfig> SubSiteBankFeeConfigList = new List<SubSiteBankFeesConfig>();

                    SubSiteBankFeeConfigList = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == Id && o.SubSiteFeeConfig_ID == item.ID).ToList();

                    foreach (var BankFees in SubSiteBankFeeConfigList)
                    {
                        CustomerAssociatedFees.Amount = BankFees.BankMaxFees;
                    }

                    if (item.IsSameforAll && CustomerAssociatedFees.Amount == 0)
                    {
                        CustomerAssociatedFees.AmountStatus = "No Add-On";
                    }
                    else if (item.IsAddOnFeeCharge == false)
                    {
                        CustomerAssociatedFees.AmountStatus = "No Add-On";
                    }
                    else
                    {
                        CustomerAssociatedFees.Amount = CustomerAssociatedFees.Amount;
                        CustomerAssociatedFees.AmountStatus = CustomerAssociatedFees.Amount.ToString();
                    }
                    CustomerAssociatedFees.FeeFor = item.ServiceorTransmission == 1 ? (int)EMPConstants.FeesFor.SVBFees : (int)EMPConstants.FeesFor.TransmissionFees;
                    CustomerAssociatedFees.Name = item.ServiceorTransmission == 1 ? "Service Bureau Fee - Add on " : "Transmission Fee- Add on";
                    // CustomerAssociatedFeesDTOList.Add(CustomerAssociatedFees);

                    CustomerAssociatedFees.FeeCategoryID = 1;

                    EnrollmentBankSelectionDTOList.Add(CustomerAssociatedFees);
                }
                return EnrollmentBankSelectionDTOList;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/BankFee", Id);
                return null;
            }
        }

        public bool EnrollmentBankSelectSave(EnrollmentBankSelectionDTO dto)
        {
            int entityState = 0;
            Guid oldBank = Guid.Empty;
            decimal addonfee = 0, svbfee = 0;

            EnrollmentBankSelection enrollmentbankselectionInfo = new EnrollmentBankSelection();
            if (dto != null)
            {
                enrollmentbankselectionInfo = db.EnrollmentBankSelections.Where(a => a.CustomerId == dto.CustomerId && a.StatusCode == EMPConstants.Active && a.BankId == dto.BankId).FirstOrDefault();
                if (enrollmentbankselectionInfo != null)
                {
                    addonfee = enrollmentbankselectionInfo.IsTransmissionFee.Value ? enrollmentbankselectionInfo.TransmissionBankAmount.Value : 0;
                    svbfee = enrollmentbankselectionInfo.IsServiceBureauFee.Value ? enrollmentbankselectionInfo.ServiceBureauBankAmount.Value : 0;
                    oldBank = enrollmentbankselectionInfo.BankId;
                    entityState = (int)System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    enrollmentbankselectionInfo = new EnrollmentBankSelection();
                    enrollmentbankselectionInfo.Id = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }

                enrollmentbankselectionInfo.CustomerId = dto.CustomerId;
                enrollmentbankselectionInfo.BankId = dto.BankId;
                enrollmentbankselectionInfo.QuestionId = dto.QuestionId;
                enrollmentbankselectionInfo.IsServiceBureauFee = dto.IsServiceBureauFee;
                enrollmentbankselectionInfo.ServiceBureauBankAmount = dto.ServiceBureauBankAmount;
                enrollmentbankselectionInfo.IsTransmissionFee = dto.IsTransmissionFee;
                enrollmentbankselectionInfo.TransmissionBankAmount = dto.TransmissionBankAmount;
                enrollmentbankselectionInfo.StatusCode = EMPConstants.Active;
                enrollmentbankselectionInfo.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                enrollmentbankselectionInfo.LastUpdatedDate = System.DateTime.Now;
                enrollmentbankselectionInfo.BankSubmissionStatus = 0;
                enrollmentbankselectionInfo.IsPreferredBank = false;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    enrollmentbankselectionInfo.CreatedBy = dto.UserId ?? Guid.Empty;
                    enrollmentbankselectionInfo.CreatedDate = System.DateTime.Now;
                    db.EnrollmentBankSelections.Add(enrollmentbankselectionInfo);
                }
                else
                {
                    db.Entry(enrollmentbankselectionInfo).State = System.Data.Entity.EntityState.Modified;
                }

                if (oldBank != Guid.Empty)
                {
                    var bankcode = db.BankMasters.Where(x => x.Id == oldBank).Select(x => x.BankCode).FirstOrDefault();
                    //if (oldBank != dto.BankId)
                    //{
                    //    var enrolldata = db.BankEnrollments.Where(x => x.CustomerId == dto.CustomerId && x.IsActive == true).FirstOrDefault();
                    //    if (enrolldata != null)
                    //    {
                    //        enrolldata.IsActive = false;
                    //        enrolldata.UpdatedBy = dto.UserId;
                    //        enrolldata.UpdatedDate = DateTime.Now;
                    //    }

                    //    if (bankcode == EMPConstants.TPGBank)
                    //    {
                    //        var bankenroll = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == dto.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    //        if (bankenroll != null)
                    //        {
                    //            bankenroll.StatusCode = EMPConstants.InActive;
                    //        }
                    //    }
                    //    else if (bankcode == EMPConstants.RBBank)
                    //    {
                    //        var bankenroll = db.BankEnrollmentForRBs.Where(x => x.CustomerId == dto.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    //        if (bankenroll != null)
                    //        {
                    //            bankenroll.StatusCode = EMPConstants.InActive;
                    //        }
                    //    }
                    //    else if (bankcode == EMPConstants.RABank)
                    //    {
                    //        var bankenroll = db.BankEnrollmentForRAs.Where(x => x.CustomerId == dto.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    //        if (bankenroll != null)
                    //        {
                    //            bankenroll.StatusCode = EMPConstants.InActive;
                    //        }
                    //    }

                    //    var enrollguid = new Guid("0FEEB0FE-D0E7-4370-8733-DD5F7D2041FC");
                    //    var menucheck = db.CustomerConfigurationStatus.Where(x => x.CustomerId == dto.CustomerId && x.SitemapId == enrollguid).ToList();
                    //    if (menucheck.Count > 0)
                    //        db.CustomerConfigurationStatus.RemoveRange(menucheck);
                    //}
                    //else
                    //{
                    if (bankcode == EMPConstants.TPGBank)
                    {
                        var bankenroll = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == dto.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (bankenroll != null)
                        {
                            if (dto.IsTransmissionFee && addonfee < dto.TransmissionBankAmount)
                                bankenroll.AddonFee = (Convert.ToDecimal(bankenroll.AddonFee) + (dto.TransmissionBankAmount - addonfee)).ToString();
                            else if (dto.IsTransmissionFee && addonfee > dto.TransmissionBankAmount)
                                bankenroll.AddonFee = (Convert.ToDecimal(bankenroll.AddonFee) + (dto.TransmissionBankAmount - addonfee)).ToString();
                            else if (addonfee != dto.TransmissionBankAmount)
                                bankenroll.AddonFee = (Convert.ToDecimal(bankenroll.AddonFee) - addonfee).ToString();

                            if (dto.IsServiceBureauFee && svbfee < dto.ServiceBureauBankAmount)
                                bankenroll.ServiceBureauFee = (Convert.ToDecimal(bankenroll.ServiceBureauFee) + (dto.ServiceBureauBankAmount - svbfee)).ToString();
                            else if (dto.IsServiceBureauFee && svbfee > dto.ServiceBureauBankAmount)
                                bankenroll.ServiceBureauFee = (Convert.ToDecimal(bankenroll.ServiceBureauFee) + (dto.ServiceBureauBankAmount - svbfee)).ToString();
                            else if (svbfee != dto.ServiceBureauBankAmount)
                                bankenroll.ServiceBureauFee = (Convert.ToDecimal(bankenroll.ServiceBureauFee) - svbfee).ToString();
                        }
                    }
                    else if (bankcode == EMPConstants.RBBank)
                    {
                        var bankenroll = db.BankEnrollmentForRBs.Where(x => x.CustomerId == dto.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (bankenroll != null)
                        {
                            if (dto.IsTransmissionFee && addonfee < dto.TransmissionBankAmount)
                                bankenroll.TransimissionAddon = (Convert.ToDecimal(bankenroll.TransimissionAddon) + (dto.TransmissionBankAmount - addonfee)).ToString();
                            else if (dto.IsTransmissionFee && addonfee > dto.TransmissionBankAmount)
                                bankenroll.TransimissionAddon = (Convert.ToDecimal(bankenroll.TransimissionAddon) + (dto.TransmissionBankAmount - addonfee)).ToString();
                            else if (addonfee != dto.TransmissionBankAmount)
                                bankenroll.TransimissionAddon = (Convert.ToDecimal(bankenroll.TransimissionAddon) - addonfee).ToString();

                            if (dto.IsServiceBureauFee && svbfee < dto.ServiceBureauBankAmount)
                                bankenroll.SBFee = (Convert.ToDecimal(bankenroll.SBFee) + (dto.ServiceBureauBankAmount - svbfee)).ToString();
                            else if (dto.IsServiceBureauFee && svbfee > dto.ServiceBureauBankAmount)
                                bankenroll.SBFee = (Convert.ToDecimal(bankenroll.SBFee) + (dto.ServiceBureauBankAmount - svbfee)).ToString();
                            else if (svbfee != dto.ServiceBureauBankAmount)
                                bankenroll.SBFee = (Convert.ToDecimal(bankenroll.SBFee) - svbfee).ToString();
                        }
                    }
                    else if (bankcode == EMPConstants.RABank)
                    {
                        var bankenroll = db.BankEnrollmentForRAs.Where(x => x.CustomerId == dto.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (bankenroll != null)
                        {
                            if (dto.IsTransmissionFee && addonfee < dto.TransmissionBankAmount)
                                bankenroll.TransmissionAddon = (Convert.ToDecimal(bankenroll.TransmissionAddon) + (dto.TransmissionBankAmount - addonfee)).ToString();
                            else if (dto.IsTransmissionFee && addonfee > dto.TransmissionBankAmount)
                                bankenroll.TransmissionAddon = (Convert.ToDecimal(bankenroll.TransmissionAddon) + (dto.TransmissionBankAmount - addonfee)).ToString();
                            else if (addonfee != dto.TransmissionBankAmount)
                                bankenroll.TransmissionAddon = (Convert.ToDecimal(bankenroll.TransmissionAddon) - addonfee).ToString();

                            if (dto.IsServiceBureauFee && svbfee < dto.ServiceBureauBankAmount)
                                bankenroll.SbFee = (Convert.ToDecimal(bankenroll.SbFee) + (dto.ServiceBureauBankAmount - svbfee)).ToString();
                            else if (dto.IsServiceBureauFee && svbfee > dto.ServiceBureauBankAmount)
                                bankenroll.SbFee = (Convert.ToDecimal(bankenroll.SbFee) + (dto.ServiceBureauBankAmount - svbfee)).ToString();
                            else if (svbfee != dto.ServiceBureauBankAmount)
                                bankenroll.SbFee = (Convert.ToDecimal(bankenroll.SbFee) - svbfee).ToString();
                        }
                    }
                    //}
                }

            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/EnrollmentBankSelectSave", Guid.Empty);
                return false;
            }
        }

        public IQueryable<EnrollmentBankSelectionDTO> GetEnrollmentBankSelection(Guid userid, Guid Parentid, bool IsStaging, Guid bankid)
        {
            try
            {
                db = new DatabaseEntities();
                List<EnrollmentBankSelectionDTO> enrollmentbankSelection = new List<EnrollmentBankSelectionDTO>();
                EnrollmentBankSelection itm = new EnrollmentBankSelection();

                if (bankid == Guid.Empty)
                {
                    itm = (from rdb in db.EnrollmentBankSelections where rdb.CustomerId == userid && rdb.StatusCode == EMPConstants.Active orderby rdb.BankSubmissionStatus descending, rdb.LastUpdatedDate descending select rdb).FirstOrDefault();//&& rdb.BankId == bankid

                    if (itm != null)
                    {
                        bankid = itm.BankId;
                    }
                }
                else
                {
                    itm = (from rdb in db.EnrollmentBankSelections where rdb.CustomerId == userid && rdb.StatusCode == EMPConstants.Active && rdb.BankId == bankid select rdb).FirstOrDefault();//&& rdb.BankId == bankid
                    if (itm != null)
                    {
                        bankid = itm.BankId;
                    }
                }

                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(userid);

                Guid TopParentId = Guid.Empty;
                int entityid = 0;

                if (EntityHierarchyDTOs.Count > 0)
                {
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
                    entityid = TopFromHierarchy.EntityId ?? 0;
                }


                if (entityid != (int)EMPConstants.Entity.SO && entityid != (int)EMPConstants.Entity.SOME && entityid != (int)EMPConstants.Entity.SOME_SS)
                {
                    var SSBConfig = db.SubSiteBankConfigs.Where(o => o.emp_CustomerInformation_ID == TopParentId && o.BankMaster_ID == bankid).ToList();
                    if (SSBConfig.Count() == 0)
                    {
                        bankid = Guid.Empty;
                    }
                }

                //foreach (var itm in data)
                if (bankid != Guid.Empty)
                {
                    if (itm != null)
                    {
                        EnrollmentBankSelectionDTO enrollmentModel = new EnrollmentBankSelectionDTO();
                        enrollmentModel.Id = itm.Id.ToString();
                        enrollmentModel.CustomerId = itm.CustomerId;
                        enrollmentModel.BankId = itm.BankId;
                        enrollmentModel.QuestionId = itm.QuestionId;
                        enrollmentModel.IsServiceBureauFee = itm.IsServiceBureauFee ?? false;
                        enrollmentModel.ServiceBureauBankAmount = itm.ServiceBureauBankAmount ?? 0;
                        enrollmentModel.IsTransmissionFee = itm.IsTransmissionFee ?? false;
                        enrollmentModel.TransmissionBankAmount = itm.TransmissionBankAmount ?? 0;
                        enrollmentModel.StatusCode = itm.StatusCode;

                        Guid bankmstid = new Guid("A29B3547-8954-4036-9BD3-312F1D6A3DAA");

                        var Options = (from ssb in db.SubSiteBankConfigs
                                       join bs in db.BankSubQuestions on ssb.SubQuestion_ID equals bs.Id
                                       where ssb.emp_CustomerInformation_ID == Parentid && ssb.BankMaster_ID == bankmstid
                                       select bs.Options).FirstOrDefault();
                        if (Options != null)
                            enrollmentModel.TPGOptions = Options ?? 0;
                        else
                        {
                            enrollmentModel.TPGOptions = 0;
                        }


                        var SsFConfig = (from ssf in db.SubSiteFeeConfigs where ssf.emp_CustomerInformation_ID == Parentid select new { ssf.IsSubSiteAddonFee, ssf.ServiceorTransmission });
                        if (SsFConfig != null && Parentid != Guid.Empty)
                        {
                            foreach (var itms in SsFConfig)
                            {
                                if (itms.ServiceorTransmission == 1)
                                    enrollmentModel.IsDVServiceBureauFee = itms.IsSubSiteAddonFee;
                                if (itms.ServiceorTransmission == 2)
                                    enrollmentModel.IsDVTransmissionFee = itms.IsSubSiteAddonFee;
                            }
                        }
                        else
                        {
                            enrollmentModel.IsDVServiceBureauFee = true;
                            enrollmentModel.IsDVTransmissionFee = true;
                        }

                        if (IsStaging)
                        {
                            var stagingvalue = db.EnrollmentAddonStagings.Where(x => x.CustomerId == userid && x.IsActive == true && x.BankId == enrollmentModel.BankId).FirstOrDefault();
                            if (stagingvalue != null)
                            {
                                enrollmentModel.IsServiceBureauFee = stagingvalue.IsSvbFee;
                                enrollmentModel.ServiceBureauBankAmount = stagingvalue.SvbAddonAmount;
                                enrollmentModel.IsTransmissionFee = stagingvalue.IsTransmissionFee;
                                enrollmentModel.TransmissionBankAmount = stagingvalue.TransmissionAddonAmount;
                            }
                        }
                        enrollmentModel.IsAvailable = 1;
                        enrollmentbankSelection.Add(enrollmentModel);
                    }
                    else
                    {
                        EnrollmentBankSelectionDTO enrollmentModel = new EnrollmentBankSelectionDTO();
                        enrollmentModel.IsAvailable = 2;
                        enrollmentbankSelection.Add(enrollmentModel);
                    }
                }
                else
                {
                    EnrollmentBankSelectionDTO enrollmentModel = new EnrollmentBankSelectionDTO();
                    enrollmentModel.IsAvailable = 0;
                    enrollmentbankSelection.Add(enrollmentModel);
                }

                return enrollmentbankSelection.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetEnrollmentBankSelection", userid);
                return null;
            }
        }

        public string GetBankSelectedByCustomer(Guid CustomerId, Guid bankid)
        {
            try
            {
                var bank = (from s in db.EnrollmentBankSelections
                            join b in db.BankMasters on s.BankId equals b.Id
                            where s.CustomerId == CustomerId && s.StatusCode == EMPConstants.Active && b.Id == bankid
                            select b.BankCode).FirstOrDefault();
                return bank;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetBankSelectedByCustomer", CustomerId);
                return "";
            }
        }

        public void getxlinkCredentials(ref string MasterId, ref string Password, ref int CrossLinkUserId, string xUser, string CLAccountId, string ClPassword, string Login, string MasterIdentifier, Guid ParentId, int EntityId)
        {
            try
            {
                if (string.IsNullOrEmpty(CLAccountId))
                {
                    bool IsMso = false;
                    Guid TopParentId = ParentId;
                    if (ParentId != null && ParentId != Guid.Empty)
                    {
                        DropDownService ddService = new DropDownService();
                        List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                        EntityHierarchyDTOs = ddService.GetEntityHierarchies(ParentId);

                        if (EntityHierarchyDTOs.Count > 0)
                        {
                            var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                            TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
                        }

                        var parentmso = db.emp_CustomerInformation.Where(x => x.Id == TopParentId).FirstOrDefault();
                        if (parentmso.IsMSOUser ?? false)
                            IsMso = true;
                    }

                    if (IsMso)
                    {
                        var parent = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == TopParentId).FirstOrDefault();
                        if (parent != null)
                        {
                            MasterIdentifier = parent.MasterIdentifier;
                            CrossLinkUserId = Convert.ToInt32(parent.CrossLinkUserId);
                            //Password = PasswordManager.DecryptText(parent.CrossLinkPassword);                        
                        }
                    }
                    else if (EntityId == (int)EMPConstants.Entity.SVB_MO_AE_SS || EntityId == (int)EMPConstants.Entity.SVB_AE_SS || EntityId == (int)EMPConstants.Entity.MO_AE_SS || EntityId == (int)EMPConstants.Entity.SOME_SS)
                    {
                        var parent = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == ParentId).FirstOrDefault();
                        if (parent != null)
                        {
                            MasterIdentifier = parent.MasterIdentifier;
                            CrossLinkUserId = Convert.ToInt32(parent.CrossLinkUserId);
                            //Password = PasswordManager.DecryptText(parent.CrossLinkPassword);                        
                        }
                    }
                    else
                        CrossLinkUserId = Convert.ToInt32(xUser);

                    var cred = db.UtaxCrosslinkDetails.Where(x => x.CLAccountId == MasterIdentifier && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (cred != null)
                    {
                        MasterId = cred.CLLogin;
                        Password = PasswordManager.DecryptText(cred.CLAccountPassword);
                    }
                    else
                    {
                        MasterId = EMPConstants.xlinkMaster;
                        Password = EMPConstants.xlinkPassword;
                    }
                }
                else
                {
                    MasterId = Login;
                    Password = PasswordManager.DecryptText(ClPassword);
                    CrossLinkUserId = Convert.ToInt32(xUser);
                }

            }
            catch (Exception ex)
            {

            }
        }

        public XlinkResponseModel UpdateBankEfinObject(EfinObject objefin, Guid BankId, Guid EnrollmentId, Guid CustomerId)
        {
            try
            {
                var customer = (from c in db.emp_CustomerInformation
                                join l in db.emp_CustomerLoginInformation on c.Id equals l.CustomerOfficeId
                                where c.Id == CustomerId
                                select new { c.EFIN, l.CrossLinkUserId, l.CrossLinkPassword, l.MasterIdentifier, c.ParentId, c.EntityId, l.CLAccountId, l.CLAccountPassword, l.CLLogin }).FirstOrDefault();

                string MasterId = ""; //customer.MasterIdentifier;
                string Password = ""; //PasswordManager.DecryptText(customer.CrossLinkPassword);
                int CrossLinkUserId = 0;

                getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, customer.CrossLinkUserId, customer.CLAccountId, customer.CLAccountPassword, customer.CLLogin, customer.MasterIdentifier, customer.ParentId ?? Guid.Empty, customer.EntityId ?? 0);

                int efin = customer.EFIN.HasValue ? customer.EFIN.Value : 0;
                string ParentId = customer.ParentId.HasValue ? customer.ParentId.Value.ToString() : "";

                string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                if (_accesskey != "")
                {
                    AuthObject _objAuth = new AuthObject();
                    _objAuth.AccessKey = _accesskey;
                    _objAuth.UserID = customer.MasterIdentifier;

                    // checking the Authentication
                    XlinkResponse isAuth = _apiObj.isAuth(_objAuth);
                    if (isAuth.success)
                    {
                        var efinobj = _apiObj.getEFINbyEFIN(_objAuth, efin, MasterId, 0);
                        if (efinobj.Locked ?? false)
                        {
                            List<string> error = new List<string>();
                            error.Add("EFIN is locked");
                            return new XlinkResponseModel() { Status = false, Messages = error };
                        }

                        objefin.AccountID = MasterId;
                        objefin.CreatedBy = MasterId;
                        objefin.Efin = efin;
                        objefin.EfinID = efinobj.EfinID;
                        objefin.UpdatedBy = MasterId;
                        objefin.UserID = CrossLinkUserId;

                        string xml = getEfinObjecyXml(objefin);
                        XlinkResponse isValid = _apiObj.validateEfinObject(objefin, false);
                        if (!isValid.success)
                        {
                            if (isValid.message != null)
                            {
                                SaveEnrollmentHistory("EFIN Update failed :: " + string.Join(",", isValid.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml);
                            }
                            return new XlinkResponseModel() { IsXlink = true, Messages = isValid.message.ToList(), Status = isValid.success };
                        }
                        XlinkResponse res = _apiObj.updateEFIN(_objAuth, objefin, MasterId, 0, false);
                        if (!res.success)
                        {
                            if (res.message != null)
                            {
                                if (res.message.Count() > 0)
                                    SaveEnrollmentHistory("EFIN Update failed :: " + string.Join(",", res.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml);
                                else
                                    SaveEnrollmentHistory("EFIN Update failed", CustomerId, 0, BankId, EnrollmentId, xml);
                            }
                            return new XlinkResponseModel() { IsXlink = true, Messages = res.message.ToList(), Status = res.success };
                        }
                        else
                            SaveEnrollmentHistory("EFIN Updated ", CustomerId, 1, BankId, EnrollmentId, xml);
                        return new XlinkResponseModel() { IsXlink = true, Messages = res.message.ToList(), Status = res.success };
                    }
                    return new XlinkResponseModel() { Messages = new List<string>(), Status = false };
                }
                return new XlinkResponseModel() { Messages = new List<string>(), Status = false };
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/UpdateEfinObject", CustomerId);
                return new XlinkResponseModel() { IsXlink = false, Messages = new List<string>(), Status = false };
            }
        }

        public XlinkResponseModel SaveTPGBankEnrollment(TPGBankEnrollment enrollment)
        {
            try
            {
                var isExist = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    var enrollstatus = db.BankEnrollments.Where(x => x.CustomerId == enrollment.CustomerId && x.IsActive == true && x.BankId == enrollment.BankId).FirstOrDefault();
                    if (enrollstatus != null)
                    {
                        if (enrollstatus.StatusCode == EMPConstants.Approved)
                        {
                            EfinObject _objefin = new CrosslinkService.EfinObject();

                            _objefin.AcctName = enrollment.AccountName;
                            _objefin.AcctType = enrollment.AccountType;
                            _objefin.Address = enrollment.OwnerAddress;
                            _objefin.AgreePEIDate = DateTime.Now;
                            _objefin.AgreePEITerms = true;
                            _objefin.AgreeFeeOption = true;
                            _objefin.BankName = enrollment.BankName;
                            _objefin.City = enrollment.OwnerCity;
                            _objefin.Company = enrollment.CompanyName;
                            _objefin.CreatedDate = DateTime.Now;
                            _objefin.DAN = enrollment.OfficeDAN;
                            _objefin.DOB = Convert.ToDateTime(enrollment.OwnerDOB);
                            _objefin.EFINType = "S";
                            _objefin.EIN = enrollment.OwnerEIN;
                            _objefin.Email = enrollment.OwnerEmail;
                            _objefin.FivePlus = false;
                            _objefin.FName = enrollment.OwnerFirstName;
                            _objefin.IDNumber = enrollment.EfinIDNumber;
                            _objefin.IDState = enrollment.EfinIdState;
                            _objefin.LName = enrollment.OwnerLastName;
                            _objefin.Mobile = string.IsNullOrEmpty(enrollment.EfinOwnerMobile) ? "" : enrollment.EfinOwnerMobile.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            _objefin.Phone = enrollment.OwnerTelephone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            _objefin.RTN = enrollment.OfficeRTN;
                            _objefin.SBFeeAll = enrollment.SbfeeAll;
                            _objefin.SelectedBank = EMPConstants.TPGBank;
                            _objefin.SSN = enrollment.OwnerSSn;
                            _objefin.State = enrollment.OwnerState;
                            _objefin.Title = enrollment.EfinOwnerTitle;
                            _objefin.UpdatedDate = DateTime.Now;
                            _objefin.Zip = enrollment.OwnerZip;

                            var res = UpdateBankEfinObject(_objefin, enrollstatus.BankId.Value, enrollstatus.Id, enrollment.CustomerId);
                            if (!res.Status)
                            {
                                return res;
                            }

                            isExist.IsUpdated = true;
                        }
                    }

                    isExist.BankUsedLastYear = enrollment.LastYearBank;
                    isExist.CompanyName = enrollment.CompanyName;
                    isExist.EFINOwnerAddress = enrollment.OwnerAddress;
                    isExist.EFINOwnerCity = enrollment.OwnerCity;
                    isExist.EFINOwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.EFINOwnerEIN = enrollment.OwnerEIN;
                    isExist.EFINOwnerEmail = enrollment.OwnerEmail;
                    isExist.EFINOwnerFirstName = enrollment.OwnerFirstName;
                    isExist.EFINOwnerLastName = enrollment.OwnerLastName;
                    isExist.EFINOwnerSSN = enrollment.OwnerSSn;
                    isExist.EFINOwnerState = enrollment.OwnerState;
                    isExist.EFINOwnerTelephone = enrollment.OwnerTelephone;
                    isExist.EFINOwnerZip = enrollment.OwnerZip;
                    isExist.ManagerEmail = enrollment.ManagerEmail;
                    isExist.ManagerFirstName = enrollment.ManagerFirstName;
                    isExist.ManagerLastName = enrollment.ManagerLastName;
                    isExist.OfficeAccountType = enrollment.AccountType;
                    isExist.OfficeAddress = enrollment.OfficeAddress;
                    isExist.OfficeCity = enrollment.OfficeCity;
                    isExist.OfficeDAN = enrollment.OfficeDAN;
                    isExist.OfficeFAX = enrollment.OfficeFax;
                    isExist.OfficeRTN = enrollment.OfficeRTN;
                    isExist.OfficeState = enrollment.OfficeState;
                    isExist.OfficeTelephone = enrollment.OfficeTelephone;
                    isExist.OfficeZip = enrollment.OfficeZip;
                    isExist.PriorYearEFIN = enrollment.LastYearEFIN;
                    isExist.PriorYearFund = enrollment.BankProductFund;
                    isExist.PriorYearVolume = enrollment.LastYearVolume;
                    isExist.ShippingAddress = enrollment.ShippingAddress;
                    isExist.ShippingCity = enrollment.ShippingCity;
                    isExist.ShippingState = enrollment.ShippingState;
                    isExist.ShippingZip = enrollment.ShippingZip;

                    isExist.OwnerTitle = enrollment.EfinOwnerTitle;
                    isExist.OwnerMobile = enrollment.EfinOwnerMobile;
                    isExist.OwnerIdNumber = enrollment.EfinIDNumber;
                    isExist.OwnerIdState = enrollment.EfinIdState;
                    isExist.FeeOnAll = enrollment.SbfeeAll;
                    isExist.AddonFee = enrollment.Addonfee;
                    isExist.ServiceBureauFee = enrollment.ServiceBureaufee;
                    isExist.DocPrepFee = enrollment.DocPrepFee;
                    isExist.BankName = enrollment.BankName;
                    isExist.AccountName = enrollment.AccountName;
                    isExist.AgreeBank = enrollment.AgreeBank;
                    isExist.CheckPrint = enrollment.CheckPrint;
                    isExist.IsEnrtyCompleted = true;
                    isExist.EntryLevel = 5;

                    isExist.UpdatedBy = enrollment.UserId;
                    isExist.UpdatedDate = DateTime.Now;

                    db.SaveChanges();
                }
                else
                {
                    BankEnrollmentForTPG tpg = new BankEnrollmentForTPG();
                    tpg.BankUsedLastYear = enrollment.LastYearBank;
                    tpg.CompanyName = enrollment.CompanyName;
                    tpg.EFINOwnerAddress = enrollment.OwnerAddress;
                    tpg.EFINOwnerCity = enrollment.OwnerCity;
                    tpg.EFINOwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    tpg.EFINOwnerEIN = enrollment.OwnerEIN;
                    tpg.EFINOwnerEmail = enrollment.OwnerEmail;
                    tpg.EFINOwnerFirstName = enrollment.OwnerFirstName;
                    tpg.EFINOwnerLastName = enrollment.OwnerLastName;
                    tpg.EFINOwnerSSN = enrollment.OwnerSSn;
                    tpg.EFINOwnerState = enrollment.OwnerState;
                    tpg.EFINOwnerTelephone = enrollment.OwnerTelephone;
                    tpg.EFINOwnerZip = enrollment.OwnerZip;
                    tpg.Id = Guid.NewGuid();
                    tpg.ManagerEmail = enrollment.ManagerEmail;
                    tpg.ManagerFirstName = enrollment.ManagerFirstName;
                    tpg.ManagerLastName = enrollment.ManagerLastName;
                    tpg.OfficeAccountType = enrollment.AccountType;
                    tpg.OfficeAddress = enrollment.OfficeAddress;
                    tpg.OfficeCity = enrollment.OfficeCity;
                    tpg.OfficeDAN = enrollment.OfficeDAN;
                    tpg.OfficeFAX = enrollment.OfficeFax;
                    tpg.OfficeRTN = enrollment.OfficeRTN;
                    tpg.OfficeState = enrollment.OfficeState;
                    tpg.OfficeTelephone = enrollment.OfficeTelephone;
                    tpg.OfficeZip = enrollment.OfficeZip;
                    tpg.PriorYearEFIN = enrollment.LastYearEFIN;
                    tpg.PriorYearFund = enrollment.BankProductFund;
                    tpg.PriorYearVolume = enrollment.LastYearVolume;
                    tpg.ShippingAddress = enrollment.ShippingAddress;
                    tpg.ShippingCity = enrollment.ShippingCity;
                    tpg.ShippingState = enrollment.ShippingState;
                    tpg.ShippingZip = enrollment.ShippingZip;

                    tpg.CustomerId = enrollment.CustomerId;
                    tpg.StatusCode = EMPConstants.Active;
                    tpg.OwnerTitle = enrollment.EfinOwnerTitle;
                    tpg.OwnerMobile = enrollment.EfinOwnerMobile;
                    tpg.OwnerIdNumber = enrollment.EfinIDNumber;
                    tpg.OwnerIdState = enrollment.EfinIdState;
                    tpg.FeeOnAll = enrollment.SbfeeAll;
                    tpg.AddonFee = enrollment.Addonfee;
                    tpg.ServiceBureauFee = enrollment.ServiceBureaufee;
                    tpg.DocPrepFee = enrollment.DocPrepFee;
                    tpg.BankName = enrollment.BankName;
                    tpg.AccountName = enrollment.AccountName;
                    tpg.AgreeBank = enrollment.AgreeBank;
                    tpg.CheckPrint = enrollment.CheckPrint;
                    tpg.IsEnrtyCompleted = true;
                    tpg.EntryLevel = 5;

                    tpg.CreatedBy = enrollment.UserId;
                    tpg.CreatedDate = DateTime.Now;
                    tpg.UpdatedBy = enrollment.UserId;
                    tpg.UpdatedDate = DateTime.Now;

                    db.BankEnrollmentForTPGs.Add(tpg);
                    db.SaveChanges();
                }
                //  saveEnrollmentCompletedStatus(enrollment.UserId ?? Guid.Empty, enrollment.CustomerId);

                var isaprvd = db.EnrollmentBankSelections.Where(x => x.BankId == enrollment.BankId && x.BankSubmissionStatus == 1 && x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isaprvd != null)
                {
                    saveEnrollmenttoService(enrollment.CustomerId, enrollment.UserId ?? Guid.Empty, enrollment.BankId, Guid.Empty);
                    UpdateDefaultBankStatus(enrollment.CustomerId, enrollment.UserId ?? Guid.Empty, enrollment.BankId, true);
                }

                return new XlinkResponseModel() { Status = true };
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveTPGBankEnrollment", Guid.Empty);

                return new XlinkResponseModel() { Status = false };
            }
        }

        public TPGBankEnrollment GetTPGBankEnrollment(Guid CustomerId, bool IsStaging, Guid xBankid)
        {
            TPGBankEnrollment tpg = new DTO.TPGBankEnrollment();
            try
            {
                var isExist = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    tpg = getTPGData(isExist, CustomerId, IsStaging, xBankid);
                }
                else
                {
                    var isolddata = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.InActive).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (isolddata != null)
                    {
                        tpg = getTPGData(isolddata, CustomerId, IsStaging, xBankid);
                    }
                    else
                    {
                        var IsMainSite = true;
                        var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                        if (info != null)
                        {
                            tpg.CompanyName = info.CompanyName;
                            tpg.OwnerFirstName = info.BusinessOwnerFirstName;
                            tpg.OwnerLastName = info.BusinesOwnerLastName;
                            tpg.ManagerFirstName = info.BusinessOwnerFirstName;
                            tpg.ManagerLastName = info.BusinesOwnerLastName;
                            tpg.OfficeTelephone = info.OfficePhone;
                            tpg.ShippingAddress = info.ShippingAddress1;
                            tpg.ShippingCity = info.ShippingCity;
                            tpg.ShippingState = info.ShippingState;
                            tpg.ShippingZip = info.ShippingZipCode;
                            tpg.OfficeAddress = info.PhysicalAddress1;
                            tpg.OfficeCity = info.PhysicalCity;
                            tpg.OfficeState = info.PhysicalState;
                            tpg.OfficeZip = info.PhysicalZipCode;
                            tpg.OwnerAddress = info.PhysicalAddress1;
                            tpg.OwnerCity = info.PhysicalCity;
                            tpg.OwnerState = info.PhysicalState;
                            tpg.OwnerZip = info.PhysicalZipCode;
                            tpg.OwnerTelephone = info.OfficePhone;
                            tpg.OwnerEmail = info.PrimaryEmail;
                            tpg.ManagerEmail = info.PrimaryEmail;
                            tpg.AgreeBank = false;
                            tpg.LastYearEFIN = info.EFIN.ToString().PadLeft(6, '0');
                            tpg.SbfeeAll = "";

                            if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                            {
                                IsMainSite = false;
                                var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                                if (SubSiteOffice != null)
                                {
                                    info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                                }
                            }

                            decimal addon = 0, sb = 0;
                            var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.BankId == xBankid && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                            string addontitle = string.Empty, svbfeetitle = string.Empty;

                            if (bankid != null)
                            {
                                if (bankid.BankId != Guid.Empty)
                                {
                                    var addonMasterfees = (from f in db.FeeMasters
                                                           where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                           select f.Amount).FirstOrDefault();
                                    addon += addonMasterfees.Value;

                                    addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                                    var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                                    if (!IsMainSite)
                                    {
                                        if ((info.IsMSOUser ?? false) == false)
                                        {
                                            decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                            addon += BankFee;
                                            addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                        }
                                        else
                                        {
                                            decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                            addon += BankFee;
                                            addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                        }
                                    }

                                    addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                                    addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                                    var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                                    var mainfees = (from f in db.FeeMasters
                                                    where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                                    select f.Amount).FirstOrDefault();
                                    sb += mainfees.Value;
                                    svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";

                                    if (!IsMainSite)
                                    {
                                        if ((info.IsMSOUser ?? false) == false)
                                        {
                                            decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                            sb += BankMaxFees;
                                            svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                        }
                                        else
                                        {
                                            decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                            sb += BankMaxFees;
                                            svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                        }
                                    }

                                    sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                                    svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                                }
                            }

                            tpg.Addonfee = addon.ToString("0.00");
                            tpg.ServiceBureaufee = sb.ToString("0.00");
                            tpg.AddonfeeTitle = addontitle;
                            tpg.ServiceBureaufeeTitle = svbfeetitle;

                            var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();

                            string MasterId = logininfo.MasterIdentifier;
                            string Password = "";
                            int CrossLinkUserId = 0;
                            getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, logininfo.MasterIdentifier, info.ParentId ?? Guid.Empty, info.EntityId ?? 0);


                            //string CLPassword = PasswordManager.DecryptText(logininfo.CrossLinkPassword);
                            string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                            if (_accesskey != "")
                            {
                                AuthObject _objAuth = new AuthObject();
                                _objAuth.AccessKey = _accesskey;
                                _objAuth.UserID = MasterId;
                                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                                if (isValid.success)
                                {
                                    int efinid = _apiObj.getEFINID(info.EFIN.Value);
                                    var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, "S");//TPG - S
                                    if (latestbankApp.BankAppID != 0)
                                    {
                                        var bankinfo = _apiObj.getSBTPGApp(_objAuth, latestbankApp.BankAppID);
                                        if (bankinfo.Response.success)
                                        {
                                            tpg.ManagerFirstName = bankinfo.ManagerFName;
                                            tpg.ManagerLastName = bankinfo.ManagerLName;
                                            tpg.ManagerEmail = bankinfo.ManagerEmail;
                                            tpg.LastYearBank = bankinfo.RalBankLY;
                                            tpg.LastYearEFIN = bankinfo.EFINLY.HasValue ? bankinfo.EFINLY.Value.ToString() : "";
                                            tpg.LastYearVolume = bankinfo.VolumeLY.HasValue ? bankinfo.VolumeLY.Value.ToString() : "";
                                            tpg.BankProductFund = bankinfo.BankProductsLY.HasValue ? bankinfo.BankProductsLY.Value.ToString() : "";
                                            tpg.CheckPrint = bankinfo.CheckPrint;
                                        }
                                    }
                                    var efinobj = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                                    //if (efinobj.Response.success)
                                    //{
                                    tpg.OwnerEIN = efinobj.EIN;
                                    tpg.OwnerSSn = efinobj.SSN;
                                    tpg.OwnerFirstName = efinobj.FName;
                                    tpg.OwnerLastName = efinobj.LName;
                                    tpg.EfinOwnerTitle = efinobj.Title;
                                    tpg.OwnerTelephone = efinobj.Phone;
                                    tpg.EfinOwnerMobile = efinobj.Mobile;
                                    tpg.OwnerCity = efinobj.City;
                                    tpg.OwnerState = efinobj.State;
                                    tpg.OwnerZip = efinobj.Zip;
                                    tpg.OwnerDOB = efinobj.DOB.HasValue ? efinobj.DOB.Value.ToString("MM'/'dd'/'yyyy") : "";
                                    tpg.EfinIDNumber = efinobj.IDNumber;
                                    tpg.EfinIdState = efinobj.IDState;

                                    //}
                                }
                            }
                        }
                        else
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetTPGBankEnrollment", CustomerId);
                //GetTPGBankEnrollment
                return null;
            }
            return tpg;
        }

        public TPGBankEnrollment getTPGData(BankEnrollmentForTPG isExist, Guid CustomerId, bool IsStaging, Guid xBankId)
        {
            var IsMainSite = true;
            try
            {
                TPGBankEnrollment tpg = new TPGBankEnrollment();

                if (isExist.EntryLevel == 5)
                {
                    tpg.AccountType = isExist.OfficeAccountType;
                    tpg.BankProductFund = isExist.PriorYearFund;
                    tpg.CompanyName = isExist.CompanyName;
                    tpg.LastYearBank = isExist.BankUsedLastYear;
                    tpg.LastYearEFIN = isExist.PriorYearEFIN;
                    tpg.LastYearVolume = isExist.PriorYearVolume;
                    tpg.ManagerEmail = isExist.ManagerEmail;
                    tpg.ManagerFirstName = isExist.ManagerFirstName;
                    tpg.ManagerLastName = isExist.ManagerLastName;
                    tpg.OfficeAddress = isExist.OfficeAddress;
                    tpg.OfficeCity = isExist.OfficeCity;
                    tpg.OfficeDAN = isExist.OfficeDAN;
                    tpg.OfficeFax = isExist.OfficeFAX;
                    tpg.OfficeRTN = isExist.OfficeRTN;
                    tpg.OfficeState = isExist.OfficeState;
                    tpg.OfficeTelephone = isExist.OfficeTelephone;
                    tpg.OfficeZip = isExist.OfficeZip;
                    tpg.OwnerAddress = isExist.EFINOwnerAddress;
                    tpg.OwnerCity = isExist.EFINOwnerCity;
                    tpg.OwnerDOB = isExist.EFINOwnerDOB.Value.ToString("MM'/'dd'/'yyyy");
                    tpg.OwnerEIN = isExist.EFINOwnerEIN;
                    tpg.OwnerEmail = isExist.EFINOwnerEmail;
                    tpg.OwnerFirstName = isExist.EFINOwnerFirstName;
                    tpg.OwnerLastName = isExist.EFINOwnerLastName;
                    tpg.OwnerSSn = isExist.EFINOwnerSSN;
                    tpg.OwnerState = isExist.EFINOwnerState;
                    tpg.OwnerTelephone = isExist.EFINOwnerTelephone;
                    tpg.OwnerZip = isExist.EFINOwnerZip;
                    tpg.ShippingAddress = isExist.ShippingAddress;
                    tpg.ShippingCity = isExist.ShippingCity;
                    tpg.ShippingState = isExist.ShippingState;
                    tpg.ShippingZip = isExist.ShippingZip;
                    tpg.EfinOwnerTitle = isExist.OwnerTitle;
                    tpg.EfinOwnerMobile = isExist.OwnerMobile;
                    tpg.EfinIDNumber = isExist.OwnerIdNumber;
                    tpg.EfinIdState = isExist.OwnerIdState;

                    tpg.CheckPrint = isExist.CheckPrint;
                    tpg.AgreeBank = isExist.AgreeBank.HasValue ? isExist.AgreeBank.Value : false;
                    tpg.SbfeeAll = isExist.FeeOnAll;
                    tpg.DocPrepFee = isExist.DocPrepFee;
                    tpg.BankName = isExist.BankName;
                    tpg.AccountName = isExist.AccountName;


                    var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                    {
                        IsMainSite = false;
                        var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                        if (SubSiteOffice != null)
                        {
                            info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                        }
                    }

                    decimal addon = 0, sb = 0;
                    var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == xBankId).FirstOrDefault();
                    string addontitle = string.Empty, svbfeetitle = string.Empty;

                    if (bankid != null)
                    {
                        if (bankid.BankId != Guid.Empty)
                        {
                            var customerfee = (from s in db.CustomerAssociatedFees
                                               join f in db.FeeMasters on s.FeeMaster_ID equals f.Id
                                               where s.emp_CustomerInformation_ID == CustomerId && s.IsActive == true && f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                               select s).FirstOrDefault();
                            if (customerfee == null)
                            {
                                var addonMasterfees = (from f in db.FeeMasters
                                                       where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                       select f.Amount).FirstOrDefault();
                                addon += addonMasterfees.Value;
                                addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";
                            }
                            else
                            {
                                addon += customerfee.Amount;
                                addontitle = addontitle + "Cross Link Transmitter Fee :" + customerfee.Amount + ", ";
                            }

                            var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                                else
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                            }

                            decimal trnasaddon = bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0, svbaddon = bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                            if (IsStaging)
                            {
                                var staging = db.EnrollmentAddonStagings.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == bankid.BankId).FirstOrDefault();
                                if (staging != null)
                                {
                                    trnasaddon = staging.IsTransmissionFee ? staging.TransmissionAddonAmount : 0;
                                    svbaddon = staging.IsSvbFee ? staging.SvbAddonAmount : 0;
                                }
                            }

                            addon += trnasaddon;
                            addontitle = addontitle + "Add On Fee :" + trnasaddon;

                            var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                            var customersvbfee = (from s in db.CustomerAssociatedFees
                                                  join f in db.FeeMasters on s.FeeMaster_ID equals f.Id
                                                  where s.emp_CustomerInformation_ID == CustomerId && s.IsActive == true && f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                                  select s).FirstOrDefault();
                            if (customersvbfee == null)
                            {
                                var mainfees = (from f in db.FeeMasters
                                                where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                                select f.Amount).FirstOrDefault();
                                sb += mainfees.Value;
                                svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                            }
                            else
                            {
                                sb += customersvbfee.Amount;
                                svbfeetitle = svbfeetitle + "uTax Service Fee :" + customersvbfee.Amount + ", ";
                            }
                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                                else
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }

                            }
                            sb += svbaddon;
                            svbfeetitle = svbfeetitle + "Add On Fee :" + svbaddon;
                        }
                    }

                    tpg.AddonfeeTitle = addontitle;
                    tpg.ServiceBureaufeeTitle = svbfeetitle;

                    if (string.IsNullOrEmpty(tpg.Addonfee))
                    {
                        tpg.Addonfee = addon.ToString("0.00");
                    }

                    if (string.IsNullOrEmpty(tpg.ServiceBureaufee))
                    {
                        tpg.ServiceBureaufee = sb.ToString("0.00");
                    }
                }
                else
                {
                    SBTPGAppObject bankobj = new SBTPGAppObject();
                    EfinObject efinobj = new CrosslinkService.EfinObject();

                    var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if (info != null)
                    {
                        var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                        //string CLPassword = PasswordManager.DecryptText(logininfo.CrossLinkPassword);

                        string MasterId = logininfo.MasterIdentifier;
                        string Password = "";
                        int CrossLinkUserId = 0;
                        getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, logininfo.MasterIdentifier, info.ParentId ?? Guid.Empty, info.EntityId ?? 0);

                        string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                        if (_accesskey != "")
                        {
                            AuthObject _objAuth = new AuthObject();
                            _objAuth.AccessKey = _accesskey;
                            _objAuth.UserID = MasterId;
                            XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                            if (isValid.success)
                            {
                                int efinid = _apiObj.getEFINID(info.EFIN.Value);
                                var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, "S");//TPG - S
                                if (latestbankApp.BankAppID != 0)
                                {
                                    var bankinfo = _apiObj.getSBTPGApp(_objAuth, latestbankApp.BankAppID);
                                    bankobj = bankinfo;
                                }
                                efinobj = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                            }
                        }
                        if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                        {
                            IsMainSite = false;
                            var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                            if (SubSiteOffice != null)
                            {
                                info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                            }
                        }
                    }

                    decimal addon = 0, sb = 0;
                    string addontitle = string.Empty, svbfeetitle = string.Empty;
                    var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == xBankId).FirstOrDefault();

                    if (bankid != null)
                    {
                        if (bankid.BankId != Guid.Empty)
                        {
                            var addonMasterfees = (from f in db.FeeMasters
                                                   where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                   select f.Amount).FirstOrDefault();
                            addon += addonMasterfees.Value;

                            addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                            var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                                else
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                            }

                            addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                            addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                            var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                            var mainfees = (from f in db.FeeMasters
                                            where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                            select f.Amount).FirstOrDefault();
                            sb += mainfees.Value;
                            svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                                else
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                            }

                            sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                            svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                        }
                    }

                    if (isExist.EntryLevel >= 1)
                    {
                        tpg.CompanyName = isExist.CompanyName;
                        tpg.ManagerEmail = isExist.ManagerEmail;
                        tpg.ManagerFirstName = isExist.ManagerFirstName;
                        tpg.ManagerLastName = isExist.ManagerLastName;
                        tpg.OfficeAddress = isExist.OfficeAddress;
                        tpg.OfficeCity = isExist.OfficeCity;
                        tpg.OfficeDAN = isExist.OfficeDAN;
                        tpg.OfficeFax = isExist.OfficeFAX;
                        tpg.OfficeRTN = isExist.OfficeRTN;
                        tpg.OfficeState = isExist.OfficeState;
                        tpg.OfficeTelephone = isExist.OfficeTelephone;
                        tpg.OfficeZip = isExist.OfficeZip;
                        tpg.ShippingAddress = isExist.ShippingAddress;
                        tpg.ShippingCity = isExist.ShippingCity;
                        tpg.ShippingState = isExist.ShippingState;
                        tpg.ShippingZip = isExist.ShippingZip;

                        tpg.LastYearBank = bankobj.RalBankLY;
                        tpg.LastYearEFIN = bankobj.EFINLY.HasValue ? bankobj.EFINLY.Value.ToString() : "";
                        tpg.LastYearVolume = bankobj.VolumeLY.HasValue ? bankobj.VolumeLY.Value.ToString() : "";
                        tpg.BankProductFund = bankobj.BankProductsLY.HasValue ? bankobj.BankProductsLY.Value.ToString() : "";
                        tpg.CheckPrint = bankobj.CheckPrint;
                        tpg.OwnerEIN = efinobj.EIN;
                        tpg.OwnerSSn = efinobj.SSN;
                        tpg.OwnerFirstName = efinobj.FName;
                        tpg.OwnerLastName = efinobj.LName;
                        tpg.EfinOwnerTitle = efinobj.Title;
                        tpg.OwnerTelephone = efinobj.Phone;
                        tpg.EfinOwnerMobile = efinobj.Mobile;
                        tpg.OwnerCity = efinobj.City;
                        tpg.OwnerState = efinobj.State;
                        tpg.OwnerZip = efinobj.Zip;
                        tpg.OwnerDOB = efinobj.DOB.HasValue ? efinobj.DOB.Value.ToString("MM'/'dd'/'yyyy") : "";
                        tpg.EfinIDNumber = efinobj.IDNumber;
                        tpg.EfinIdState = efinobj.IDState;

                        tpg.Addonfee = addon.ToString("0.00");
                        tpg.ServiceBureaufee = sb.ToString("0.00");
                        tpg.AddonfeeTitle = addontitle;
                        tpg.ServiceBureaufeeTitle = svbfeetitle;
                    }
                    if (isExist.EntryLevel >= 2)
                    {
                        tpg.OwnerAddress = isExist.EFINOwnerAddress;
                        tpg.OwnerCity = isExist.EFINOwnerCity;
                        tpg.OwnerDOB = isExist.EFINOwnerDOB.HasValue ? isExist.EFINOwnerDOB.Value.ToString("MM'/'dd'/'yyyy") : "";
                        tpg.OwnerEIN = isExist.EFINOwnerEIN;
                        tpg.OwnerEmail = isExist.EFINOwnerEmail;
                        tpg.OwnerFirstName = isExist.EFINOwnerFirstName;
                        tpg.OwnerLastName = isExist.EFINOwnerLastName;
                        tpg.OwnerSSn = isExist.EFINOwnerSSN;
                        tpg.OwnerState = isExist.EFINOwnerState;
                        tpg.OwnerTelephone = isExist.EFINOwnerTelephone;
                        tpg.OwnerZip = isExist.EFINOwnerZip;
                        tpg.EfinOwnerTitle = isExist.OwnerTitle;
                        tpg.EfinOwnerMobile = isExist.OwnerMobile;
                        tpg.EfinIDNumber = isExist.OwnerIdNumber;
                        tpg.EfinIdState = isExist.OwnerIdState;

                        tpg.LastYearBank = bankobj.RalBankLY;
                        tpg.LastYearEFIN = bankobj.EFINLY.HasValue ? bankobj.EFINLY.Value.ToString() : "";
                        tpg.LastYearVolume = bankobj.VolumeLY.HasValue ? bankobj.VolumeLY.Value.ToString() : "";
                        tpg.BankProductFund = bankobj.BankProductsLY.HasValue ? bankobj.BankProductsLY.Value.ToString() : "";
                        tpg.CheckPrint = bankobj.CheckPrint;

                        tpg.Addonfee = addon.ToString("0.00");
                        tpg.ServiceBureaufee = sb.ToString("0.00");
                        tpg.AddonfeeTitle = addontitle;
                        tpg.ServiceBureaufeeTitle = svbfeetitle;
                    }

                    if (isExist.EntryLevel >= 3)
                    {
                        tpg.Addonfee = isExist.AddonFee;
                        tpg.ServiceBureaufee = isExist.ServiceBureauFee;
                        tpg.SbfeeAll = isExist.FeeOnAll;
                        tpg.DocPrepFee = isExist.DocPrepFee;
                    }
                    if (isExist.EntryLevel >= 4)
                    {
                        tpg.BankProductFund = isExist.PriorYearFund;
                        tpg.LastYearBank = isExist.BankUsedLastYear;
                        tpg.LastYearEFIN = isExist.PriorYearEFIN;
                        tpg.LastYearVolume = isExist.PriorYearVolume;
                    }
                }
                return tpg;
            }
            catch (Exception ex)
            {
                return new TPGBankEnrollment();
            }
        }

        public XlinkResponseModel SaveRABankEnrollment(RABankEnrollment enrollment)
        {
            try
            {
                db = new DatabaseEntities();
                var isExist = db.BankEnrollmentForRAs.Where(x => x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    var enrollstatus = db.BankEnrollments.Where(x => x.CustomerId == enrollment.CustomerId && x.IsActive == true && x.BankId == enrollment.BankId).FirstOrDefault();
                    if (enrollstatus != null)
                    {
                        if (enrollstatus.StatusCode == EMPConstants.Approved)
                        {
                            EfinObject _objefin = new CrosslinkService.EfinObject();

                            _objefin.AcctName = enrollment.AccountName;
                            _objefin.AcctType = enrollment.BankAccountType;
                            _objefin.Address = enrollment.OwnerAddress;
                            _objefin.AgreePEIDate = DateTime.Now;
                            _objefin.AgreePEITerms = true;
                            _objefin.AgreeFeeOption = true;
                            _objefin.BankName = enrollment.BankName;
                            _objefin.City = enrollment.OwnerCity;
                            _objefin.Company = enrollment.EROOfficeName;
                            _objefin.CreatedDate = DateTime.Now;
                            _objefin.DAN = enrollment.BankAccountNumber;
                            _objefin.DOB = Convert.ToDateTime(enrollment.OwnerDOB);
                            _objefin.EFINType = "S";
                            _objefin.EIN = enrollment.BusinessEIN;
                            _objefin.Email = enrollment.OwnerEmail;
                            _objefin.FivePlus = false;
                            _objefin.FName = enrollment.OwnerFirstName;
                            _objefin.IDNumber = enrollment.OwnerStateIssuedIdNumber;
                            _objefin.IDState = enrollment.OwnerIssuingState;
                            _objefin.LName = enrollment.OwnerLastName;
                            _objefin.Mobile = string.IsNullOrEmpty(enrollment.OwnerCellPhone) ? "" : enrollment.OwnerCellPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            _objefin.Phone = enrollment.OwnerHomePhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            _objefin.RTN = enrollment.BankRoutingNumber;
                            _objefin.SBFeeAll = enrollment.SbFeeall;
                            _objefin.SelectedBank = EMPConstants.RABank;
                            _objefin.SSN = enrollment.OwnerSSN;
                            _objefin.State = enrollment.OwnerState;
                            _objefin.Title = enrollment.OwnerTitle;
                            _objefin.UpdatedDate = DateTime.Now;
                            _objefin.Zip = enrollment.OwnerZipCode;

                            var res = UpdateBankEfinObject(_objefin, enrollstatus.BankId.Value, enrollstatus.Id, enrollment.CustomerId);
                            if (!res.Status)
                            {
                                return res;
                            }

                            isExist.IsUpdated = true;
                        }
                    }

                    isExist.BankAccountNumber = enrollment.BankAccountNumber;
                    isExist.BankAccountType = enrollment.BankAccountType;
                    isExist.BankRoutingNumber = enrollment.BankRoutingNumber;
                    isExist.BusinessEIN = enrollment.BusinessEIN;
                    isExist.BusinessFederalIDNumber = enrollment.BusinessFederalIDNumber;
                    isExist.CollectionofBusinessOwners = enrollment.CollectionofBusinessOwners;
                    isExist.CollectionOfOtherOwners = enrollment.CollectionOfOtherOwners;
                    isExist.CorporationType = enrollment.CorporationType;
                    isExist.EFINOwnersSite = enrollment.EFINOwnersSite;
                    isExist.EROMailingCity = enrollment.EROMailingCity;
                    isExist.EROMailingState = enrollment.EROMailingState;
                    isExist.EROMailingZipcode = enrollment.EROMailingZipcode;
                    isExist.EROMaillingAddress = enrollment.EROMaillingAddress;
                    isExist.EROOfficeAddress = enrollment.EROOfficeAddress;
                    isExist.EROOfficeCity = enrollment.EROOfficeCity;
                    isExist.EROOfficeName = enrollment.EROOfficeName;

                    if (!string.IsNullOrEmpty(enrollment.EROOfficePhone))
                    {
                        isExist.EROOfficePhone = enrollment.EROOfficePhone.Replace("-", "");
                    }

                    isExist.EROOfficeState = enrollment.EROOfficeState;
                    isExist.EROOfficeZipCoce = enrollment.EROOfficeZipCoce;
                    isExist.EROShippingAddress = enrollment.EROShippingAddress;
                    isExist.EROShippingCity = enrollment.EROShippingCity;
                    isExist.EROShippingState = enrollment.EROShippingState;
                    isExist.EROShippingState = enrollment.EROShippingState;
                    isExist.EROShippingZip = enrollment.EROShippingZip;
                    isExist.ExpectedCurrentYearVolume = enrollment.ExpectedCurrentYearVolume;
                    isExist.HasAssociatedWithVictims = enrollment.HasAssociatedWithVictims;
                    isExist.IRSAddress = enrollment.IRSAddress;
                    isExist.IRSCity = enrollment.IRSCity;
                    isExist.IRSState = enrollment.IRSState;
                    isExist.IRSZipcode = enrollment.IRSZipcode;
                    isExist.IsLastYearClient = enrollment.IsLastYearClient;
                    isExist.NoofYearsExperience = enrollment.NoofYearsExperience;
                    isExist.OwnerAddress = enrollment.OwnerAddress;

                    if (!string.IsNullOrEmpty(enrollment.OwnerCellPhone))
                    {
                        isExist.OwnerCellPhone = enrollment.OwnerCellPhone.Replace("-", "");
                    }

                    isExist.OwnerCity = enrollment.OwnerCity;

                    if (!string.IsNullOrEmpty(enrollment.OwnerDOB))
                    {
                        isExist.OwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }

                    isExist.OwnerEmail = enrollment.OwnerEmail;
                    isExist.OwnerFirstName = enrollment.OwnerFirstName;
                    if (!string.IsNullOrEmpty(enrollment.OwnerHomePhone))
                    {
                        isExist.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    }
                    isExist.OwnerIssuingState = enrollment.OwnerIssuingState;
                    isExist.OwnerLastName = enrollment.OwnerLastName;
                    isExist.OwnerSSN = enrollment.OwnerSSN;
                    isExist.OwnerState = enrollment.OwnerState;
                    isExist.OwnerStateIssuedIdNumber = enrollment.OwnerStateIssuedIdNumber;
                    isExist.OwnerZipCode = enrollment.OwnerZipCode;
                    isExist.PreviousBankName = enrollment.PreviousBankName;
                    isExist.PreviousYearVolume = enrollment.PreviousYearVolume;

                    isExist.OwnerTitle = enrollment.OwnerTitle;
                    isExist.SbFeeall = enrollment.SbFeeall;
                    isExist.TransmissionAddon = enrollment.TransmissionAddon;
                    isExist.SbFee = enrollment.SbFee;
                    isExist.ElectronicFee = enrollment.ElectronicFee;
                    isExist.AgreeTandC = enrollment.AgreeTandC;
                    isExist.BankName = enrollment.BankName;
                    isExist.AccountName = enrollment.AccountName;
                    isExist.MainContactFirstName = enrollment.MainContactFirstName;
                    isExist.MainContactLastName = enrollment.MainContactLastName;
                    isExist.MainContactPhone = enrollment.MainContactPhone;
                    isExist.TextMessages = enrollment.TextMessages;
                    isExist.LegalIssues = enrollment.LegalIssues;
                    isExist.StateOfIncorporation = enrollment.StateOfIncorporation;
                    isExist.IsEnrtyCompleted = true;
                    isExist.EntryLevel = 4;

                    isExist.UpdatedBy = enrollment.UserId;
                    isExist.UpdatedDate = DateTime.Now;

                    db.SaveChanges();
                    //  saveEnrollmentCompletedStatus(enrollment.UserId ?? Guid.Empty, enrollment.CustomerId);
                    SaveRAEFINOwnerInfo(enrollment.RAEFINOwnerInfo, enrollment.UserId ?? Guid.Empty, isExist.Id);
                }
                else
                {
                    BankEnrollmentForRA ra = new BankEnrollmentForRA();

                    ra.Id = Guid.NewGuid();
                    ra.CustomerId = enrollment.CustomerId;

                    ra.BankAccountNumber = enrollment.BankAccountNumber;
                    ra.BankAccountType = enrollment.BankAccountType;
                    ra.BankRoutingNumber = enrollment.BankRoutingNumber;
                    ra.BusinessEIN = enrollment.BusinessEIN;
                    ra.BusinessFederalIDNumber = enrollment.BusinessFederalIDNumber;
                    ra.CollectionofBusinessOwners = enrollment.CollectionofBusinessOwners;
                    ra.CollectionOfOtherOwners = enrollment.CollectionOfOtherOwners;
                    ra.CorporationType = enrollment.CorporationType;
                    ra.EFINOwnersSite = enrollment.EFINOwnersSite;
                    ra.EROMailingCity = enrollment.EROMailingCity;
                    ra.EROMailingState = enrollment.EROMailingState;
                    ra.EROMailingZipcode = enrollment.EROMailingZipcode;
                    ra.EROMaillingAddress = enrollment.EROMaillingAddress;
                    ra.EROOfficeAddress = enrollment.EROOfficeAddress;
                    ra.EROOfficeCity = enrollment.EROOfficeCity;
                    ra.EROOfficeName = enrollment.EROOfficeName;
                    if (!string.IsNullOrEmpty(enrollment.EROOfficePhone))
                    {
                        ra.EROOfficePhone = enrollment.EROOfficePhone.Replace("-", "");
                    }
                    ra.EROOfficeState = enrollment.EROOfficeState;
                    ra.EROOfficeZipCoce = enrollment.EROOfficeZipCoce;
                    ra.EROShippingAddress = enrollment.EROShippingAddress;
                    ra.EROShippingCity = enrollment.EROShippingCity;
                    ra.EROShippingState = enrollment.EROShippingState;
                    ra.EROShippingZip = enrollment.EROShippingZip;
                    ra.ExpectedCurrentYearVolume = enrollment.ExpectedCurrentYearVolume;
                    ra.HasAssociatedWithVictims = enrollment.HasAssociatedWithVictims;
                    ra.IRSAddress = enrollment.IRSAddress;
                    ra.IRSCity = enrollment.IRSCity;
                    ra.IRSState = enrollment.IRSState;
                    ra.IRSZipcode = enrollment.IRSZipcode;
                    ra.IsLastYearClient = enrollment.IsLastYearClient;
                    ra.NoofYearsExperience = enrollment.NoofYearsExperience;
                    ra.OwnerAddress = enrollment.OwnerAddress;
                    ra.StateOfIncorporation = enrollment.StateOfIncorporation;

                    if (!string.IsNullOrEmpty(enrollment.OwnerCellPhone))
                    {
                        ra.OwnerCellPhone = enrollment.OwnerCellPhone.Replace("-", "");
                    }

                    ra.OwnerCity = enrollment.OwnerCity;

                    if (!string.IsNullOrEmpty(enrollment.OwnerDOB))
                    {
                        ra.OwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }

                    ra.OwnerEmail = enrollment.OwnerEmail;
                    ra.OwnerFirstName = enrollment.OwnerFirstName;

                    if (!string.IsNullOrEmpty(enrollment.OwnerHomePhone))
                    {
                        ra.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    }

                    ra.OwnerIssuingState = enrollment.OwnerIssuingState;
                    ra.OwnerLastName = enrollment.OwnerLastName;
                    ra.OwnerSSN = enrollment.OwnerSSN;
                    ra.OwnerState = enrollment.OwnerState;
                    ra.OwnerStateIssuedIdNumber = enrollment.OwnerStateIssuedIdNumber;
                    ra.OwnerZipCode = enrollment.OwnerZipCode;
                    ra.PreviousBankName = enrollment.PreviousBankName;
                    ra.PreviousYearVolume = enrollment.PreviousYearVolume;


                    int? efin = db.emp_CustomerInformation.Where(x => x.Id == enrollment.CustomerId).Select(x => x.EFIN).FirstOrDefault();
                    ra.EfinID = efin == null ? "" : efin.ToString().PadLeft(6, '0');


                    ra.OwnerTitle = enrollment.OwnerTitle;
                    ra.SbFeeall = enrollment.SbFeeall;
                    ra.TransmissionAddon = enrollment.TransmissionAddon;
                    ra.SbFee = enrollment.SbFee;
                    ra.ElectronicFee = enrollment.ElectronicFee;
                    ra.AgreeTandC = enrollment.AgreeTandC;
                    ra.BankName = enrollment.BankName;
                    ra.AccountName = enrollment.AccountName;
                    ra.MainContactFirstName = enrollment.MainContactFirstName;
                    ra.MainContactLastName = enrollment.MainContactLastName;
                    ra.MainContactPhone = enrollment.MainContactPhone;
                    ra.TextMessages = enrollment.TextMessages;
                    ra.LegalIssues = enrollment.LegalIssues;

                    ra.StatusCode = EMPConstants.Active;
                    ra.CreatedBy = enrollment.UserId;
                    ra.CreatedDate = DateTime.Now;
                    ra.UpdatedBy = enrollment.UserId;
                    ra.UpdatedDate = DateTime.Now;

                    db.BankEnrollmentForRAs.Add(ra);
                    db.SaveChanges();

                    // saveEnrollmentCompletedStatus(enrollment.UserId ?? Guid.Empty, enrollment.CustomerId);

                    SaveRAEFINOwnerInfo(enrollment.RAEFINOwnerInfo, enrollment.UserId ?? Guid.Empty, ra.Id);

                }

                var isaprvd = db.EnrollmentBankSelections.Where(x => x.BankId == enrollment.BankId && x.BankSubmissionStatus == 1 && x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isaprvd != null)
                {
                    saveEnrollmenttoService(enrollment.CustomerId, enrollment.UserId ?? Guid.Empty, enrollment.BankId, Guid.Empty);
                    UpdateDefaultBankStatus(enrollment.CustomerId, enrollment.UserId ?? Guid.Empty, enrollment.BankId, true);
                }

                return new XlinkResponseModel() { Status = true };
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveRABankEnrollment", Guid.Empty);
                return new XlinkResponseModel() { Status = false };
            }
        }

        public RABankEnrollment GetRABankEnrollment(Guid CustomerId, bool IsStaging, Guid xBankid)
        {
            var IsMainSite = true;
            RABankEnrollment ra = new DTO.RABankEnrollment();
            try
            {
                var isExist = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    return getRAData(isExist, CustomerId, IsStaging, xBankid);
                }
                else
                {
                    var isoldExist = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.InActive).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (isoldExist != null)
                    {
                        return getRAData(isoldExist, CustomerId, IsStaging, xBankid);
                    }
                    else
                    {
                        var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                        if (info != null)
                        {
                            ra.EROOfficePhone = info.OfficePhone;
                            ra.EROShippingAddress = info.ShippingAddress1;
                            ra.EROShippingCity = info.ShippingCity;
                            ra.EROShippingState = info.ShippingState;
                            ra.EROShippingZip = info.ShippingZipCode;
                            ra.EROOfficeAddress = info.PhysicalAddress1;
                            ra.EROOfficeCity = info.PhysicalCity;
                            ra.EROOfficeState = info.PhysicalState;
                            ra.EROOfficeZipCoce = info.PhysicalZipCode;
                            ra.OwnerFirstName = info.BusinessOwnerFirstName;
                            ra.OwnerLastName = info.BusinesOwnerLastName;
                            ra.MainContactFirstName = info.BusinessOwnerFirstName;
                            ra.MainContactLastName = info.BusinesOwnerLastName;
                            ra.MainContactPhone = info.OfficePhone;
                            ra.EROOfficeName = info.CompanyName;

                            //var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                            //decimal addon = 0, sb = 0;
                            //if (info.ParentId != null)
                            //{
                            //    var fees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.ParentId.Value).ToList();
                            //    addon = fees.Sum(x => x.BankMaxFees);
                            //}
                            //var subaddon = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.ServiceBureauBankAmount).FirstOrDefault();
                            //addon += subaddon.HasValue ? subaddon.Value : 0;


                            //var mainfees = (from f in db.FeeMasters
                            //                where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                            //                select f.Amount).FirstOrDefault();
                            //sb += mainfees.Value;
                            //if (info.IsMSOUser ?? false)
                            //{
                            //    var customerfees = (from s in db.SubSiteBankFeesConfigs
                            //                        where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == bankid.BankId && s.ServiceOrTransmitter == 1
                            //                        select s.BankMaxFees_MSO).FirstOrDefault();
                            //    sb += customerfees.Value;
                            //}
                            //else
                            //{
                            //    var customerfees = (from s in db.SubSiteBankFeesConfigs
                            //                        where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == bankid.BankId && s.ServiceOrTransmitter == 1
                            //                        select s.BankMaxFees).FirstOrDefault();
                            //    sb += customerfees;
                            //}

                            //if (info.ParentId != null)
                            //{
                            //    var fees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.ParentId.Value && x.ServiceOrTransmitter == 1).FirstOrDefault();
                            //    if (info.IsMSOUser ?? false)
                            //    {
                            //        sb += fees.BankMaxFees_MSO.Value;
                            //    }
                            //    else
                            //        sb += fees.BankMaxFees;
                            //}
                            //sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;

                            if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                            {
                                IsMainSite = false;
                                var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                                if (SubSiteOffice != null)
                                {
                                    info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                                }
                            }

                            decimal addon = 0, sb = 0;
                            var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.BankId == xBankid && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                            string addontitle = string.Empty, svbfeetitle = string.Empty;

                            if (bankid != null)
                            {
                                if (bankid.BankId != Guid.Empty)
                                {
                                    var addonMasterfees = (from f in db.FeeMasters
                                                           where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                           select f.Amount).FirstOrDefault();
                                    addon += addonMasterfees.Value;

                                    addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                                    var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();
                                    if (!IsMainSite)
                                    {
                                        if ((info.IsMSOUser ?? false) == false)
                                        {
                                            decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                            addon += BankFee;
                                            addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                        }
                                        else
                                        {
                                            decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                            addon += BankFee;
                                            addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                        }
                                    }

                                    addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                                    addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                                    var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                                    var mainfees = (from f in db.FeeMasters
                                                    where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                                    select f.Amount).FirstOrDefault();
                                    sb += mainfees.Value;
                                    svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";

                                    if (!IsMainSite)
                                    {
                                        if ((info.IsMSOUser ?? false) == false)
                                        {
                                            decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                            sb += BankMaxFees;
                                            svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                        }
                                        else
                                        {
                                            decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                            sb += BankMaxFees;
                                            svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                        }
                                    }

                                    sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                                    svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                                }
                            }

                            ra.TransmissionAddon = addon.ToString("0.00");
                            ra.SbFee = sb.ToString("0.00");
                            ra.AddonfeeTitle = addontitle;
                            ra.ServiceBureaufeeTitle = svbfeetitle;


                            var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                            //string CLPassword = PasswordManager.DecryptText(logininfo.CrossLinkPassword);

                            string MasterId = logininfo.MasterIdentifier;
                            string Password = "";
                            int CrossLinkUserId = 0;
                            getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, logininfo.MasterIdentifier, info.ParentId ?? Guid.Empty, info.EntityId ?? 0);

                            string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                            if (_accesskey != "")
                            {
                                AuthObject _objAuth = new AuthObject();
                                _objAuth.AccessKey = _accesskey;
                                _objAuth.UserID = MasterId;
                                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                                if (isValid.success)
                                {
                                    int efinid = _apiObj.getEFINID(info.EFIN.Value);
                                    var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, "V");

                                    if (latestbankApp.BankAppID != 0)
                                    {
                                        var bankinfo = _apiObj.getRefundAdvantageApp(_objAuth, latestbankApp.BankAppID);
                                        if (bankinfo.Response.success)
                                        {
                                            ra.EROMaillingAddress = bankinfo.MailingAddressStreet;
                                            ra.EROMailingCity = bankinfo.MailingAddressCity;
                                            ra.EROMailingState = bankinfo.MailingAddressState;
                                            ra.EROMailingZipcode = bankinfo.MailingAddressZip;
                                            ra.CorporationType = bankinfo.CorporationType;

                                            var owners = _apiObj.getRefundAdvantageOwners(_objAuth, latestbankApp.BankAppID);
                                            var ownerlist = (from s in owners
                                                             select new EnrollmentBankEFINOwnerRADTO
                                                             {
                                                                 Address = s.Address,
                                                                 BankEnrollmentRAId = Guid.Empty,
                                                                 City = s.City,
                                                                 DateofBirth = s.DOB.HasValue ? s.DOB.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                                 EmailId = "",
                                                                 FirstName = s.FirstName,
                                                                 HomePhone = s.Phone,
                                                                 Id = Guid.Empty,
                                                                 IDNumber = s.IdNumber,
                                                                 IDState = s.IdState,
                                                                 LastName = s.LastName,
                                                                 MobilePhone = "",
                                                                 PercentageOwned = s.PercentOwned.HasValue ? s.PercentOwned.Value : 0,
                                                                 SSN = s.SSN,
                                                                 StateId = s.State,
                                                                 ZipCode = s.Zip
                                                             }).ToList();
                                            ra.RAEFINOwnerInfo = ownerlist;
                                            ra.PreviousYearVolume = bankinfo.PriorYearVolume.HasValue ? bankinfo.PriorYearVolume.Value : 0;
                                            ra.ExpectedCurrentYearVolume = bankinfo.CurrentYearVolume.HasValue ? bankinfo.CurrentYearVolume.Value : 0;
                                            ra.PreviousBankName = bankinfo.PriorYearBank;
                                            ra.NoofYearsExperience = bankinfo.ExperienceFiling.HasValue ? bankinfo.ExperienceFiling.Value : 0;
                                        }
                                    }
                                    var efinobj = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                                    ra.OwnerFirstName = efinobj.FName;
                                    ra.OwnerLastName = efinobj.LName;
                                    ra.OwnerSSN = efinobj.SSN;
                                    ra.OwnerTitle = efinobj.Title;
                                    ra.OwnerDOB = efinobj.DOB.HasValue ? efinobj.DOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                                    ra.OwnerEmail = efinobj.Email;
                                    ra.OwnerCellPhone = efinobj.Mobile;
                                    ra.OwnerHomePhone = efinobj.Phone;
                                    ra.OwnerAddress = efinobj.Address;
                                    ra.OwnerCity = efinobj.City;
                                    ra.OwnerState = efinobj.State;
                                    ra.OwnerZipCode = efinobj.Zip;
                                    ra.OwnerStateIssuedIdNumber = efinobj.IDNumber;
                                    ra.OwnerIssuingState = efinobj.IDState;
                                    ra.BusinessEIN = efinobj.EIN;
                                }
                            }

                            return ra;
                        }
                        else
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetRABankEnrollment", Guid.Empty);
                return null;
            }
        }

        public RABankEnrollment getRAData(BankEnrollmentForRA isExist, Guid CustomerId, bool IsStaging, Guid xBankId)
        {
            try
            {
                var IsMainSite = true;
                RABankEnrollment ra = new RABankEnrollment();
                if (isExist.EntryLevel == 4)
                {
                    ra.BankAccountNumber = isExist.BankAccountNumber;
                    ra.BankAccountType = isExist.BankAccountType;
                    ra.BankRoutingNumber = isExist.BankRoutingNumber;
                    ra.BusinessEIN = isExist.BusinessEIN;
                    ra.BusinessFederalIDNumber = isExist.BusinessFederalIDNumber;
                    ra.CollectionofBusinessOwners = isExist.CollectionofBusinessOwners;
                    ra.CollectionOfOtherOwners = isExist.CollectionOfOtherOwners;
                    ra.CorporationType = isExist.CorporationType;
                    ra.EFINOwnersSite = isExist.EFINOwnersSite;
                    ra.EROMailingCity = isExist.EROMailingCity;
                    ra.EROMailingState = isExist.EROMailingState;
                    ra.EROMailingZipcode = isExist.EROMailingZipcode;
                    ra.EROMaillingAddress = isExist.EROMaillingAddress;
                    ra.EROOfficeAddress = isExist.EROOfficeAddress;
                    ra.EROOfficeCity = isExist.EROOfficeCity;
                    ra.EROOfficeName = isExist.EROOfficeName;
                    ra.EROOfficePhone = isExist.EROOfficePhone;
                    ra.EROOfficeState = isExist.EROOfficeState;
                    ra.EROOfficeZipCoce = isExist.EROOfficeZipCoce;
                    ra.EROShippingAddress = isExist.EROShippingAddress;
                    ra.EROShippingCity = isExist.EROShippingCity;
                    ra.EROShippingState = isExist.EROShippingState;
                    ra.EROShippingState = isExist.EROShippingState;
                    ra.EROShippingZip = isExist.EROShippingZip;
                    ra.ExpectedCurrentYearVolume = isExist.ExpectedCurrentYearVolume.HasValue ? isExist.ExpectedCurrentYearVolume.Value : 0;
                    ra.HasAssociatedWithVictims = isExist.HasAssociatedWithVictims;
                    ra.IRSAddress = isExist.IRSAddress;
                    ra.IRSCity = isExist.IRSCity;
                    ra.IRSState = isExist.IRSState;
                    ra.IRSZipcode = isExist.IRSZipcode;
                    ra.IsLastYearClient = isExist.IsLastYearClient;
                    ra.NoofYearsExperience = isExist.NoofYearsExperience.HasValue ? isExist.NoofYearsExperience.Value : 0;
                    ra.OwnerAddress = isExist.OwnerAddress;
                    ra.OwnerCellPhone = isExist.OwnerCellPhone;
                    ra.OwnerCity = isExist.OwnerCity;

                    if (isExist.OwnerDOB != DateTime.MinValue)
                    {
                        ra.OwnerDOB = (isExist.OwnerDOB ?? DateTime.MinValue).ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                    }

                    ra.OwnerEmail = isExist.OwnerEmail;
                    ra.OwnerFirstName = isExist.OwnerFirstName;
                    ra.OwnerHomePhone = isExist.OwnerHomePhone;
                    ra.OwnerIssuingState = isExist.OwnerIssuingState;
                    ra.OwnerLastName = isExist.OwnerLastName;
                    ra.OwnerSSN = isExist.OwnerSSN;
                    ra.OwnerState = isExist.OwnerState;
                    ra.OwnerStateIssuedIdNumber = isExist.OwnerStateIssuedIdNumber;
                    ra.OwnerZipCode = isExist.OwnerZipCode;
                    ra.PreviousBankName = isExist.PreviousBankName;
                    ra.PreviousYearVolume = isExist.PreviousYearVolume.Value;

                    ra.AccountName = isExist.AccountName;
                    ra.AgreeTandC = isExist.AgreeTandC.Value;
                    ra.BankName = isExist.BankName;
                    ra.OwnerTitle = isExist.OwnerTitle;

                    ra.ElectronicFee = isExist.ElectronicFee;
                    ra.MainContactFirstName = isExist.MainContactFirstName;
                    ra.MainContactLastName = isExist.MainContactLastName;
                    ra.MainContactPhone = isExist.MainContactPhone;
                    ra.TextMessages = isExist.TextMessages.HasValue ? isExist.TextMessages.Value : false;
                    ra.LegalIssues = isExist.LegalIssues.HasValue ? isExist.LegalIssues.Value : false;
                    ra.StateOfIncorporation = isExist.StateOfIncorporation;

                    ra.RAEFINOwnerInfo = db.BankEnrollmentEFINOwnersForRAs.Where(o => o.BankEnrollmentRAId == isExist.Id).Select(o => new EnrollmentBankEFINOwnerRADTO()
                    {
                        Id = o.Id,
                        EmailId = o.EmailId,
                        FirstName = o.FirstName,
                        LastName = o.LastName,
                        Address = o.Address,
                        BankEnrollmentRAId = o.BankEnrollmentRAId,
                        City = o.City,
                        StateId = o.StateId,
                        MobilePhone = o.MobilePhone,
                        HomePhone = o.HomePhone,
                        DateofBirth = o.DateofBirth,
                        IDNumber = o.IDNumber,
                        IDState = o.IDState,
                        PercentageOwned = o.PercentageOwned,
                        SSN = o.SSN,
                        ZipCode = o.ZipCode,
                        UpdatedBy = o.UpdatedBy,
                        UpdatedDate = o.UpdatedDate
                    }).ToList();


                    var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                    {
                        IsMainSite = false;
                        var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                        if (SubSiteOffice != null)
                        {
                            info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                        }
                    }

                    decimal addon = 0, sb = 0;
                    var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == xBankId).FirstOrDefault();
                    string addontitle = string.Empty, svbfeetitle = string.Empty;

                    if (bankid != null)
                    {
                        if (bankid.BankId != Guid.Empty)
                        {
                            var addonMasterfees = (from f in db.FeeMasters
                                                   where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                   select f.Amount).FirstOrDefault();
                            addon += addonMasterfees.Value;

                            addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                            var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                                else
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                            }

                            decimal trnasaddon = bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0, svbaddon = bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                            if (IsStaging)
                            {
                                var staging = db.EnrollmentAddonStagings.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == bankid.BankId).FirstOrDefault();
                                if (staging != null)
                                {
                                    trnasaddon = staging.IsTransmissionFee ? staging.TransmissionAddonAmount : 0;
                                    svbaddon = staging.IsSvbFee ? staging.SvbAddonAmount : 0;
                                }
                            }

                            addon += trnasaddon;
                            addontitle = addontitle + "Add On Fee :" + trnasaddon.ToString("0.00");

                            var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                            var mainfees = (from f in db.FeeMasters
                                            where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                            select f.Amount).FirstOrDefault();
                            sb += mainfees.Value;
                            svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                                else
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }

                            }
                            sb += svbaddon;
                            svbfeetitle = svbfeetitle + "Add On Fee :" + svbaddon.ToString("0.00");
                        }
                    }

                    ra.AddonfeeTitle = addontitle;
                    ra.ServiceBureaufeeTitle = svbfeetitle;
                    ra.SbFeeall = isExist.SbFeeall;
                    ra.TransmissionAddon = addon.ToString("0.00");
                    ra.SbFee = sb.ToString("0.00");
                }
                else
                {
                    var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if (info != null)
                    {
                        ra.EROOfficePhone = info.OfficePhone;
                        ra.EROShippingAddress = info.ShippingAddress1;
                        ra.EROShippingCity = info.ShippingCity;
                        ra.EROShippingState = info.ShippingState;
                        ra.EROShippingZip = info.ShippingZipCode;
                        ra.EROOfficeAddress = info.PhysicalAddress1;
                        ra.EROOfficeCity = info.PhysicalCity;
                        ra.EROOfficeState = info.PhysicalState;
                        ra.EROOfficeZipCoce = info.PhysicalZipCode;

                        if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                        {
                            IsMainSite = false;
                            var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                            if (SubSiteOffice != null)
                            {
                                info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                            }
                        }

                        decimal addon = 0, sb = 0;
                        var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == xBankId).FirstOrDefault();
                        string addontitle = string.Empty, svbfeetitle = string.Empty;

                        if (bankid != null)
                        {
                            if (bankid.BankId != Guid.Empty)
                            {
                                var addonMasterfees = (from f in db.FeeMasters
                                                       where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                       select f.Amount).FirstOrDefault();
                                addon += addonMasterfees.Value;

                                addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                                var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                                if (!IsMainSite)
                                {
                                    if ((info.IsMSOUser ?? false) == false)
                                    {
                                        decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                        addon += BankFee;
                                        addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                    }
                                    else
                                    {
                                        decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                        addon += BankFee;
                                        addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                    }
                                }

                                addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                                addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                                var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                                var mainfees = (from f in db.FeeMasters
                                                where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                                select f.Amount).FirstOrDefault();
                                sb += mainfees.Value;
                                svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";

                                if (!IsMainSite)
                                {
                                    if ((info.IsMSOUser ?? false) == false)
                                    {
                                        decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                        sb += BankMaxFees;
                                        svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                    }
                                    else
                                    {
                                        decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                        sb += BankMaxFees;
                                        svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                    }
                                }

                                sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                                svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                            }
                        }

                        ra.TransmissionAddon = addon.ToString("0.00");
                        ra.SbFee = sb.ToString("0.00");
                        ra.AddonfeeTitle = addontitle;
                        ra.ServiceBureaufeeTitle = svbfeetitle;
                    }

                    if (isExist.EntryLevel >= 1)
                    {
                        ra.OwnerFirstName = isExist.OwnerFirstName;
                        ra.OwnerLastName = isExist.OwnerLastName;
                        ra.OwnerTitle = isExist.OwnerTitle;
                        ra.OwnerSSN = isExist.OwnerSSN;
                        if (isExist.OwnerDOB != null)
                            ra.OwnerDOB = Convert.ToDateTime(isExist.OwnerDOB).ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                        ra.OwnerEmail = isExist.OwnerEmail;
                        ra.OwnerCellPhone = isExist.OwnerCellPhone;
                        ra.OwnerHomePhone = isExist.OwnerHomePhone;
                        ra.OwnerAddress = isExist.OwnerAddress;
                        ra.OwnerCity = isExist.OwnerCity;
                        ra.OwnerState = isExist.OwnerState;
                        ra.OwnerZipCode = isExist.OwnerZipCode;
                        ra.OwnerStateIssuedIdNumber = isExist.OwnerStateIssuedIdNumber;
                        ra.OwnerIssuingState = isExist.OwnerIssuingState;
                        ra.OwnerFirstName = info.BusinessOwnerFirstName;
                        ra.OwnerLastName = info.BusinesOwnerLastName;
                        ra.MainContactFirstName = info.BusinessOwnerFirstName;
                        ra.MainContactLastName = info.BusinesOwnerLastName;
                    }
                    if (isExist.EntryLevel >= 2)
                    {
                        ra.EROOfficeName = isExist.EROOfficeName;
                        ra.MainContactFirstName = isExist.MainContactFirstName;
                        ra.MainContactLastName = isExist.MainContactLastName;
                        ra.MainContactPhone = isExist.MainContactPhone;
                        ra.EROOfficeAddress = isExist.EROOfficeAddress;
                        ra.EROOfficeCity = isExist.EROOfficeCity;
                        ra.EROOfficePhone = isExist.EROOfficePhone;
                        ra.EROOfficeState = isExist.EROOfficeState;
                        ra.EROOfficeZipCoce = isExist.EROOfficeZipCoce;
                        ra.EROMaillingAddress = isExist.EROMaillingAddress;
                        ra.EROMailingCity = isExist.EROMailingCity;
                        ra.EROMailingState = isExist.EROMailingState;
                        ra.EROMailingZipcode = isExist.EROMailingZipcode;
                        ra.EROShippingAddress = isExist.EROShippingAddress;
                        ra.EROShippingCity = isExist.EROShippingCity;
                        ra.EROShippingState = isExist.EROShippingState;
                        ra.EROShippingState = isExist.EROShippingState;
                        ra.EROShippingZip = isExist.EROShippingZip;

                        ra.BusinessEIN = isExist.BusinessEIN;
                        ra.CorporationType = isExist.CorporationType;
                        ra.StateOfIncorporation = isExist.StateOfIncorporation;
                        ra.PreviousYearVolume = isExist.PreviousYearVolume.Value;
                        ra.ExpectedCurrentYearVolume = isExist.ExpectedCurrentYearVolume.Value;
                        ra.PreviousBankName = isExist.PreviousBankName;
                        ra.NoofYearsExperience = isExist.NoofYearsExperience.Value;
                        ra.LegalIssues = isExist.LegalIssues ?? false;
                        ra.TextMessages = isExist.TextMessages ?? false;

                        ra.RAEFINOwnerInfo = db.BankEnrollmentEFINOwnersForRAs.Where(o => o.BankEnrollmentRAId == isExist.Id).Select(o => new EnrollmentBankEFINOwnerRADTO()
                        {
                            Id = o.Id,
                            EmailId = o.EmailId,
                            FirstName = o.FirstName,
                            LastName = o.LastName,
                            Address = o.Address,
                            BankEnrollmentRAId = o.BankEnrollmentRAId,
                            City = o.City,
                            StateId = o.StateId,
                            MobilePhone = o.MobilePhone,
                            HomePhone = o.HomePhone,
                            DateofBirth = o.DateofBirth,
                            IDNumber = o.IDNumber,
                            IDState = o.IDState,
                            PercentageOwned = o.PercentageOwned,
                            SSN = o.SSN,
                            ZipCode = o.ZipCode,
                            UpdatedBy = o.UpdatedBy,
                            UpdatedDate = o.UpdatedDate
                        }).ToList();
                    }
                    if (isExist.EntryLevel >= 3)
                    {
                        ra.TransmissionAddon = isExist.TransmissionAddon;
                        ra.SbFee = isExist.SbFee;
                        ra.ElectronicFee = isExist.ElectronicFee;
                        ra.AgreeTandC = isExist.AgreeTandC ?? false;
                    }
                }
                return ra;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public XlinkResponseModel SaveRBBankEnrollment(RBBankEnrollment enrollment)
        {
            DateTime? dd = null;
            try
            {
                var isExist = db.BankEnrollmentForRBs.Where(x => x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    var enrollstatus = db.BankEnrollments.Where(x => x.CustomerId == enrollment.CustomerId && x.IsActive == true && x.BankId == enrollment.BankId).FirstOrDefault();
                    if (enrollstatus != null)
                    {
                        if (enrollstatus.StatusCode == EMPConstants.Approved)
                        {
                            EfinObject _objefin = new CrosslinkService.EfinObject();

                            _objefin.AcctName = enrollment.CheckingAccountName;
                            _objefin.AcctType = enrollment.BankAccountType;
                            _objefin.Address = enrollment.OwnerAddress;
                            _objefin.AgreePEIDate = DateTime.Now;
                            _objefin.AgreePEITerms = true;
                            _objefin.AgreeFeeOption = true;
                            _objefin.BankName = enrollment.BankName;
                            _objefin.City = enrollment.OwnerCity;
                            _objefin.Company = enrollment.OfficeName;
                            _objefin.CreatedDate = DateTime.Now;
                            _objefin.DAN = enrollment.BankAccountNumber;
                            _objefin.DOB = Convert.ToDateTime(enrollment.EFINOwnerDOB);
                            _objefin.EFINType = "S";
                            _objefin.EIN = enrollment.EFINOwnerEIN;
                            _objefin.Email = enrollment.EFINOwnerEmail;
                            _objefin.FivePlus = false;
                            _objefin.FName = enrollment.OwnerFirstName;
                            _objefin.IDNumber = enrollment.EFINOwnerIDNumber;
                            _objefin.IDState = enrollment.EFINOwnerIDState;
                            _objefin.LName = enrollment.OwnerLastName;
                            _objefin.Mobile = string.IsNullOrEmpty(enrollment.EFINOwnerMobile) ? "" : enrollment.EFINOwnerMobile.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            _objefin.Phone = enrollment.EFINOwnerPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                            _objefin.RTN = enrollment.BankRoutingNumber;
                            _objefin.SBFeeAll = enrollment.SBFeeonAll;
                            _objefin.SelectedBank = EMPConstants.RBBank;
                            _objefin.SSN = enrollment.EFINOwnerSSN;
                            _objefin.State = enrollment.OwnerState;
                            _objefin.Title = enrollment.EFINOwnerTitle;
                            _objefin.UpdatedDate = DateTime.Now;
                            _objefin.Zip = enrollment.OwnerZip;

                            var res = UpdateBankEfinObject(_objefin, enrollstatus.BankId.Value, enrollstatus.Id, enrollment.CustomerId);
                            if (!res.Status)
                            {
                                return res;
                            }

                            isExist.IsUpdated = true;
                        }
                    }

                    isExist.ActualNoofBankProductsLastYear = enrollment.ActualNoofBankProductsLastYear;
                    isExist.AdvertisingApproval = enrollment.AdvertisingApproval;
                    isExist.AltOfficeContact1Email = enrollment.AltOfficeContact1Email;
                    isExist.AltOfficeContact1FirstName = enrollment.AltOfficeContact1FirstName;
                    isExist.AltOfficeContact1LastName = enrollment.AltOfficeContact1LastName;
                    isExist.AltOfficeContact1SSn = enrollment.AltOfficeContact1SSn;
                    isExist.AltOfficeContact2Email = enrollment.AltOfficeContact2Email;
                    isExist.AltOfficeContact2FirstName = enrollment.AltOfficeContact2FirstName;
                    isExist.AltOfficeContact2LastName = enrollment.AltOfficeContact2LastName;
                    isExist.AltOfficeContact2SSn = enrollment.AltOfficeContact2SSn;
                    isExist.AltOfficePhysicalAddress = enrollment.AltOfficePhysicalAddress;
                    isExist.AltOfficePhysicalAddress2 = enrollment.AltOfficePhysicalAddress2;
                    isExist.AltOfficePhysicalCity = enrollment.AltOfficePhysicalCity;
                    isExist.AltOfficePhysicalState = enrollment.AltOfficePhysicalState;
                    isExist.AltOfficePhysicalZipcode = enrollment.AltOfficePhysicalZipcode;
                    isExist.AntivirusRequired = enrollment.AntivirusRequired;
                    isExist.BankAccountNumber = enrollment.BankAccountNumber;
                    isExist.BankAccountType = enrollment.BankAccountType;
                    isExist.BankRoutingNumber = enrollment.BankRoutingNumber;
                    isExist.BusinessEIN = enrollment.BusinessEIN;
                    isExist.BusinessIncorporation = string.IsNullOrEmpty(enrollment.BusinessIncorporation) ? dd : DateTime.ParseExact(enrollment.BusinessIncorporation.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.BusinessName = enrollment.BusinessName;
                    isExist.CellPhoneNumber = enrollment.CellPhoneNumber.Replace("-", "");
                    isExist.CheckingAccountName = enrollment.CheckingAccountName;
                    isExist.ConsumerLending = enrollment.ConsumerLending;
                    isExist.EFINOwnerDOB = string.IsNullOrEmpty(enrollment.EFINOwnerDOB) ? dd : DateTime.ParseExact(enrollment.EFINOwnerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.EFINOwnerFirstName = enrollment.EFINOwnerFirstName;
                    isExist.EFINOwnerLastName = enrollment.EFINOwnerLastName;
                    isExist.EFINOwnerSSN = enrollment.EFINOwnerSSN;
                    isExist.EmailAddress = enrollment.EmailAddress;
                    isExist.EROApplicattionDate = string.IsNullOrEmpty(enrollment.EROApplicattionDate) ? dd : DateTime.ParseExact(enrollment.EROApplicattionDate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    isExist.EROParticipation = enrollment.EROParticipation;
                    isExist.EROReadTAndC = enrollment.EROReadTAndC;
                    isExist.FAXNumber = enrollment.FAXNumber;

                    isExist.OfficeContactDOB = string.IsNullOrEmpty(enrollment.OfficeContactDOB) ? dd : DateTime.ParseExact(enrollment.OfficeContactDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.OfficeManagerPhone = enrollment.OfficeManagerPhone;
                    isExist.OfficeManagerCellNo = enrollment.OfficeManagerCellNo;
                    isExist.OfficeManagerEmail = enrollment.OfficeManagerEmail;

                    isExist.FulfillmentShippingAddress = enrollment.FulfillmentShippingAddress;
                    isExist.FulfillmentShippingCity = enrollment.FulfillmentShippingCity;
                    isExist.FulfillmentShippingState = enrollment.FulfillmentShippingState;
                    isExist.FulfillmentShippingZip = enrollment.FulfillmentShippingZip;
                    isExist.HasFirewall = enrollment.HasFirewall;
                    isExist.IsAsPerComplainceLaw = enrollment.IsAsPerComplainceLaw;
                    isExist.IsAsPerProcessLaw = enrollment.IsAsPerProcessLaw;
                    isExist.IsLimitAccess = enrollment.IsLimitAccess;
                    isExist.IsLockedStore_Checks = enrollment.IsLockedStore_Checks;
                    isExist.IsLockedStore_Documents = enrollment.IsLockedStore_Documents;
                    isExist.IsLocked_Office = enrollment.IsLocked_Office;
                    isExist.IsMultiOffice = enrollment.IsMultiOffice;
                    isExist.IsOfficeTransmit = enrollment.IsOfficeTransmit;
                    isExist.IsPTIN = enrollment.IsPTIN;
                    isExist.LegarEntityStatus = enrollment.LegarEntityStatus;
                    isExist.LLCMembershipRegistration = enrollment.LLCMembershipRegistration;
                    isExist.LoginAccesstoEmployees = enrollment.LoginAccesstoEmployees;
                    isExist.MailingAddress = enrollment.MailingAddress;
                    isExist.MailingCity = enrollment.MailingCity;
                    isExist.MailingState = enrollment.MailingState;
                    isExist.MailingZip = enrollment.MailingZip;
                    isExist.NoofBankProductsLastYear = enrollment.NoofBankProductsLastYear;
                    isExist.NoofPersoneel = enrollment.NoofPersoneel;
                    isExist.OfficeContactFirstName = enrollment.OfficeContactFirstName;
                    isExist.OfficeContactLastName = enrollment.OfficeContactLastName;
                    isExist.OfficeContactSSN = enrollment.OfficeContactSSN;
                    isExist.OfficeManageLastName = enrollment.OfficeManageLastName;
                    if (enrollment.OfficeManagerDOB != "")
                        isExist.OfficeManagerDOB = DateTime.ParseExact(enrollment.OfficeManagerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.OfficeManagerFirstName = enrollment.OfficeManagerFirstName;
                    isExist.OfficeManagerSSN = enrollment.OfficeManagerSSN;
                    isExist.OfficeName = enrollment.OfficeName;
                    isExist.OfficePhoneNumber = enrollment.OfficePhoneNumber.Replace("-", "");
                    isExist.OfficePhysicalAddress = enrollment.OfficePhysicalAddress;
                    isExist.OfficePhysicalCity = enrollment.OfficePhysicalCity;
                    isExist.OfficePhysicalState = enrollment.OfficePhysicalState;
                    isExist.OfficePhysicalZip = enrollment.OfficePhysicalZip;
                    isExist.OnlineTraining = enrollment.OnlineTraining;
                    if (enrollment.OnwerDOB != "")
                        isExist.OnwerDOB = DateTime.ParseExact(enrollment.OnwerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.OwnerAddress = enrollment.OwnerAddress;
                    isExist.OwnerCity = enrollment.OwnerCity;
                    isExist.OwnerFirstName = enrollment.OwnerFirstName;
                    isExist.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    isExist.OwnerLastName = enrollment.OwnerLastName;
                    isExist.OwnerSSN = enrollment.OwnerSSN;
                    isExist.OwnerState = enrollment.OwnerState;
                    isExist.OwnerZip = enrollment.OwnerZip;
                    isExist.PasswordRequired = enrollment.PasswordRequired;
                    isExist.PlantoDispose = enrollment.PlantoDispose;
                    isExist.PreviousBankProductFacilitator = enrollment.PreviousBankProductFacilitator;
                    isExist.ProductsOffering = enrollment.ProductsOffering;
                    isExist.RetailPricingMethod = enrollment.RetailPricingMethod;
                    isExist.SPAAmount = enrollment.SPAAmount;
                    isExist.SPADate = string.IsNullOrEmpty(enrollment.SPADate) ? dd : DateTime.ParseExact(enrollment.SPADate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    isExist.WebsiteAddress = enrollment.WebsiteAddress;
                    isExist.YearsinBusiness = enrollment.YearsinBusiness;

                    isExist.EFINOwnerTitle = enrollment.EFINOwnerTitle;
                    isExist.EFINOwnerAddress = enrollment.EFINOwnerAddress;
                    isExist.EFINOwnerCity = enrollment.EFINOwnerCity;
                    isExist.EFINOwnerState = enrollment.EFINOwnerState;
                    isExist.EFINOwnerZip = enrollment.EFINOwnerZip;
                    isExist.EFINOwnerPhone = enrollment.EFINOwnerPhone;
                    isExist.EFINOwnerMobile = enrollment.EFINOwnerMobile;
                    isExist.EFINOwnerEmail = enrollment.EFINOwnerEmail;
                    isExist.EFINOwnerIDNumber = enrollment.EFINOwnerIDNumber;
                    isExist.EFINOwnerIDState = enrollment.EFINOwnerIDState;
                    isExist.EFINOwnerEIN = enrollment.EFINOwnerEIN;
                    isExist.SupportOS = enrollment.SupportOS;
                    isExist.BankName = enrollment.BankName;
                    isExist.SBFeeonAll = enrollment.SBFeeonAll;
                    isExist.SBFee = enrollment.SBFee;
                    isExist.TransimissionAddon = enrollment.TransimissionAddon;
                    isExist.PrepaidCardProgram = enrollment.PrepaidCardProgram;
                    isExist.TandC = enrollment.TandC;
                    isExist.IsEnrtyCompleted = true;
                    isExist.EntryLevel = 7;

                    isExist.UpdatedBy = enrollment.UserId;
                    isExist.UpdatedDate = DateTime.Now;

                    db.SaveChanges();
                }
                else
                {
                    BankEnrollmentForRB rb = new BankEnrollmentForRB();

                    rb.CustomerId = enrollment.CustomerId;
                    rb.Id = Guid.NewGuid();

                    rb.ActualNoofBankProductsLastYear = enrollment.ActualNoofBankProductsLastYear;
                    rb.AdvertisingApproval = enrollment.AdvertisingApproval;
                    rb.AltOfficeContact1Email = enrollment.AltOfficeContact1Email;
                    rb.AltOfficeContact1FirstName = enrollment.AltOfficeContact1FirstName;
                    rb.AltOfficeContact1LastName = enrollment.AltOfficeContact1LastName;
                    rb.AltOfficeContact1SSn = enrollment.AltOfficeContact1SSn;
                    rb.AltOfficeContact2Email = enrollment.AltOfficeContact2Email;
                    rb.AltOfficeContact2FirstName = enrollment.AltOfficeContact2FirstName;
                    rb.AltOfficeContact2LastName = enrollment.AltOfficeContact2LastName;
                    rb.AltOfficeContact2SSn = enrollment.AltOfficeContact2SSn;
                    rb.AltOfficePhysicalAddress = enrollment.AltOfficePhysicalAddress;
                    rb.AltOfficePhysicalAddress2 = enrollment.AltOfficePhysicalAddress2;
                    rb.AltOfficePhysicalCity = enrollment.AltOfficePhysicalCity;
                    rb.AltOfficePhysicalState = enrollment.AltOfficePhysicalState;
                    rb.AltOfficePhysicalZipcode = enrollment.AltOfficePhysicalZipcode;

                    rb.OfficeContactDOB = string.IsNullOrEmpty(enrollment.OfficeContactDOB) ? dd : DateTime.ParseExact(enrollment.OfficeContactDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.OfficeManagerPhone = enrollment.OfficeManagerPhone;
                    rb.OfficeManagerCellNo = enrollment.OfficeManagerCellNo;
                    rb.OfficeManagerEmail = enrollment.OfficeManagerEmail;

                    rb.AntivirusRequired = enrollment.AntivirusRequired;
                    rb.BankAccountNumber = enrollment.BankAccountNumber;
                    rb.BankAccountType = enrollment.BankAccountType;
                    rb.BankRoutingNumber = enrollment.BankRoutingNumber;
                    rb.BusinessEIN = enrollment.BusinessEIN;
                    rb.BusinessIncorporation = string.IsNullOrEmpty(enrollment.BusinessIncorporation) ? dd : DateTime.ParseExact(enrollment.BusinessIncorporation.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.BusinessName = enrollment.BusinessName;
                    rb.CellPhoneNumber = enrollment.CellPhoneNumber.Replace("-", "");
                    rb.CheckingAccountName = enrollment.CheckingAccountName;
                    rb.ConsumerLending = enrollment.ConsumerLending;
                    rb.EFINOwnerDOB = string.IsNullOrEmpty(enrollment.EFINOwnerDOB) ? dd : DateTime.ParseExact(enrollment.EFINOwnerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.EFINOwnerFirstName = enrollment.EFINOwnerFirstName;
                    rb.EFINOwnerLastName = enrollment.EFINOwnerLastName;
                    rb.EFINOwnerSSN = enrollment.EFINOwnerSSN;
                    rb.EmailAddress = enrollment.EmailAddress;
                    rb.EROApplicattionDate = string.IsNullOrEmpty(enrollment.EROApplicattionDate) ? dd : DateTime.ParseExact(enrollment.EROApplicattionDate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    rb.EROParticipation = enrollment.EROParticipation;
                    rb.EROReadTAndC = enrollment.EROReadTAndC;
                    rb.FAXNumber = enrollment.FAXNumber;
                    rb.FulfillmentShippingAddress = enrollment.FulfillmentShippingAddress;
                    rb.FulfillmentShippingCity = enrollment.FulfillmentShippingCity;
                    rb.FulfillmentShippingState = enrollment.FulfillmentShippingState;
                    rb.FulfillmentShippingZip = enrollment.FulfillmentShippingZip;
                    rb.HasFirewall = enrollment.HasFirewall;
                    rb.IsAsPerComplainceLaw = enrollment.IsAsPerComplainceLaw;
                    rb.IsAsPerProcessLaw = enrollment.IsAsPerProcessLaw;
                    rb.IsLimitAccess = enrollment.IsLimitAccess;
                    rb.IsLockedStore_Checks = enrollment.IsLockedStore_Checks;
                    rb.IsLockedStore_Documents = enrollment.IsLockedStore_Documents;
                    rb.IsLocked_Office = enrollment.IsLocked_Office;
                    rb.IsMultiOffice = enrollment.IsMultiOffice;
                    rb.IsOfficeTransmit = enrollment.IsOfficeTransmit;
                    rb.IsPTIN = enrollment.IsPTIN;
                    rb.LegarEntityStatus = enrollment.LegarEntityStatus;
                    rb.LLCMembershipRegistration = enrollment.LLCMembershipRegistration;
                    rb.LoginAccesstoEmployees = enrollment.LoginAccesstoEmployees;
                    rb.MailingAddress = enrollment.MailingAddress;
                    rb.MailingCity = enrollment.MailingCity;
                    rb.MailingState = enrollment.MailingState;
                    rb.MailingZip = enrollment.MailingZip;
                    rb.NoofBankProductsLastYear = enrollment.NoofBankProductsLastYear;
                    rb.NoofPersoneel = enrollment.NoofPersoneel;
                    rb.OfficeContactFirstName = enrollment.OfficeContactFirstName;
                    rb.OfficeContactLastName = enrollment.OfficeContactLastName;
                    rb.OfficeContactSSN = enrollment.OfficeContactSSN;
                    rb.OfficeManageLastName = enrollment.OfficeManageLastName;
                    if (enrollment.OfficeManagerDOB != "")
                        rb.OfficeManagerDOB = DateTime.ParseExact(enrollment.OfficeManagerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.OfficeManagerFirstName = enrollment.OfficeManagerFirstName;
                    rb.OfficeManagerSSN = enrollment.OfficeManagerSSN;
                    rb.OfficeName = enrollment.OfficeName;
                    rb.OfficePhoneNumber = enrollment.OfficePhoneNumber.Replace("-", "");
                    rb.OfficePhysicalAddress = enrollment.OfficePhysicalAddress;
                    rb.OfficePhysicalCity = enrollment.OfficePhysicalCity;
                    rb.OfficePhysicalState = enrollment.OfficePhysicalState;
                    rb.OfficePhysicalZip = enrollment.OfficePhysicalZip;
                    rb.OnlineTraining = enrollment.OnlineTraining;
                    if (enrollment.OnwerDOB != "")
                        rb.OnwerDOB = DateTime.ParseExact(enrollment.OnwerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.OwnerAddress = enrollment.OwnerAddress;
                    rb.OwnerCity = enrollment.OwnerCity;
                    rb.OwnerFirstName = enrollment.OwnerFirstName;
                    rb.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    rb.OwnerLastName = enrollment.OwnerLastName;
                    rb.OwnerSSN = enrollment.OwnerSSN;
                    rb.OwnerState = enrollment.OwnerState;
                    rb.OwnerZip = enrollment.OwnerZip;
                    rb.PasswordRequired = enrollment.PasswordRequired;
                    rb.PlantoDispose = enrollment.PlantoDispose;
                    rb.PreviousBankProductFacilitator = enrollment.PreviousBankProductFacilitator;
                    rb.ProductsOffering = enrollment.ProductsOffering;
                    rb.RetailPricingMethod = enrollment.RetailPricingMethod;
                    rb.SPAAmount = enrollment.SPAAmount;
                    rb.SPADate = string.IsNullOrEmpty(enrollment.SPADate) ? dd : DateTime.ParseExact(enrollment.SPADate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    rb.WebsiteAddress = enrollment.WebsiteAddress;
                    rb.YearsinBusiness = enrollment.YearsinBusiness;


                    rb.EFINOwnerTitle = enrollment.EFINOwnerTitle;
                    rb.EFINOwnerAddress = enrollment.EFINOwnerAddress;
                    rb.EFINOwnerCity = enrollment.EFINOwnerCity;
                    rb.EFINOwnerState = enrollment.EFINOwnerState;
                    rb.EFINOwnerZip = enrollment.EFINOwnerZip;
                    rb.EFINOwnerPhone = enrollment.EFINOwnerPhone;
                    rb.EFINOwnerMobile = enrollment.EFINOwnerMobile;
                    rb.EFINOwnerEmail = enrollment.EFINOwnerEmail;
                    rb.EFINOwnerIDNumber = enrollment.EFINOwnerIDNumber;
                    rb.EFINOwnerIDState = enrollment.EFINOwnerIDState;
                    rb.EFINOwnerEIN = enrollment.EFINOwnerEIN;
                    rb.SupportOS = enrollment.SupportOS;
                    rb.BankName = enrollment.BankName;
                    rb.SBFeeonAll = enrollment.SBFeeonAll;
                    rb.SBFee = enrollment.SBFee;
                    rb.TransimissionAddon = enrollment.TransimissionAddon;
                    rb.PrepaidCardProgram = enrollment.PrepaidCardProgram;
                    rb.TandC = enrollment.TandC;

                    isExist.IsEnrtyCompleted = true;
                    isExist.EntryLevel = 7;

                    rb.CreatedBy = enrollment.UserId;
                    rb.CreatedDate = DateTime.Now;
                    rb.UpdatedBy = enrollment.UserId;
                    rb.UpdatedDate = DateTime.Now;
                    rb.StatusCode = EMPConstants.Active;

                    db.BankEnrollmentForRBs.Add(rb);
                    db.SaveChanges();
                }

                var isaprvd = db.EnrollmentBankSelections.Where(x => x.BankId == enrollment.BankId && x.BankSubmissionStatus == 1 && x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isaprvd != null)
                {
                    saveEnrollmenttoService(enrollment.CustomerId, enrollment.UserId ?? Guid.Empty, enrollment.BankId, Guid.Empty);
                    UpdateDefaultBankStatus(enrollment.CustomerId, enrollment.UserId ?? Guid.Empty, enrollment.BankId, true);
                }

                //saveEnrollmentCompletedStatus(enrollment.UserId ?? Guid.Empty, enrollment.CustomerId);
                return new XlinkResponseModel() { Status = true };
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveRBBankEnrollment", Guid.Empty);
                return new XlinkResponseModel() { Status = false };
            }
        }

        public RBBankEnrollment GetRBBankEnrollment(Guid CustomerId, bool IsStaging, Guid xBankid)
        {
            var IsMainSite = true;
            RBBankEnrollment rb = new DTO.RBBankEnrollment();
            try
            {
                var isExist = db.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    return getRBData(isExist, CustomerId, IsStaging, xBankid);
                }
                else
                {
                    var isoldExist = db.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.InActive).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (isoldExist != null)
                    {
                        return getRBData(isoldExist, CustomerId, IsStaging, xBankid);
                    }
                    else
                    {
                        var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                        if (info != null)
                        {
                            rb.OfficeName = info.CompanyName;
                            rb.OwnerFirstName = info.BusinessOwnerFirstName;
                            rb.OwnerLastName = info.BusinesOwnerLastName;
                            rb.OfficeContactFirstName = info.BusinessOwnerFirstName;
                            rb.OfficeContactLastName = info.BusinesOwnerLastName;
                            rb.OfficePhoneNumber = info.OfficePhone;
                            rb.FulfillmentShippingAddress = info.ShippingAddress1;
                            rb.FulfillmentShippingCity = info.ShippingCity;
                            rb.FulfillmentShippingState = info.ShippingState;
                            rb.FulfillmentShippingZip = info.ShippingZipCode;
                            rb.OfficePhysicalAddress = info.PhysicalAddress1;
                            rb.OfficePhysicalCity = info.PhysicalCity;
                            rb.OfficePhysicalState = info.PhysicalState;
                            rb.OfficePhysicalZip = info.PhysicalZipCode;
                            rb.AltOfficePhysicalAddress = info.PhysicalAddress1;
                            rb.AltOfficePhysicalCity = info.PhysicalCity;
                            rb.AltOfficePhysicalState = info.PhysicalState;
                            rb.AltOfficePhysicalZipcode = info.PhysicalZipCode;
                            rb.MailingAddress = info.PhysicalAddress1;
                            rb.MailingCity = info.PhysicalCity;
                            rb.MailingState = info.PhysicalState;
                            rb.MailingZip = info.PhysicalZipCode;
                            rb.OwnerAddress = info.PhysicalAddress1;
                            rb.OwnerCity = info.PhysicalCity;
                            rb.OwnerState = info.PhysicalState;
                            rb.OwnerZip = info.PhysicalZipCode;
                            rb.OwnerHomePhone = info.OfficePhone;
                            rb.EmailAddress = info.PrimaryEmail;

                            //var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                            //decimal addon = 0, sb = 0;
                            //if (info.ParentId != null)
                            //{
                            //    var fees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.ParentId.Value).ToList();
                            //    addon = fees.Sum(x => x.BankMaxFees);
                            //}
                            //var subaddon = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.ServiceBureauBankAmount).FirstOrDefault();
                            //addon += subaddon.HasValue ? subaddon.Value : 0;


                            //var mainfees = (from f in db.FeeMasters
                            //                where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                            //                select f.Amount).FirstOrDefault();
                            //sb += mainfees.Value;
                            //if (info.IsMSOUser ?? false)
                            //{
                            //    var customerfees = (from s in db.SubSiteBankFeesConfigs
                            //                        where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == bankid.BankId && s.ServiceOrTransmitter == 1
                            //                        select s.BankMaxFees_MSO).FirstOrDefault();
                            //    sb += customerfees.Value;
                            //}
                            //else
                            //{
                            //    var customerfees = (from s in db.SubSiteBankFeesConfigs
                            //                        where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == bankid.BankId && s.ServiceOrTransmitter == 1
                            //                        select s.BankMaxFees).FirstOrDefault();
                            //    sb += customerfees;
                            //}

                            //if (info.ParentId != null)
                            //{
                            //    var fees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.ParentId.Value && x.ServiceOrTransmitter == 1).FirstOrDefault();
                            //    if (info.IsMSOUser ?? false)
                            //    {
                            //        sb += fees.BankMaxFees_MSO.Value;
                            //    }
                            //    else
                            //        sb += fees.BankMaxFees;
                            //}
                            //sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;

                            if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                            {
                                IsMainSite = false;
                                var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                                if (SubSiteOffice != null)
                                {
                                    info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                                }
                            }

                            decimal addon = 0, sb = 0;
                            var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.BankId == xBankid && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                            string addontitle = string.Empty, svbfeetitle = string.Empty;

                            if (bankid != null)
                            {
                                if (bankid.BankId != Guid.Empty)
                                {
                                    var addonMasterfees = (from f in db.FeeMasters
                                                           where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                           select f.Amount).FirstOrDefault();
                                    addon += addonMasterfees.Value;

                                    addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                                    var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();
                                    if (!IsMainSite)
                                    {
                                        if ((info.IsMSOUser ?? false) == false)
                                        {
                                            decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                            addon += BankFee;
                                            addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                        }
                                        else
                                        {
                                            decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                            addon += BankFee;
                                            addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                        }
                                    }

                                    addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                                    addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                                    var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                                    var mainfees = (from f in db.FeeMasters
                                                    where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                                    select f.Amount).FirstOrDefault();
                                    sb += mainfees.Value;
                                    svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";

                                    if (!IsMainSite)
                                    {
                                        if ((info.IsMSOUser ?? false) == false)
                                        {
                                            decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                            sb += BankMaxFees;
                                            svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                        }
                                        else
                                        {
                                            decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                            sb += BankMaxFees;
                                            svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                        }
                                    }

                                    sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                                    svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                                }
                            }

                            rb.AddonfeeTitle = addontitle;
                            rb.ServiceBureaufeeTitle = svbfeetitle;

                            rb.TransimissionAddon = addon.ToString("0.00");
                            rb.SBFee = sb.ToString("0.00");


                            var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                            //string CLPassword = PasswordManager.DecryptText(logininfo.CrossLinkPassword);

                            string MasterId = logininfo.MasterIdentifier;
                            string Password = "";
                            int CrossLinkUserId = 0;
                            getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, logininfo.MasterIdentifier, info.ParentId ?? Guid.Empty, info.EntityId ?? 0);

                            string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                            if (_accesskey != "")
                            {
                                AuthObject _objAuth = new AuthObject();
                                _objAuth.AccessKey = _accesskey;
                                _objAuth.UserID = MasterId;
                                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                                if (isValid.success)
                                {
                                    int efinid = _apiObj.getEFINID(info.EFIN.Value);
                                    var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, "R");
                                    if (latestbankApp.BankAppID != 0)
                                    {
                                        var bankinfo = _apiObj.getRepublicApp(_objAuth, latestbankApp.BankAppID);
                                        if (bankinfo.Response.success)
                                        {
                                            rb.OfficeContactFirstName = bankinfo.OfficeContactFirstName;
                                            rb.OfficeContactLastName = bankinfo.OfficeContactLastName;
                                            rb.OfficeContactSSN = bankinfo.OfficeContactSSN;
                                            rb.CellPhoneNumber = bankinfo.CellPhoneNumber;
                                            rb.OfficeManagerFirstName = bankinfo.OfficeManagerFirstName;
                                            rb.OfficeManageLastName = bankinfo.OfficeManagerLastName;
                                            rb.OfficeManagerDOB = bankinfo.OfficeManagerDOB.HasValue ? bankinfo.OfficeManagerDOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                                            rb.OfficeManagerSSN = bankinfo.OfficeManagerSSN;
                                            rb.MailingAddress = bankinfo.MailingAddress;
                                            rb.MailingCity = bankinfo.MailingCity;
                                            rb.MailingState = bankinfo.MailingState;
                                            rb.MailingZip = bankinfo.MailingZip;
                                            rb.WebsiteAddress = bankinfo.WebsiteAddress;
                                            rb.YearsinBusiness = bankinfo.YearsInBusiness.HasValue ? bankinfo.YearsInBusiness.Value : 0;
                                            rb.NoofBankProductsLastYear = bankinfo.LastYearBankProducts.HasValue ? bankinfo.LastYearBankProducts.Value : 0;
                                            rb.PreviousBankProductFacilitator = bankinfo.BankProductFacilitator;
                                            rb.OwnerFirstName = bankinfo.OwnerFirstName;
                                            rb.OwnerLastName = bankinfo.OwnerLastName;
                                            rb.OwnerHomePhone = bankinfo.OwnerHomePhone;
                                            rb.OwnerSSN = bankinfo.OwnerSSN;
                                            rb.OwnerState = bankinfo.OwnerState;
                                            rb.OnwerDOB = bankinfo.OwnerDOB.HasValue ? bankinfo.OwnerDOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                                            rb.OwnerAddress = bankinfo.OwnerAddress;
                                            rb.OwnerCity = bankinfo.OwnerCity;
                                            rb.OwnerZip = bankinfo.OwnerZip;
                                            rb.LegarEntityStatus = bankinfo.LegalEntityStatusInd;
                                            rb.LLCMembershipRegistration = bankinfo.LLCMembershipRegistration;
                                            rb.BusinessEIN = bankinfo.EIN;

                                        }
                                    }
                                    var efinobj = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                                    rb.EFINOwnerFirstName = efinobj.FName;
                                    rb.EFINOwnerLastName = efinobj.LName;
                                    rb.EFINOwnerTitle = efinobj.Title;
                                    rb.EFINOwnerSSN = efinobj.SSN;
                                    rb.EFINOwnerAddress = efinobj.Address;
                                    rb.EFINOwnerCity = efinobj.City;
                                    rb.EFINOwnerDOB = efinobj.DOB.HasValue ? efinobj.DOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                                    rb.EFINOwnerEIN = efinobj.EIN;
                                    rb.EFINOwnerEmail = efinobj.Email;
                                    rb.EFINOwnerIDState = efinobj.IDState;
                                    rb.EFINOwnerMobile = efinobj.Mobile;
                                    rb.EFINOwnerPhone = efinobj.Phone;
                                    rb.EFINOwnerIDState = efinobj.IDState;
                                    rb.EFINOwnerState = efinobj.State;
                                    rb.EFINOwnerZip = efinobj.Zip;
                                }
                            }
                            return rb;
                        }
                        else
                            return null;
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetRBBankEnrollment", CustomerId);
                return null;
            }
        }

        public RBBankEnrollment getRBData(BankEnrollmentForRB isExist, Guid CustomerId, bool IsStaging, Guid xBankid)
        {
            var IsMainSite = true;
            RBBankEnrollment rb = new DTO.RBBankEnrollment();
            try
            {
                if (isExist.EntryLevel == 7)
                {
                    rb.ActualNoofBankProductsLastYear = isExist.ActualNoofBankProductsLastYear.Value;
                    rb.AdvertisingApproval = isExist.AdvertisingApproval;
                    rb.AltOfficeContact1Email = isExist.AltOfficeContact1Email;
                    rb.AltOfficeContact1FirstName = isExist.AltOfficeContact1FirstName;
                    rb.AltOfficeContact1LastName = isExist.AltOfficeContact1LastName;
                    rb.AltOfficeContact1SSn = isExist.AltOfficeContact1SSn;
                    rb.AltOfficeContact2Email = isExist.AltOfficeContact2Email;
                    rb.AltOfficeContact2FirstName = isExist.AltOfficeContact2FirstName;
                    rb.AltOfficeContact2LastName = isExist.AltOfficeContact2LastName;
                    rb.AltOfficeContact2SSn = isExist.AltOfficeContact2SSn;
                    rb.AltOfficePhysicalAddress = isExist.AltOfficePhysicalAddress;
                    rb.AltOfficePhysicalAddress2 = isExist.AltOfficePhysicalAddress2;
                    rb.AltOfficePhysicalCity = isExist.AltOfficePhysicalCity;
                    rb.AltOfficePhysicalState = isExist.AltOfficePhysicalState;
                    rb.AltOfficePhysicalZipcode = isExist.AltOfficePhysicalZipcode;
                    rb.AntivirusRequired = isExist.AntivirusRequired;
                    rb.BankAccountNumber = isExist.BankAccountNumber;
                    rb.BankAccountType = isExist.BankAccountType;
                    rb.BankRoutingNumber = isExist.BankRoutingNumber;
                    rb.BusinessEIN = isExist.BusinessEIN;
                    rb.BusinessIncorporation = isExist.BusinessIncorporation.HasValue ? isExist.BusinessIncorporation.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/") : "";
                    rb.BusinessName = isExist.BusinessName;
                    rb.CellPhoneNumber = isExist.CellPhoneNumber.Replace("-", "");
                    rb.CheckingAccountName = isExist.CheckingAccountName;
                    rb.ConsumerLending = isExist.ConsumerLending;
                    rb.EFINOwnerDOB = isExist.EFINOwnerDOB.HasValue ? isExist.EFINOwnerDOB.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/") : "";
                    rb.EFINOwnerFirstName = isExist.EFINOwnerFirstName;
                    rb.EFINOwnerLastName = isExist.EFINOwnerLastName;
                    rb.EFINOwnerSSN = isExist.EFINOwnerSSN;
                    rb.EmailAddress = isExist.EmailAddress;
                    rb.EROApplicattionDate = isExist.EROApplicattionDate.HasValue ? isExist.EROApplicattionDate.Value.ToString("MM'/'dd'/'yyyy hh:mm tt").Replace("-", "/") : "";
                    rb.EROParticipation = isExist.EROParticipation;
                    rb.EROReadTAndC = isExist.EROReadTAndC;
                    rb.FAXNumber = isExist.FAXNumber;
                    rb.FulfillmentShippingAddress = isExist.FulfillmentShippingAddress;
                    rb.FulfillmentShippingCity = isExist.FulfillmentShippingCity;
                    rb.FulfillmentShippingState = isExist.FulfillmentShippingState;
                    rb.FulfillmentShippingZip = isExist.FulfillmentShippingZip;
                    rb.HasFirewall = isExist.HasFirewall;
                    rb.IsAsPerComplainceLaw = isExist.IsAsPerComplainceLaw;
                    rb.IsAsPerProcessLaw = isExist.IsAsPerProcessLaw;
                    rb.IsLimitAccess = isExist.IsLimitAccess;
                    rb.IsLockedStore_Checks = isExist.IsLockedStore_Checks;
                    rb.IsLockedStore_Documents = isExist.IsLockedStore_Documents;
                    rb.IsLocked_Office = isExist.IsLocked_Office;
                    rb.IsMultiOffice = isExist.IsMultiOffice;
                    rb.IsOfficeTransmit = isExist.IsOfficeTransmit;
                    rb.IsPTIN = isExist.IsPTIN;
                    rb.LegarEntityStatus = isExist.LegarEntityStatus;
                    rb.LLCMembershipRegistration = isExist.LLCMembershipRegistration;
                    rb.LoginAccesstoEmployees = isExist.LoginAccesstoEmployees;
                    rb.MailingAddress = isExist.MailingAddress;
                    rb.MailingCity = isExist.MailingCity;
                    rb.MailingState = isExist.MailingState;
                    rb.MailingZip = isExist.MailingZip;
                    rb.NoofBankProductsLastYear = isExist.NoofBankProductsLastYear.Value;
                    rb.NoofPersoneel = isExist.NoofPersoneel.Value;
                    rb.OfficeContactFirstName = isExist.OfficeContactFirstName;
                    rb.OfficeContactLastName = isExist.OfficeContactLastName;
                    rb.OfficeContactSSN = isExist.OfficeContactSSN;
                    rb.OfficeManageLastName = isExist.OfficeManageLastName;
                    if (isExist.OfficeContactDOB != null)
                        rb.OfficeContactDOB = isExist.OfficeContactDOB.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                    rb.OfficeManagerPhone = isExist.OfficeManagerPhone;
                    rb.OfficeManagerCellNo = isExist.OfficeManagerCellNo;
                    rb.OfficeManagerEmail = isExist.OfficeManagerEmail;

                    rb.OfficeManagerDOB = isExist.OfficeManagerDOB.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                    rb.OfficeManagerFirstName = isExist.OfficeManagerFirstName;
                    rb.OfficeManagerSSN = isExist.OfficeManagerSSN;
                    rb.OfficeName = isExist.OfficeName;
                    rb.OfficePhoneNumber = isExist.OfficePhoneNumber.Replace("-", "");
                    rb.OfficePhysicalAddress = isExist.OfficePhysicalAddress;
                    rb.OfficePhysicalCity = isExist.OfficePhysicalCity;
                    rb.OfficePhysicalState = isExist.OfficePhysicalState;
                    rb.OfficePhysicalZip = isExist.OfficePhysicalZip;
                    rb.OnlineTraining = isExist.OnlineTraining;
                    rb.OnwerDOB = isExist.OnwerDOB.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                    rb.OwnerAddress = isExist.OwnerAddress;
                    rb.OwnerCity = isExist.OwnerCity;
                    rb.OwnerFirstName = isExist.OwnerFirstName;
                    rb.OwnerHomePhone = isExist.OwnerHomePhone.Replace("-", "");
                    rb.OwnerLastName = isExist.OwnerLastName;
                    rb.OwnerSSN = isExist.OwnerSSN;
                    rb.OwnerState = isExist.OwnerState;
                    rb.OwnerZip = isExist.OwnerZip;
                    rb.PasswordRequired = isExist.PasswordRequired;
                    rb.PlantoDispose = isExist.PlantoDispose;
                    rb.PreviousBankProductFacilitator = isExist.PreviousBankProductFacilitator;
                    rb.ProductsOffering = isExist.ProductsOffering.Value;
                    rb.RetailPricingMethod = isExist.RetailPricingMethod;
                    rb.SPAAmount = isExist.SPAAmount.Value;
                    rb.SPADate = isExist.SPADate.HasValue ? isExist.SPADate.Value.ToString("MM'/'dd'/'yyyy hh:mm tt").Replace("-", "/") : "";
                    rb.WebsiteAddress = isExist.WebsiteAddress;
                    rb.YearsinBusiness = isExist.YearsinBusiness.Value;
                    rb.EFINOwnerTitle = isExist.EFINOwnerTitle;
                    rb.EFINOwnerAddress = isExist.EFINOwnerAddress;
                    rb.EFINOwnerCity = isExist.EFINOwnerCity;
                    rb.EFINOwnerState = isExist.EFINOwnerState;
                    rb.EFINOwnerZip = isExist.EFINOwnerZip;
                    rb.EFINOwnerPhone = isExist.EFINOwnerPhone;
                    rb.EFINOwnerMobile = isExist.EFINOwnerMobile;
                    rb.EFINOwnerEmail = isExist.EFINOwnerEmail;
                    rb.EFINOwnerIDNumber = isExist.EFINOwnerIDNumber;
                    rb.EFINOwnerIDState = isExist.EFINOwnerIDState;
                    rb.EFINOwnerEIN = isExist.EFINOwnerEIN;
                    rb.SupportOS = isExist.SupportOS;
                    rb.BankName = isExist.BankName;
                    rb.SBFeeonAll = isExist.SBFeeonAll;
                    //rb.SBFee = isExist.SBFee;
                    //rb.TransimissionAddon = isExist.TransimissionAddon;
                    rb.PrepaidCardProgram = isExist.PrepaidCardProgram;
                    rb.TandC = isExist.TandC.HasValue ? isExist.TandC.Value : false;


                    var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                    {
                        IsMainSite = false;
                        var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                        if (SubSiteOffice != null)
                        {
                            info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                        }
                    }

                    decimal addon = 0, sb = 0;
                    var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == xBankid).FirstOrDefault();
                    string addontitle = string.Empty, svbfeetitle = string.Empty;

                    if (bankid != null)
                    {
                        if (bankid.BankId != Guid.Empty)
                        {
                            var addonMasterfees = (from f in db.FeeMasters
                                                   where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                   select f.Amount).FirstOrDefault();
                            addon += addonMasterfees.Value;

                            addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                            var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                                else
                                {
                                    decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    addon += BankFee;
                                    addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                }
                            }

                            decimal trnasaddon = bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0, svbaddon = bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                            if (IsStaging)
                            {
                                var staging = db.EnrollmentAddonStagings.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == bankid.BankId).FirstOrDefault();
                                if (staging != null)
                                {
                                    trnasaddon = staging.IsTransmissionFee ? staging.TransmissionAddonAmount : 0;
                                    svbaddon = staging.IsSvbFee ? staging.SvbAddonAmount : 0;
                                }
                            }

                            addon += trnasaddon;
                            addontitle = addontitle + "Add On Fee :" + trnasaddon.ToString("0.00");

                            var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                            var mainfees = (from f in db.FeeMasters
                                            where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                            select f.Amount).FirstOrDefault();
                            sb += mainfees.Value;
                            svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                            if (!IsMainSite)
                            {
                                if ((info.IsMSOUser ?? false) == false)
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }
                                else
                                {
                                    decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                    sb += BankMaxFees;
                                    svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                }

                            }
                            sb += svbaddon;
                            svbfeetitle = svbfeetitle + "Add On Fee :" + svbaddon.ToString("0.00");
                        }
                    }

                    rb.AddonfeeTitle = addontitle;
                    rb.ServiceBureaufeeTitle = svbfeetitle;

                    rb.TransimissionAddon = addon.ToString("0.00");
                    rb.SBFee = sb.ToString("0.00");

                }
                else
                {
                    var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                    if (info != null)
                    {
                        rb.OfficeName = info.CompanyName;
                        rb.OwnerFirstName = info.BusinessOwnerFirstName;
                        rb.OwnerLastName = info.BusinesOwnerLastName;
                        rb.OfficePhoneNumber = info.OfficePhone;
                        rb.FulfillmentShippingAddress = info.ShippingAddress1;
                        rb.FulfillmentShippingCity = info.ShippingCity;
                        rb.FulfillmentShippingState = info.ShippingState;
                        rb.FulfillmentShippingZip = info.ShippingZipCode;
                        rb.OfficePhysicalAddress = info.PhysicalAddress1;
                        rb.OfficePhysicalCity = info.PhysicalCity;
                        rb.OfficePhysicalState = info.PhysicalState;
                        rb.OfficePhysicalZip = info.PhysicalZipCode;
                        rb.AltOfficePhysicalAddress = info.PhysicalAddress1;
                        rb.AltOfficePhysicalCity = info.PhysicalCity;
                        rb.AltOfficePhysicalState = info.PhysicalState;
                        rb.AltOfficePhysicalZipcode = info.PhysicalZipCode;
                        rb.MailingAddress = info.PhysicalAddress1;
                        rb.MailingCity = info.PhysicalCity;
                        rb.MailingState = info.PhysicalState;
                        rb.MailingZip = info.PhysicalZipCode;
                        rb.OwnerAddress = info.PhysicalAddress1;
                        rb.OwnerCity = info.PhysicalCity;
                        rb.OwnerState = info.PhysicalState;
                        rb.OwnerZip = info.PhysicalZipCode;
                        rb.OwnerHomePhone = info.OfficePhone;
                        rb.EmailAddress = info.PrimaryEmail;

                        //var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        //decimal addon = 0, sb = 0;
                        //if (info.ParentId != null)
                        //{
                        //    var fees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.ParentId.Value).ToList();
                        //    addon = fees.Sum(x => x.BankMaxFees);
                        //}
                        //var subaddon = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.ServiceBureauBankAmount).FirstOrDefault();
                        //addon += subaddon.HasValue ? subaddon.Value : 0;


                        //var mainfees = (from f in db.FeeMasters
                        //                where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                        //                select f.Amount).FirstOrDefault();
                        //sb += mainfees.Value;
                        //if (info.IsMSOUser ?? false)
                        //{
                        //    var customerfees = (from s in db.SubSiteBankFeesConfigs
                        //                        where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == bankid.BankId && s.ServiceOrTransmitter == 1
                        //                        select s.BankMaxFees_MSO).FirstOrDefault();
                        //    sb += customerfees.Value;
                        //}
                        //else
                        //{
                        //    var customerfees = (from s in db.SubSiteBankFeesConfigs
                        //                        where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == bankid.BankId && s.ServiceOrTransmitter == 1
                        //                        select s.BankMaxFees).FirstOrDefault();
                        //    sb += customerfees;
                        //}

                        //if (info.ParentId != null)
                        //{
                        //    var fees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.ParentId.Value && x.ServiceOrTransmitter == 1).FirstOrDefault();
                        //    if (info.IsMSOUser ?? false)
                        //    {
                        //        sb += fees.BankMaxFees_MSO.Value;
                        //    }
                        //    else
                        //        sb += fees.BankMaxFees;
                        //}
                        //sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;

                        if ((info.ParentId ?? Guid.Empty) != Guid.Empty)
                        {
                            IsMainSite = false;
                            var SubSiteOffice = db.SubSiteOfficeConfigs.Where(o => o.RefId == info.Id).FirstOrDefault();
                            if (SubSiteOffice != null)
                            {
                                info.IsMSOUser = SubSiteOffice.SiteanMSOLocation;
                            }
                        }

                        decimal addon = 0, sb = 0;
                        var bankid = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == xBankid).FirstOrDefault();
                        string addontitle = string.Empty, svbfeetitle = string.Empty;

                        if (bankid != null)
                        {
                            if (bankid.BankId != Guid.Empty)
                            {
                                var addonMasterfees = (from f in db.FeeMasters
                                                       where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                                       select f.Amount).FirstOrDefault();
                                addon += addonMasterfees.Value;

                                addontitle = addontitle + "Cross Link Transmitter Fee :" + addonMasterfees.Value + ", ";

                                var xmissionAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 2 && x.BankMaster_ID == bankid.BankId).ToList();

                                if (!IsMainSite)
                                {
                                    if ((info.IsMSOUser ?? false) == false)
                                    {
                                        decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees);
                                        addon += BankFee;
                                        addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                    }
                                    else
                                    {
                                        decimal BankFee = xmissionAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                        addon += BankFee;
                                        addontitle = addontitle + "Transmission Fee :" + BankFee + ", ";
                                    }
                                }

                                addon += bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0;
                                addontitle = addontitle + "Add On Fee :" + (bankid.TransmissionBankAmount.HasValue ? bankid.TransmissionBankAmount.Value : 0);

                                var SvbAddownfees = db.SubSiteBankFeesConfigs.Where(x => x.emp_CustomerInformation_ID == info.Id && x.ServiceOrTransmitter == 1 && x.BankMaster_ID == bankid.BankId).ToList();

                                var mainfees = (from f in db.FeeMasters
                                                where f.FeesFor == 2 && f.StatusCode == EMPConstants.Active
                                                select f.Amount).FirstOrDefault();
                                sb += mainfees.Value;
                                svbfeetitle = svbfeetitle + "uTax Service Fee :" + mainfees + ", ";
                                if (!IsMainSite)
                                {
                                    if ((info.IsMSOUser ?? false) == false)
                                    {
                                        decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees);
                                        sb += BankMaxFees;
                                        svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                    }
                                    else
                                    {
                                        decimal BankMaxFees = SvbAddownfees.Sum(x => x.BankMaxFees_MSO ?? 0);
                                        sb += BankMaxFees;
                                        svbfeetitle = svbfeetitle + "SVB Fee :" + BankMaxFees + ", ";
                                    }

                                }
                                sb += bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0;
                                svbfeetitle = svbfeetitle + "Add On Fee :" + (bankid.ServiceBureauBankAmount.HasValue ? bankid.ServiceBureauBankAmount.Value : 0);
                            }
                        }

                        rb.AddonfeeTitle = addontitle;
                        rb.ServiceBureaufeeTitle = svbfeetitle;

                        rb.TransimissionAddon = addon.ToString("0.00");
                        rb.SBFee = sb.ToString("0.00");
                    }

                    if (isExist.EntryLevel >= 1)
                    {
                        rb.OfficeName = isExist.OfficeName;
                        rb.OfficePhysicalAddress = isExist.OfficePhysicalAddress;
                        rb.OfficePhysicalCity = isExist.OfficePhysicalCity;
                        rb.OfficePhysicalState = isExist.OfficePhysicalState;
                        rb.OfficePhysicalZip = isExist.OfficePhysicalZip;
                        rb.OfficeContactFirstName = isExist.OfficeContactFirstName;
                        rb.OfficeContactLastName = isExist.OfficeContactLastName;
                        rb.OfficeContactSSN = isExist.OfficeContactSSN;
                        rb.OfficePhoneNumber = isExist.OfficePhoneNumber.Replace("-", "");
                        rb.CellPhoneNumber = isExist.CellPhoneNumber;
                        rb.EmailAddress = isExist.EmailAddress;
                        rb.OfficeManagerFirstName = isExist.OfficeManagerFirstName;
                        rb.OfficeManageLastName = isExist.OfficeManageLastName;
                        rb.OfficeManagerSSN = isExist.OfficeManagerSSN;
                        rb.OfficeManagerDOB = isExist.OfficeManagerDOB == null ? "" : Convert.ToDateTime(isExist.OfficeManagerDOB).ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                        rb.MailingAddress = isExist.MailingAddress;
                        rb.MailingCity = isExist.MailingCity;
                        rb.MailingState = isExist.MailingState;
                        rb.MailingZip = isExist.MailingZip;
                        rb.FulfillmentShippingAddress = isExist.FulfillmentShippingAddress;
                        rb.FulfillmentShippingCity = isExist.FulfillmentShippingCity;
                        rb.FulfillmentShippingState = isExist.FulfillmentShippingState;
                        rb.FulfillmentShippingZip = isExist.FulfillmentShippingZip;
                        rb.WebsiteAddress = isExist.WebsiteAddress;
                        rb.YearsinBusiness = isExist.YearsinBusiness.Value;
                        rb.NoofBankProductsLastYear = isExist.NoofBankProductsLastYear.Value;
                        rb.PreviousBankProductFacilitator = isExist.PreviousBankProductFacilitator;
                        if (isExist.OfficeContactDOB != null)
                            rb.OfficeContactDOB = isExist.OfficeContactDOB.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                        rb.OfficeManagerPhone = isExist.OfficeManagerPhone;
                        rb.OfficeManagerCellNo = isExist.OfficeManagerCellNo;
                        rb.OfficeManagerEmail = isExist.OfficeManagerEmail;
                    }
                    if (isExist.EntryLevel >= 2)
                    {
                        rb.OwnerFirstName = isExist.OwnerFirstName;
                        rb.OwnerLastName = isExist.OwnerLastName;
                        rb.OwnerSSN = isExist.OwnerSSN;
                        if (isExist.OnwerDOB != null)
                            rb.OnwerDOB = isExist.OnwerDOB.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/");
                        rb.OwnerHomePhone = isExist.OwnerHomePhone.Replace("-", "");
                        rb.OwnerAddress = isExist.OwnerAddress;
                        rb.OwnerCity = isExist.OwnerCity;
                        rb.OwnerState = isExist.OwnerState;
                        rb.OwnerZip = isExist.OwnerZip;
                        rb.LegarEntityStatus = isExist.LegarEntityStatus;
                        rb.LLCMembershipRegistration = isExist.LLCMembershipRegistration;
                        rb.BusinessEIN = isExist.BusinessEIN;
                    }
                    if (isExist.EntryLevel >= 3)
                    {
                        rb.EFINOwnerFirstName = isExist.EFINOwnerFirstName;
                        rb.EFINOwnerLastName = isExist.EFINOwnerLastName;
                        rb.EFINOwnerTitle = isExist.EFINOwnerTitle;
                        rb.EFINOwnerSSN = isExist.EFINOwnerSSN;
                        if (isExist.EFINOwnerDOB != null)
                            rb.EFINOwnerDOB = isExist.EFINOwnerDOB.HasValue ? isExist.EFINOwnerDOB.Value.ToString("MM'/'dd'/'yyyy").Replace("-", "/") : "";
                        rb.IsMultiOffice = isExist.IsMultiOffice;
                        rb.EFINOwnerAddress = isExist.EFINOwnerAddress;
                        rb.EFINOwnerCity = isExist.EFINOwnerCity;
                        rb.EFINOwnerState = isExist.EFINOwnerState;
                        rb.EFINOwnerZip = isExist.EFINOwnerZip;
                        rb.EFINOwnerPhone = isExist.EFINOwnerPhone;
                        rb.EFINOwnerMobile = isExist.EFINOwnerMobile;
                        rb.EFINOwnerEmail = isExist.EFINOwnerEmail;
                        rb.EFINOwnerIDNumber = isExist.EFINOwnerIDNumber;
                        rb.EFINOwnerIDState = isExist.EFINOwnerIDState;
                        rb.EFINOwnerEIN = isExist.EFINOwnerEIN;
                    }
                    if (isExist.EntryLevel >= 4)
                    {
                        rb.ProductsOffering = isExist.ProductsOffering.Value;
                        rb.IsPTIN = isExist.IsPTIN;
                        rb.IsAsPerProcessLaw = isExist.IsAsPerProcessLaw;
                        rb.IsAsPerComplainceLaw = isExist.IsAsPerComplainceLaw;
                        rb.ConsumerLending = isExist.ConsumerLending;
                        rb.NoofPersoneel = isExist.NoofPersoneel.Value;
                        rb.AdvertisingApproval = isExist.AdvertisingApproval;
                    }
                    if (isExist.EntryLevel >= 5)
                    {
                        rb.IsLockedStore_Documents = isExist.IsLockedStore_Documents;
                        rb.IsLockedStore_Checks = isExist.IsLockedStore_Checks;
                        rb.IsLocked_Office = isExist.IsLocked_Office;
                        rb.IsLimitAccess = isExist.IsLimitAccess;
                        rb.PlantoDispose = isExist.PlantoDispose;
                        rb.LoginAccesstoEmployees = isExist.LoginAccesstoEmployees;
                        rb.AntivirusRequired = isExist.AntivirusRequired;
                        rb.HasFirewall = isExist.HasFirewall;
                        rb.OnlineTraining = isExist.OnlineTraining;
                        rb.PasswordRequired = isExist.PasswordRequired;
                        rb.SupportOS = isExist.SupportOS;
                    }
                    if (isExist.EntryLevel >= 6)
                    {
                        rb.CheckingAccountName = isExist.CheckingAccountName;
                        rb.BankName = isExist.BankName;
                        rb.BankRoutingNumber = isExist.BankRoutingNumber;
                        rb.BankAccountNumber = isExist.BankAccountNumber;
                        rb.BankAccountType = isExist.BankAccountType;
                    }
                }
                return rb;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void saveEnrollmentCompletedStatus(Guid UserId, Guid CustomerId)
        {
            try
            {
                Guid sitemapid = Guid.Parse("0FEEB0FE-D0E7-4370-8733-DD5F7D2041FC");
                var isexist = db.CustomerConfigurationStatus.Where(x => x.CustomerId == CustomerId && x.SitemapId == sitemapid && x.StatusCode == EMPConstants.Done).FirstOrDefault();
                if (isexist == null)
                {
                    CustomerConfigurationStatu status = new CustomerConfigurationStatu();
                    status.CustomerId = CustomerId;
                    status.Id = Guid.NewGuid();
                    status.SitemapId = sitemapid;
                    status.StatusCode = EMPConstants.Done;
                    status.UpdatedBy = UserId;
                    status.UpdatedDate = DateTime.Now;
                    db.CustomerConfigurationStatus.Add(status);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/saveEnrollmentCompletedStatus", UserId);
            }
        }

        public int saveEnrollmenttoService(Guid CustomerId, Guid UserId, Guid BankId, Guid Prefer)
        {
            try
            {
                var User = db.OfficeManagements.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                if (User == null)
                    return -1;
                if (User.EntityId == (int)EMPConstants.Entity.SO || User.EntityId == (int)EMPConstants.Entity.SOME || User.EntityId == (int)EMPConstants.Entity.SOME_SS)
                {
                    var isbankexist = db.BankMasters.Where(x => x.Id == BankId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (isbankexist == null)
                        return 0;
                }
                else
                {
                    DropDownService ddService = new DropDownService();
                    List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                    EntityHierarchyDTOs = ddService.GetEntityHierarchies(CustomerId);

                    Guid TopParentId = Guid.Empty;

                    if (EntityHierarchyDTOs.Count > 0)
                    {
                        var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                        TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
                    }
                    var isbankexist = db.SubSiteBankConfigs.Where(x => x.BankMaster_ID == BankId && x.emp_CustomerInformation_ID == TopParentId).FirstOrDefault();
                    if (isbankexist == null)
                        return 0;
                }

                //var bank = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active &&).Select(x => x.BankId).FirstOrDefault();
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).Select(x => x).FirstOrDefault();
                var isexist = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == BankId).FirstOrDefault();
                if (isexist == null)
                {
                    BankEnrollment enr = new BankEnrollment();
                    enr.BankId = BankId;
                    enr.SalesYearId = customer.SalesYearID;
                    enr.EntityId = customer.EntityId;
                    enr.CustomerId = CustomerId;
                    enr.Id = Guid.NewGuid();
                    enr.StatusCode = EMPConstants.Ready;
                    enr.IsActive = true;
                    enr.CreatedBy = UserId;
                    enr.CreatedDate = DateTime.Now;
                    enr.UpdatedBy = UserId;
                    enr.UpdatedDate = DateTime.Now;
                    db.BankEnrollments.Add(enr);
                    db.SaveChanges();

                    BankEnrollmentStatu status = new BankEnrollmentStatu();
                    status.EnrollmentId = enr.Id;
                    status.Id = Guid.NewGuid();
                    status.Status = EMPConstants.Ready;
                    status.CreatedDate = DateTime.Now;
                    status.UpdatedBy = UserId;
                    status.UpdatedDate = DateTime.Now;
                    db.BankEnrollmentStatus.Add(status);
                    db.SaveChanges();
                }
                else
                {
                    isexist.UpdatedBy = UserId;
                    isexist.UpdatedDate = DateTime.Now;
                    isexist.IsActive = false;
                    db.SaveChanges();

                    BankEnrollment enr = new BankEnrollment();
                    enr.BankId = BankId;
                    enr.SalesYearId = customer.SalesYearID;
                    enr.EntityId = customer.EntityId;
                    enr.CustomerId = CustomerId;
                    enr.Id = Guid.NewGuid();
                    enr.StatusCode = EMPConstants.Ready;
                    enr.IsActive = true;
                    enr.CreatedBy = UserId;
                    enr.CreatedDate = DateTime.Now;
                    enr.UpdatedBy = UserId;
                    enr.UpdatedDate = DateTime.Now;
                    db.BankEnrollments.Add(enr);
                    db.SaveChanges();

                    BankEnrollmentStatu status = new BankEnrollmentStatu();
                    status.CreatedDate = DateTime.Now;
                    status.EnrollmentId = enr.Id;
                    status.Id = Guid.NewGuid();
                    status.Status = EMPConstants.Ready;
                    status.UpdatedBy = UserId;
                    status.UpdatedDate = DateTime.Now;
                    db.BankEnrollmentStatus.Add(status);
                    db.SaveChanges();
                }

                if (Prefer != Guid.Empty)
                {
                    var banksel = db.EnrollmentBankSelections.Where(x => x.BankId == Prefer && x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (banksel != null)
                    {
                        banksel.IsPreferredBank = true;
                        db.SaveChanges();
                    }
                }

                var result = SaveCustomerInfoBankEnrolled(UserId, CustomerId, BankId, true);
                return 1;

            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/saveEnrollmenttoService", UserId);
                return -2;
            }
        }

        public BankFeeLimit getFeeLimit(Guid CustomerId, Guid ParentId, Guid BankId)
        {
            try
            {
                bool IsMainSite = false;
                DropDownService ddService = new DropDownService();

                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(CustomerId);

                Guid TopParentId = Guid.Empty;
                Guid FeeSourceParentId = Guid.Empty;

                if (EntityHierarchyDTOs.Count > 0)
                {
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    TopParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

                    var FeeSource = EntityHierarchyDTOs.Where(o => o.EntityId == o.FeeSourceEntityId).FirstOrDefault();
                    if (FeeSource != null)
                    {
                        FeeSourceParentId = FeeSource.CustomerId ?? Guid.Empty;
                    }
                    else
                    {
                        FeeSourceParentId = TopParentId;
                    }

                    if (TopFromHierarchy.Customer_Level == 0)
                    {
                        IsMainSite = true;
                    }
                }


                SubSiteOfficeConfig SubSiteOfficeCon = new SubSiteOfficeConfig();


                decimal svbfee = 0, transfee = 0;
                decimal customersvbfee = 0, customertransfee = 0;
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (customer.EntityId == (int)EMPConstants.Entity.SO || customer.EntityId == (int)EMPConstants.Entity.SOME || customer.EntityId == (int)EMPConstants.Entity.SOME_SS)
                {
                    IsMainSite = true;
                    var mainfees = (from f in db.FeeMasters
                                    where (f.FeesFor == 2 || f.FeesFor == 3) && f.StatusCode == EMPConstants.Active
                                    select new { f.Amount, f.FeesFor }).ToList();

                    svbfee = mainfees.Where(x => x.FeesFor == 2).Select(x => x.Amount).FirstOrDefault().Value;
                    transfee = mainfees.Where(x => x.FeesFor == 3).Select(x => x.Amount).FirstOrDefault().Value;
                }
                else
                {
                    //if (customer.ParentId != null && customer.ParentId != Guid.Empty)
                    //{


                    var mainfees = (from s in db.CustomerAssociatedFees
                                    join f in db.FeeMasters on s.FeeMaster_ID equals f.Id
                                    where s.emp_CustomerInformation_ID == TopParentId && (f.FeesFor == 2 || f.FeesFor == 3) && f.StatusCode == EMPConstants.Active
                                    select new { s.Amount, f.FeesFor }).ToList();
                    svbfee = mainfees.Where(x => x.FeesFor == 2).Select(x => x.Amount).FirstOrDefault();
                    transfee = mainfees.Where(x => x.FeesFor == 3).Select(x => x.Amount).FirstOrDefault();

                    if (!IsMainSite)
                    {
                        SubSiteOfficeCon = db.SubSiteOfficeConfigs.Where(o => o.RefId == CustomerId).FirstOrDefault();
                        if (SubSiteOfficeCon != null)
                        {
                            if (!SubSiteOfficeCon.SiteanMSOLocation)
                            {
                                customer.IsMSOUser = false;
                            }
                            else
                            {
                                customer.IsMSOUser = true;
                            }
                        }
                    }
                    //}
                    //else
                    //{
                    //    IsMainSite = true;

                    //    var mainfees = (from s in db.CustomerAssociatedFees
                    //                    join f in db.FeeMasters on s.FeeMaster_ID equals f.Id
                    //                    where s.emp_CustomerInformation_ID == CustomerId && (f.FeesFor == 2 || f.FeesFor == 3) && f.StatusCode == EMPConstants.Active
                    //                    select new { s.Amount, f.FeesFor }).ToList();

                    //    svbfee = mainfees.Where(x => x.FeesFor == 2).Select(x => x.Amount).FirstOrDefault();
                    //    transfee = mainfees.Where(x => x.FeesFor == 3).Select(x => x.Amount).FirstOrDefault();
                    //}
                }

                if (IsMainSite == false)
                {
                    if ((customer.IsMSOUser ?? false) == true)
                    {
                        var customerfees = (from s in db.SubSiteBankFeesConfigs
                                            where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == BankId && (s.ServiceOrTransmitter == 1 || s.ServiceOrTransmitter == 2)
                                            select new { s.BankMaxFees_MSO, s.ServiceOrTransmitter }).ToList();
                        customersvbfee = customerfees.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees_MSO).FirstOrDefault() ?? 0;
                        customertransfee = customerfees.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees_MSO).FirstOrDefault() ?? 0;
                    }
                    else
                    {
                        var customerfees = (from s in db.SubSiteBankFeesConfigs
                                            where s.emp_CustomerInformation_ID == CustomerId && s.BankMaster_ID == BankId && (s.ServiceOrTransmitter == 1 || s.ServiceOrTransmitter == 2)
                                            select new { s.BankMaxFees, s.ServiceOrTransmitter }).ToList();
                        customersvbfee = customerfees.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees).FirstOrDefault();
                        customertransfee = customerfees.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees).FirstOrDefault();
                    }
                }

                decimal parentsvb = 0, parenttrnas = 0;
                BankFeeLimit _limit = new DTO.BankFeeLimit();
                _limit.ShoeTransmission = true;
                _limit.ShowSvb = true;

                if (!IsMainSite)
                {
                    // Guid rParentId = ParentId;

                    //if (customer.IsMSOUser ?? false)
                    //{
                    //    var parentsfees = (from s in db.SubSiteBankFeesConfigs
                    //                       where s.emp_CustomerInformation_ID == rParentId && s.BankMaster_ID == BankId && (s.ServiceOrTransmitter == 1 || s.ServiceOrTransmitter == 2)
                    //                       select new { s.BankMaxFees_MSO, s.ServiceOrTransmitter }).ToList();
                    //    parentsvb = parentsfees.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees_MSO).FirstOrDefault().Value;
                    //    parenttrnas = parentsfees.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees_MSO).FirstOrDefault().Value;
                    //}
                    //else
                    //{
                    //    var parentsfees = (from s in db.SubSiteBankFeesConfigs
                    //                       where s.emp_CustomerInformation_ID == rParentId && s.BankMaster_ID == BankId && (s.ServiceOrTransmitter == 1 || s.ServiceOrTransmitter == 2)
                    //                       select new { s.BankMaxFees, s.ServiceOrTransmitter }).ToList();
                    //    parentsvb = parentsfees.Where(x => x.ServiceOrTransmitter == 1).Select(x => x.BankMaxFees).FirstOrDefault();
                    //    parenttrnas = parentsfees.Where(x => x.ServiceOrTransmitter == 2).Select(x => x.BankMaxFees).FirstOrDefault();
                    //}
                    var issvb = db.SubSiteFeeConfigs.Where(x => x.emp_CustomerInformation_ID == TopParentId && x.StatusCode == EMPConstants.Active && x.ServiceorTransmission == 1).Select(x => x.IsSubSiteAddonFee).FirstOrDefault();
                    var istrans = db.SubSiteFeeConfigs.Where(x => x.emp_CustomerInformation_ID == TopParentId && x.StatusCode == EMPConstants.Active && x.ServiceorTransmission == 2).Select(x => x.IsSubSiteAddonFee).FirstOrDefault();

                    _limit.ShoeTransmission = istrans;
                    _limit.ShowSvb = issvb;
                }

                var limit = db.BankMasters.Where(x => x.Id == BankId).FirstOrDefault();
                if (limit != null)
                {
                    var svblimit = (customer.IsMSOUser ?? false ? limit.MaxFeeLimitMSO ?? 0 : limit.MaxFeeLimitDeskTop.Value) - svbfee - customersvbfee - parentsvb;
                    //Murali Krishna AT 12th Oct 2016 : Transmission Fee not required in calculation 
                    //var translimit = limit.MaxTranFeeDeskTop.Value - transfee - customertransfee - parenttrnas; 
                    var translimit = (customer.IsMSOUser ?? false ? limit.MaxTranFeeMSO ?? 0 : limit.MaxTranFeeDeskTop.Value) - customertransfee - parenttrnas;

                    _limit.Status = true;
                    _limit.SvbFeeLimit = svblimit;
                    _limit.TransFeeLimit = translimit;
                    return _limit;
                }
                else
                    return new BankFeeLimit();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getFeeLimit", CustomerId);
                return new BankFeeLimit();
            }
        }

        public bool IsAcivated(Guid CustomerId)
        {
            try
            {
                var bank = (from s in db.EnrollmentBankSelections
                            join b in db.BankMasters on s.BankId equals b.Id
                            where s.CustomerId == CustomerId && s.StatusCode == EMPConstants.Active
                            select b.BankCode).FirstOrDefault();
                if (!string.IsNullOrEmpty(bank))
                {
                    if (bank == "S")
                    {
                        var isexist = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (isexist != null)
                            return true;
                    }
                    else if (bank == "V")
                    {
                        var isexist = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (isexist != null)
                            return true;
                    }
                    else if (bank == "R")
                    {
                        var isexist = db.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (isexist != null)
                            return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/IsAcivated", CustomerId);
                return false;
            }
        }

        public BankEnrollmentStatusInfo getBankEnrollmentStatus(Guid CustomerId, Guid bankid)
        {
            BankEnrollmentStatusInfo status = new DTO.BankEnrollmentStatusInfo();
            try
            {
                var isexist = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.BankId == bankid && x.IsActive == true).FirstOrDefault();
                if (isexist != null)
                {
                    status.SubmissionStaus = isexist.StatusCode;
                    status.BankId = isexist.BankId.Value.ToString();
                }
                else
                {
                    status.SubmissionStaus = "";
                }

                status.SubmissionCount = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && (x.StatusCode == EMPConstants.Submitted || x.StatusCode == EMPConstants.EnrPending) && x.IsActive == true).Count();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getBankEnrollmentStatus", CustomerId);
                status.SubmissionStaus = "";
            }
            return status;
        }

        public CustomerBankStaus getBankEnrollmentStatusofCustomer(Guid CustomerId)
        {
            CustomerBankStaus res = new CustomerBankStaus();
            try
            {
                var isexist = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true && (x.StatusCode == EMPConstants.Submitted || x.StatusCode == EMPConstants.EnrPending) && x.ArchiveStatusCode != EMPConstants.Archive).FirstOrDefault();
                if (isexist != null)
                    res.DisableEfin = true;
                var isunlocked = db.CustomerUnlocks.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                if (isunlocked == null)
                    res.IsUnlocked = false;
                else
                    res.IsUnlocked = true;

                if (!res.IsUnlocked && !res.DisableEfin)
                {
                    var isexist1 = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.StatusCode == EMPConstants.Approved && x.ArchiveStatusCode != EMPConstants.Archive).FirstOrDefault();
                    if (isexist1 != null)
                        res.DisableEfin = true;
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getBankEnrollmentStatusofCustomer", CustomerId);
            }
            return res;
        }

        public bool IsBankEnrollmentSubmitted(Guid CustomerId)
        {
            try
            {
                List<Guid> customers = new List<Guid>();
                customers.Add(CustomerId);
                var childcustomers = db.emp_CustomerInformation.Where(x => x.ParentId == CustomerId).Select(x => x.Id).ToList();
                if (childcustomers.Count > 0)
                {
                    customers.AddRange(childcustomers);
                    var subchildcustomers = db.emp_CustomerInformation.Where(x => customers.Contains(x.ParentId.Value)).Select(x => x.Id).ToList();
                    if (subchildcustomers.Count > 0)
                        customers.AddRange(subchildcustomers);
                }
                var isexist = db.BankEnrollments.Where(x => customers.Contains(x.CustomerId.Value) && x.IsActive == true && (x.StatusCode == EMPConstants.Ready || x.StatusCode == EMPConstants.Submitted)).Count();
                return isexist > 0;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/IsBankEnrollmentSubmitted", CustomerId);
                return false;
            }
        }

        public bool SaveRAEFINOwnerInfo(List<EnrollmentBankEFINOwnerRADTO> RAEFINOwnerInfo, Guid UserId, Guid EnrollmentRAID)
        {
            try
            {
                db = new DatabaseEntities();
                if (RAEFINOwnerInfo.ToList().Count > 0)
                {
                    var subsiteBankFeeconfig = db.BankEnrollmentEFINOwnersForRAs.Where(o => o.BankEnrollmentRAId == EnrollmentRAID).ToList();
                    if (subsiteBankFeeconfig.Count() > 0)
                    {
                        db.BankEnrollmentEFINOwnersForRAs.RemoveRange(subsiteBankFeeconfig);
                        db.SaveChanges();
                    }
                    foreach (var item in RAEFINOwnerInfo)
                    {
                        BankEnrollmentEFINOwnersForRA BankEnrollmentEFINOwnersRA = new BankEnrollmentEFINOwnersForRA();
                        BankEnrollmentEFINOwnersRA.Id = Guid.NewGuid();
                        //  BankEnrollmentEFINOwnersRA.Id = 0;
                        BankEnrollmentEFINOwnersRA.BankEnrollmentRAId = EnrollmentRAID;
                        BankEnrollmentEFINOwnersRA.FirstName = item.FirstName;
                        BankEnrollmentEFINOwnersRA.LastName = item.LastName;
                        BankEnrollmentEFINOwnersRA.DateofBirth = item.DateofBirth;
                        BankEnrollmentEFINOwnersRA.EmailId = item.EmailId;
                        BankEnrollmentEFINOwnersRA.Address = item.Address;
                        BankEnrollmentEFINOwnersRA.City = item.City;
                        BankEnrollmentEFINOwnersRA.StateId = item.StateId;
                        BankEnrollmentEFINOwnersRA.SSN = item.SSN;
                        BankEnrollmentEFINOwnersRA.HomePhone = item.HomePhone;
                        BankEnrollmentEFINOwnersRA.MobilePhone = item.MobilePhone;
                        BankEnrollmentEFINOwnersRA.PercentageOwned = item.PercentageOwned;
                        BankEnrollmentEFINOwnersRA.IDNumber = item.IDNumber;
                        BankEnrollmentEFINOwnersRA.IDState = item.IDState;
                        BankEnrollmentEFINOwnersRA.ZipCode = item.ZipCode;

                        BankEnrollmentEFINOwnersRA.CreatedBy = UserId;
                        BankEnrollmentEFINOwnersRA.CreatedDate = DateTime.Now;
                        BankEnrollmentEFINOwnersRA.UpdatedBy = UserId;
                        BankEnrollmentEFINOwnersRA.UpdatedDate = DateTime.Now;

                        db.BankEnrollmentEFINOwnersForRAs.Add(BankEnrollmentEFINOwnersRA);
                    }
                    db.SaveChanges();
                    //db.Dispose();
                }
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveRAEFINOwnerInfo", UserId);
                return false;
            }
        }

        public BankEnrollmentStatusInfo getEnrollmentStatusInfo(Guid CustomerId)
        {
            BankEnrollmentStatusInfo info = new DTO.BankEnrollmentStatusInfo();
            try
            {
                var enrCnt = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Approved && x.IsActive == true && x.ArchiveStatusCode != EMPConstants.Archive).Count();
                if (enrCnt >= 2)
                    info.ShowBankselection = true;
                else
                    info.ShowBankselection = false;


                var isapproved = (from x in db.BankEnrollments
                                  join b in db.BankMasters on x.BankId equals b.Id
                                  join s in db.EnrollmentBankSelections on x.BankId equals s.BankId
                                  where x.CustomerId == CustomerId && x.IsActive == true && x.StatusCode == EMPConstants.Approved && x.ArchiveStatusCode != EMPConstants.Archive
                                  && s.StatusCode == EMPConstants.Active && s.CustomerId == CustomerId
                                  orderby s.BankSubmissionStatus descending, s.LastUpdatedDate descending
                                  select new { b.BankName, x.Id }).FirstOrDefault();
                if (isapproved != null)
                {
                    info.status = true;
                    info.BankName = isapproved.BankName;
                    info.BankId = isapproved.Id.ToString();
                    info.SubmissionStaus = "Approved";
                    var submitted = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == isapproved.Id && x.Status == EMPConstants.SubmittedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    info.SubmitedDate = submitted == null ? "" : submitted.CreatedDate.Value.ToString("MM'/'dd'/'yyyy HH:mm");
                    var isunlocked = db.CustomerUnlocks.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                    if (isunlocked == null)
                    {
                        info.ShowUnlock = true;
                    }
                    else
                    {
                        info.ShowUnlock = !isunlocked.IsUnlocked;
                    }
                    return info;
                }

                var bank = (from s in db.BankEnrollments
                            join b in db.BankMasters on s.BankId equals b.Id
                            where s.CustomerId == CustomerId && s.IsActive == true && s.ArchiveStatusCode != EMPConstants.Archive //&& s.BankId == bankid
                            select new { b.BankName, s.Id, s.StatusCode }).FirstOrDefault();

                if (bank != null)
                {
                    info.status = true;
                    info.BankName = bank.BankName;
                    info.BankId = bank.Id.ToString();
                    info.SubmissionStaus = bank.StatusCode == EMPConstants.Ready ? "Unsuccessful" : ((bank.StatusCode == EMPConstants.Submitted) ? "Submitted" :
                                (bank.StatusCode == EMPConstants.Approved ? "Approved" : (bank.StatusCode == EMPConstants.Rejected ? "Rejected" :
                                (bank.StatusCode == EMPConstants.Denied ? "Denied" : (bank.StatusCode == EMPConstants.Cancelled ? "Cancelled" : "Pending")))));
                    if (bank.StatusCode == EMPConstants.Submitted)
                    {
                        var submitted = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.Id && x.Status == EMPConstants.SubmittedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                        info.SubmitedDate = submitted == null ? "" : submitted.CreatedDate.Value.ToString("MM'/'dd'/'yyyy HH:mm");
                    }
                    else
                        info.SubmitedDate = "";
                    var isunlocked = db.CustomerUnlocks.Where(x => x.CustomerId == CustomerId).FirstOrDefault();
                    if (isunlocked == null)
                    {
                        if (bank.StatusCode == EMPConstants.Approved)
                            info.ShowUnlock = true;
                        else
                            info.ShowUnlock = false;
                    }
                    else
                    {
                        info.ShowUnlock = !isunlocked.IsUnlocked;
                    }

                    //var substatus = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.Id).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    //if (substatus != null)
                    //{
                    //    if (substatus.IsUnlocked == true)
                    //    {
                    //        info.status = false;
                    //    }
                    //    else
                    //    {
                    //        info.BankName = bank.BankName;
                    //        info.status = true;
                    //        info.SubmissionStaus = substatus.Status == EMPConstants.Ready ? "Unsuccessful" : ((substatus.Status == EMPConstants.SubmittedService) ? "Submitted" :
                    //            (substatus.Status == EMPConstants.ApprovedService ? "Approved" : (substatus.Status == EMPConstants.RejectedService ? "Rejected" :
                    //            (substatus.Status == EMPConstants.DeniedServcce ? "Denied" : "Pending"))));
                    //        var submitted = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == bank.Id && x.Status == EMPConstants.SubmittedService).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    //        info.SubmitedDate = submitted == null ? "" : submitted.CreatedDate.Value.ToString("MM'/'dd'/'yyyy HH:mm");

                    //        var rejectedbanks = (from s in db.BankEnrollmentHistories
                    //                             join b in db.BankMasters on s.BankId equals b.Id
                    //                             where s.CustomerId == CustomerId && (s.Message == "Bank App Rejected" || s.Message == "Bank App Denied")
                    //                             //&& b.Id == bankid
                    //                             orderby s.CreatedDate descending
                    //                             select b.BankName).ToArray();
                    //        var approedbank = (from s in db.BankEnrollmentHistories
                    //                           join b in db.BankMasters on s.BankId equals b.Id
                    //                           where s.CustomerId == CustomerId && s.Message == "Bank App Approved" //&& b.Id == bankid
                    //                           orderby s.CreatedDate descending
                    //                           select b.BankName).ToArray();
                    //        var unlocked = (from s in db.BankEnrollmentStatus
                    //                        join e in db.BankEnrollments on s.EnrollmentId equals e.Id
                    //                        join b in db.BankMasters on e.BankId equals b.Id
                    //                        where e.CustomerId == CustomerId && s.IsUnlocked == true && e.IsActive == true //&& b.Id == bankid
                    //                        orderby s.CreatedDate descending
                    //                        select b.BankName).ToArray();
                    //        info.ApprovedBank = string.Join("\n", approedbank);
                    //        info.UnlockedBanks = string.Join("\n", unlocked);
                    //        info.RejectedBanks = string.Join("\n", rejectedbanks);
                    //        info.BankId = bank.Id.ToString();
                    //    }
                    //}
                }
                else
                {
                    info.status = false;
                    //var bank2 = (from s in db.EnrollmentBankSelections
                    //             where s.CustomerId == CustomerId && s.StatusCode == EMPConstants.Active && s.BankId == bankid
                    //             select new { s.BankId }).FirstOrDefault();

                    //if (bank2 != null)
                    //{
                    //    if (Guid.Empty == bank2.BankId)
                    //    {
                    //        info.BankName = "NONE";
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getEnrollmentStatusInfo", CustomerId);
                info.status = false;
            }
            return info;
        }

        public bool unlockEnrollment(UnlockEnrollment req)
        {
            try
            {
                var isexist = db.CustomerUnlocks.Where(x => x.CustomerId == req.CustomerId).FirstOrDefault();
                if (isexist != null)
                {
                    isexist.IsUnlocked = true;
                    isexist.UpdatedBy = req.UserId;
                    isexist.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    CustomerUnlock unlock = new CustomerUnlock();
                    unlock.CreatedBy = req.UserId;
                    unlock.CreatedDate = DateTime.Now;
                    unlock.CustomerId = req.CustomerId;
                    unlock.IsUnlocked = true;
                    unlock.UpdatedBy = req.UserId;
                    unlock.UpdatedDate = DateTime.Now;
                    db.CustomerUnlocks.Add(unlock);
                    db.SaveChanges();
                }

                //var bankselection = db.EnrollmentBankSelections.Where(x => x.CustomerId == req.CustomerId && x.StatusCode == EMPConstants.Active && x.BankSubmissionStatus==EMPConstants.DefaultBank).FirstOrDefault();
                //if (bankselection != null)
                //{
                //    bankselection.StatusCode = EMPConstants.InActive;
                //    bankselection.LastUpdatedBy = req.UserId;
                //    bankselection.LastUpdatedDate = DateTime.Now;
                //    bankselection.BankSubmissionStatus = 0;
                //    bankselection.IsPreferredBank = false;

                //    var bankcode = db.BankMasters.Where(x => x.Id == bankselection.BankId).Select(x => x.BankCode).FirstOrDefault();
                //    if (bankcode == "S")
                //    {
                //        var bankenroll = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == req.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                //        if (bankenroll != null)
                //        {
                //            bankenroll.StatusCode = EMPConstants.InActive;
                //        }
                //    }
                //    else if (bankcode == "R")
                //    {
                //        var bankenroll = db.BankEnrollmentForRBs.Where(x => x.CustomerId == req.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                //        if (bankenroll != null)
                //        {
                //            bankenroll.StatusCode = EMPConstants.InActive;
                //        }
                //    }
                //    else if (bankcode == "V")
                //    {
                //        var bankenroll = db.BankEnrollmentForRAs.Where(x => x.CustomerId == req.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                //        if (bankenroll != null)
                //        {
                //            bankenroll.StatusCode = EMPConstants.InActive;
                //        }
                //    }
                //    db.SaveChanges();

                //    var enrollmentId = db.BankEnrollments.Where(x => x.CustomerId == req.CustomerId && x.IsActive == true && x.BankId == bankselection.BankId).FirstOrDefault();
                //    if (enrollmentId != null)
                //    {
                //        enrollmentId.IsActive = false;

                //        var status = db.BankEnrollmentStatus.Where(x => x.EnrollmentId == enrollmentId.Id && x.Status == EMPConstants.ApprovedService && x.IsUnlocked != true).FirstOrDefault();
                //        if (status != null)
                //        {
                //            status.IsUnlocked = true;
                //            status.UnlockedBy = req.UserId;
                //            status.UnlockedDate = DateTime.Now;
                //            status.Reason = req.Reason;
                //        }
                //        db.SaveChanges();
                //    }

                //    var configstatus = db.CustomerConfigurationStatus.Where(x => x.CustomerId == req.CustomerId && x.StatusCode == EMPConstants.Done
                //    && (x.SitemapId == new Guid("067C03A3-34F1-4143-BEAE-35327A8FCA44") || x.SitemapId == new Guid("0FEEB0FE-D0E7-4370-8733-DD5F7D2041FC"))).ToList();
                //    //foreach (var item in configstatus)
                //    //{
                //    //    item.StatusCode = EMPConstants.UnLocked;

                //    //}
                //    if (configstatus.Count > 0)
                //        db.CustomerConfigurationStatus.RemoveRange(configstatus);
                //    db.SaveChanges();

                //}
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/unlockEnrollment", Guid.Empty);
                return false;
            }
        }

        public List<Guid> GetUnlockedBanks(Guid userid)
        {
            try
            {

                var bankids = (from s in db.BankEnrollmentStatus
                               join e in db.BankEnrollments on s.EnrollmentId equals e.Id
                               where s.Status == EMPConstants.ApprovedService && s.IsUnlocked == true && e.CustomerId == userid && e.IsActive == false
                               select e).ToList();
                return bankids.Select(x => x.BankId.Value).Distinct().ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetUnlockedBanks", userid);
                return new List<Guid>();
            }
        }

        public List<Guid> GetRejectedBanks(Guid userid)
        {
            try
            {
                var bankids = (from s in db.BankEnrollmentStatus
                               join e in db.BankEnrollments on s.EnrollmentId equals e.Id
                               where s.Status == EMPConstants.RejectedService && e.CustomerId == userid && e.IsActive == false
                               select e).ToList();
                return bankids.Select(x => x.BankId.Value).Distinct().ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/GetRejectedBanks", userid);
                return new List<Guid>();
            }
        }

        public bool SaveNextTPGBankEnrollment(TPGBankEnrollment enrollment)
        {
            try
            {
                var isExist = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    isExist.BankUsedLastYear = enrollment.LastYearBank;
                    isExist.CompanyName = enrollment.CompanyName;
                    isExist.EFINOwnerAddress = enrollment.OwnerAddress;
                    isExist.EFINOwnerCity = enrollment.OwnerCity;
                    if (!string.IsNullOrEmpty(enrollment.OwnerDOB))
                        isExist.EFINOwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.EFINOwnerEIN = enrollment.OwnerEIN;
                    isExist.EFINOwnerEmail = enrollment.OwnerEmail;
                    isExist.EFINOwnerFirstName = enrollment.OwnerFirstName;
                    isExist.EFINOwnerLastName = enrollment.OwnerLastName;
                    isExist.EFINOwnerSSN = enrollment.OwnerSSn;
                    isExist.EFINOwnerState = enrollment.OwnerState;
                    isExist.EFINOwnerTelephone = enrollment.OwnerTelephone;
                    isExist.EFINOwnerZip = enrollment.OwnerZip;
                    isExist.ManagerEmail = enrollment.ManagerEmail;
                    isExist.ManagerFirstName = enrollment.ManagerFirstName;
                    isExist.ManagerLastName = enrollment.ManagerLastName;
                    isExist.OfficeAccountType = enrollment.AccountType;
                    isExist.OfficeAddress = enrollment.OfficeAddress;
                    isExist.OfficeCity = enrollment.OfficeCity;
                    isExist.OfficeDAN = enrollment.OfficeDAN;
                    isExist.OfficeFAX = enrollment.OfficeFax;
                    isExist.OfficeRTN = enrollment.OfficeRTN;
                    isExist.OfficeState = enrollment.OfficeState;
                    isExist.OfficeTelephone = enrollment.OfficeTelephone;
                    isExist.OfficeZip = enrollment.OfficeZip;
                    isExist.PriorYearEFIN = enrollment.LastYearEFIN;
                    isExist.PriorYearFund = enrollment.BankProductFund;
                    isExist.PriorYearVolume = enrollment.LastYearVolume;
                    isExist.ShippingAddress = enrollment.ShippingAddress;
                    isExist.ShippingCity = enrollment.ShippingCity;
                    isExist.ShippingState = enrollment.ShippingState;
                    isExist.ShippingZip = enrollment.ShippingZip;

                    isExist.OwnerTitle = enrollment.EfinOwnerTitle;
                    isExist.OwnerMobile = enrollment.EfinOwnerMobile;
                    isExist.OwnerIdNumber = enrollment.EfinIDNumber;
                    isExist.OwnerIdState = enrollment.EfinIdState;
                    isExist.FeeOnAll = enrollment.SbfeeAll;
                    isExist.AddonFee = enrollment.Addonfee;
                    isExist.ServiceBureauFee = enrollment.ServiceBureaufee;
                    isExist.DocPrepFee = enrollment.DocPrepFee;
                    isExist.BankName = enrollment.BankName;
                    isExist.AccountName = enrollment.AccountName;
                    isExist.AgreeBank = enrollment.AgreeBank;
                    isExist.CheckPrint = enrollment.CheckPrint;
                    isExist.EntryLevel = isExist.EntryLevel != 5 ? enrollment.EntryLevel : isExist.EntryLevel;
                    isExist.UpdatedBy = enrollment.UserId;
                    isExist.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                }
                else
                {
                    BankEnrollmentForTPG tpg = new BankEnrollmentForTPG();
                    tpg.BankUsedLastYear = enrollment.LastYearBank;
                    tpg.CompanyName = enrollment.CompanyName;
                    tpg.EFINOwnerAddress = enrollment.OwnerAddress;
                    tpg.EFINOwnerCity = enrollment.OwnerCity;
                    if (!string.IsNullOrEmpty(enrollment.OwnerDOB))
                        tpg.EFINOwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    tpg.EFINOwnerEIN = enrollment.OwnerEIN;
                    tpg.EFINOwnerEmail = enrollment.OwnerEmail;
                    tpg.EFINOwnerFirstName = enrollment.OwnerFirstName;
                    tpg.EFINOwnerLastName = enrollment.OwnerLastName;
                    tpg.EFINOwnerSSN = enrollment.OwnerSSn;
                    tpg.EFINOwnerState = enrollment.OwnerState;
                    tpg.EFINOwnerTelephone = enrollment.OwnerTelephone;
                    tpg.EFINOwnerZip = enrollment.OwnerZip;
                    tpg.Id = Guid.NewGuid();
                    tpg.ManagerEmail = enrollment.ManagerEmail;
                    tpg.ManagerFirstName = enrollment.ManagerFirstName;
                    tpg.ManagerLastName = enrollment.ManagerLastName;
                    tpg.OfficeAccountType = enrollment.AccountType;
                    tpg.OfficeAddress = enrollment.OfficeAddress;
                    tpg.OfficeCity = enrollment.OfficeCity;
                    tpg.OfficeDAN = enrollment.OfficeDAN;
                    tpg.OfficeFAX = enrollment.OfficeFax;
                    tpg.OfficeRTN = enrollment.OfficeRTN;
                    tpg.OfficeState = enrollment.OfficeState;
                    tpg.OfficeTelephone = enrollment.OfficeTelephone;
                    tpg.OfficeZip = enrollment.OfficeZip;
                    tpg.PriorYearEFIN = enrollment.LastYearEFIN;
                    tpg.PriorYearFund = enrollment.BankProductFund;
                    tpg.PriorYearVolume = enrollment.LastYearVolume;
                    tpg.ShippingAddress = enrollment.ShippingAddress;
                    tpg.ShippingCity = enrollment.ShippingCity;
                    tpg.ShippingState = enrollment.ShippingState;
                    tpg.ShippingZip = enrollment.ShippingZip;

                    tpg.CustomerId = enrollment.CustomerId;
                    tpg.StatusCode = EMPConstants.Active;
                    tpg.OwnerTitle = enrollment.EfinOwnerTitle;
                    tpg.OwnerMobile = enrollment.EfinOwnerMobile;
                    tpg.OwnerIdNumber = enrollment.EfinIDNumber;
                    tpg.OwnerIdState = enrollment.EfinIdState;
                    tpg.FeeOnAll = enrollment.SbfeeAll;
                    tpg.AddonFee = enrollment.Addonfee;
                    tpg.ServiceBureauFee = enrollment.ServiceBureaufee;
                    tpg.DocPrepFee = enrollment.DocPrepFee;
                    tpg.BankName = enrollment.BankName;
                    tpg.AccountName = enrollment.AccountName;
                    tpg.AgreeBank = enrollment.AgreeBank;
                    tpg.CheckPrint = enrollment.CheckPrint;
                    tpg.IsEnrtyCompleted = false;
                    tpg.EntryLevel = enrollment.EntryLevel;

                    tpg.CreatedBy = enrollment.UserId;
                    tpg.CreatedDate = DateTime.Now;
                    tpg.UpdatedBy = enrollment.UserId;
                    tpg.UpdatedDate = DateTime.Now;

                    db.BankEnrollmentForTPGs.Add(tpg);
                    db.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveNextTPGBankEnrollment", Guid.Empty);
                return false;
            }
        }

        public bool SaveCustomerInfoBankEnrolled(Guid UserId, Guid CustomerId, Guid? BankId, bool IsEnrolled = false)
        {
            try
            {
                db = new DatabaseEntities();
                var custinfo = db.emp_CustomerInformation.Where(o => o.Id == CustomerId).FirstOrDefault();
                if (custinfo != null)
                {
                    custinfo.IsEnrolled = IsEnrolled;
                    custinfo.EnrolledBankId = BankId;
                    custinfo.LastUpdatedBy = UserId;
                    custinfo.IsActivationCompleted = 1;
                    custinfo.AccountStatus = "Active";
                    custinfo.StatusCode = EMPConstants.Active;
                    custinfo.LastUpdatedDate = DateTime.Now;
                    db.Entry(custinfo).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    db.Dispose();
                }
                else
                {
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveCustomerInfoBankEnrolled", UserId);
                return false;
            }
        }

        public bool SaveNextRBBankEnrollment(RBBankEnrollment enrollment)
        {
            DateTime? dd = null;
            try
            {
                var isExist = db.BankEnrollmentForRBs.Where(x => x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    isExist.ActualNoofBankProductsLastYear = enrollment.ActualNoofBankProductsLastYear;
                    isExist.AdvertisingApproval = enrollment.AdvertisingApproval;
                    isExist.AltOfficeContact1Email = enrollment.AltOfficeContact1Email;
                    isExist.AltOfficeContact1FirstName = enrollment.AltOfficeContact1FirstName;
                    isExist.AltOfficeContact1LastName = enrollment.AltOfficeContact1LastName;
                    isExist.AltOfficeContact1SSn = enrollment.AltOfficeContact1SSn;
                    isExist.AltOfficeContact2Email = enrollment.AltOfficeContact2Email;
                    isExist.AltOfficeContact2FirstName = enrollment.AltOfficeContact2FirstName;
                    isExist.AltOfficeContact2LastName = enrollment.AltOfficeContact2LastName;
                    isExist.AltOfficeContact2SSn = enrollment.AltOfficeContact2SSn;
                    isExist.AltOfficePhysicalAddress = enrollment.AltOfficePhysicalAddress;
                    isExist.AltOfficePhysicalAddress2 = enrollment.AltOfficePhysicalAddress2;
                    isExist.AltOfficePhysicalCity = enrollment.AltOfficePhysicalCity;
                    isExist.AltOfficePhysicalState = enrollment.AltOfficePhysicalState;
                    isExist.AltOfficePhysicalZipcode = enrollment.AltOfficePhysicalZipcode;
                    isExist.AntivirusRequired = enrollment.AntivirusRequired;
                    isExist.BankAccountNumber = enrollment.BankAccountNumber;
                    isExist.BankAccountType = enrollment.BankAccountType;
                    isExist.BankRoutingNumber = enrollment.BankRoutingNumber;
                    isExist.BusinessEIN = enrollment.BusinessEIN;

                    isExist.OfficeContactDOB = string.IsNullOrEmpty(enrollment.OfficeContactDOB) ? dd : DateTime.ParseExact(enrollment.OfficeContactDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.OfficeManagerPhone = enrollment.OfficeManagerPhone;
                    isExist.OfficeManagerCellNo = enrollment.OfficeManagerCellNo;
                    isExist.OfficeManagerEmail = enrollment.OfficeManagerEmail;

                    isExist.BusinessIncorporation = string.IsNullOrEmpty(enrollment.BusinessIncorporation) ? dd : DateTime.ParseExact(enrollment.BusinessIncorporation.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.BusinessName = enrollment.BusinessName;
                    isExist.CellPhoneNumber = enrollment.CellPhoneNumber.Replace("-", "");
                    isExist.CheckingAccountName = enrollment.CheckingAccountName;
                    isExist.ConsumerLending = enrollment.ConsumerLending;
                    isExist.EFINOwnerDOB = string.IsNullOrEmpty(enrollment.EFINOwnerDOB) ? dd : DateTime.ParseExact(enrollment.EFINOwnerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.EFINOwnerFirstName = enrollment.EFINOwnerFirstName;
                    isExist.EFINOwnerLastName = enrollment.EFINOwnerLastName;
                    isExist.EFINOwnerSSN = enrollment.EFINOwnerSSN;
                    isExist.EmailAddress = enrollment.EmailAddress;
                    isExist.EROApplicattionDate = string.IsNullOrEmpty(enrollment.EROApplicattionDate) ? dd : DateTime.ParseExact(enrollment.EROApplicattionDate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    isExist.EROParticipation = enrollment.EROParticipation;
                    isExist.EROReadTAndC = enrollment.EROReadTAndC;
                    isExist.FAXNumber = enrollment.FAXNumber;
                    isExist.FulfillmentShippingAddress = enrollment.FulfillmentShippingAddress;
                    isExist.FulfillmentShippingCity = enrollment.FulfillmentShippingCity;
                    isExist.FulfillmentShippingState = enrollment.FulfillmentShippingState;
                    isExist.FulfillmentShippingZip = enrollment.FulfillmentShippingZip;
                    isExist.HasFirewall = enrollment.HasFirewall;
                    isExist.IsAsPerComplainceLaw = enrollment.IsAsPerComplainceLaw;
                    isExist.IsAsPerProcessLaw = enrollment.IsAsPerProcessLaw;
                    isExist.IsLimitAccess = enrollment.IsLimitAccess;
                    isExist.IsLockedStore_Checks = enrollment.IsLockedStore_Checks;
                    isExist.IsLockedStore_Documents = enrollment.IsLockedStore_Documents;
                    isExist.IsLocked_Office = enrollment.IsLocked_Office;
                    isExist.IsMultiOffice = enrollment.IsMultiOffice;
                    isExist.IsOfficeTransmit = enrollment.IsOfficeTransmit;
                    isExist.IsPTIN = enrollment.IsPTIN;
                    isExist.LegarEntityStatus = enrollment.LegarEntityStatus;
                    isExist.LLCMembershipRegistration = enrollment.LLCMembershipRegistration;
                    isExist.LoginAccesstoEmployees = enrollment.LoginAccesstoEmployees;
                    isExist.MailingAddress = enrollment.MailingAddress;
                    isExist.MailingCity = enrollment.MailingCity;
                    isExist.MailingState = enrollment.MailingState;
                    isExist.MailingZip = enrollment.MailingZip;
                    isExist.NoofBankProductsLastYear = enrollment.NoofBankProductsLastYear;
                    isExist.NoofPersoneel = enrollment.NoofPersoneel;
                    isExist.OfficeContactFirstName = enrollment.OfficeContactFirstName;
                    isExist.OfficeContactLastName = enrollment.OfficeContactLastName;
                    isExist.OfficeContactSSN = enrollment.OfficeContactSSN;
                    isExist.OfficeManageLastName = enrollment.OfficeManageLastName;
                    if (!string.IsNullOrEmpty(enrollment.OfficeManagerDOB))
                        isExist.OfficeManagerDOB = DateTime.ParseExact(enrollment.OfficeManagerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.OfficeManagerFirstName = enrollment.OfficeManagerFirstName;
                    isExist.OfficeManagerSSN = enrollment.OfficeManagerSSN;
                    isExist.OfficeName = enrollment.OfficeName;
                    isExist.OfficePhoneNumber = enrollment.OfficePhoneNumber.Replace("-", "");
                    isExist.OfficePhysicalAddress = enrollment.OfficePhysicalAddress;
                    isExist.OfficePhysicalCity = enrollment.OfficePhysicalCity;
                    isExist.OfficePhysicalState = enrollment.OfficePhysicalState;
                    isExist.OfficePhysicalZip = enrollment.OfficePhysicalZip;
                    isExist.OnlineTraining = enrollment.OnlineTraining;
                    if (!string.IsNullOrEmpty(enrollment.OnwerDOB))
                        isExist.OnwerDOB = DateTime.ParseExact(enrollment.OnwerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    isExist.OwnerAddress = enrollment.OwnerAddress;
                    isExist.OwnerCity = enrollment.OwnerCity;
                    isExist.OwnerFirstName = enrollment.OwnerFirstName;
                    isExist.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    isExist.OwnerLastName = enrollment.OwnerLastName;
                    isExist.OwnerSSN = enrollment.OwnerSSN;
                    isExist.OwnerState = enrollment.OwnerState;
                    isExist.OwnerZip = enrollment.OwnerZip;
                    isExist.PasswordRequired = enrollment.PasswordRequired;
                    isExist.PlantoDispose = enrollment.PlantoDispose;
                    isExist.PreviousBankProductFacilitator = enrollment.PreviousBankProductFacilitator;
                    isExist.ProductsOffering = enrollment.ProductsOffering;
                    isExist.RetailPricingMethod = enrollment.RetailPricingMethod;
                    isExist.SPAAmount = enrollment.SPAAmount;
                    isExist.SPADate = string.IsNullOrEmpty(enrollment.SPADate) ? dd : DateTime.ParseExact(enrollment.SPADate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    isExist.WebsiteAddress = enrollment.WebsiteAddress;
                    isExist.YearsinBusiness = enrollment.YearsinBusiness;

                    isExist.EFINOwnerTitle = enrollment.EFINOwnerTitle;
                    isExist.EFINOwnerAddress = enrollment.EFINOwnerAddress;
                    isExist.EFINOwnerCity = enrollment.EFINOwnerCity;
                    isExist.EFINOwnerState = enrollment.EFINOwnerState;
                    isExist.EFINOwnerZip = enrollment.EFINOwnerZip;
                    isExist.EFINOwnerPhone = enrollment.EFINOwnerPhone;
                    isExist.EFINOwnerMobile = enrollment.EFINOwnerMobile;
                    isExist.EFINOwnerEmail = enrollment.EFINOwnerEmail;
                    isExist.EFINOwnerIDNumber = enrollment.EFINOwnerIDNumber;
                    isExist.EFINOwnerIDState = enrollment.EFINOwnerIDState;
                    isExist.EFINOwnerEIN = enrollment.EFINOwnerEIN;
                    isExist.SupportOS = enrollment.SupportOS;
                    isExist.BankName = enrollment.BankName;
                    isExist.SBFeeonAll = enrollment.SBFeeonAll;
                    isExist.SBFee = enrollment.SBFee;
                    isExist.TransimissionAddon = enrollment.TransimissionAddon;
                    isExist.PrepaidCardProgram = enrollment.PrepaidCardProgram;
                    isExist.TandC = enrollment.TandC;
                    isExist.EntryLevel = isExist.EntryLevel != 7 ? enrollment.EntryLevel : isExist.EntryLevel;
                    if (isExist.EntryLevel > 6)
                    {
                        isExist.IsEnrtyCompleted = true;
                    }

                    isExist.UpdatedBy = enrollment.UserId;
                    isExist.UpdatedDate = DateTime.Now;

                    db.SaveChanges();
                }
                else
                {
                    BankEnrollmentForRB rb = new BankEnrollmentForRB();
                    rb.ActualNoofBankProductsLastYear = enrollment.ActualNoofBankProductsLastYear;
                    rb.AdvertisingApproval = enrollment.AdvertisingApproval;
                    rb.AltOfficeContact1Email = enrollment.AltOfficeContact1Email;
                    rb.AltOfficeContact1FirstName = enrollment.AltOfficeContact1FirstName;
                    rb.AltOfficeContact1LastName = enrollment.AltOfficeContact1LastName;
                    rb.AltOfficeContact1SSn = enrollment.AltOfficeContact1SSn;
                    rb.AltOfficeContact2Email = enrollment.AltOfficeContact2Email;
                    rb.AltOfficeContact2FirstName = enrollment.AltOfficeContact2FirstName;
                    rb.AltOfficeContact2LastName = enrollment.AltOfficeContact2LastName;
                    rb.AltOfficeContact2SSn = enrollment.AltOfficeContact2SSn;
                    rb.AltOfficePhysicalAddress = enrollment.AltOfficePhysicalAddress;
                    rb.AltOfficePhysicalAddress2 = enrollment.AltOfficePhysicalAddress2;
                    rb.AltOfficePhysicalCity = enrollment.AltOfficePhysicalCity;
                    rb.AltOfficePhysicalState = enrollment.AltOfficePhysicalState;
                    rb.AltOfficePhysicalZipcode = enrollment.AltOfficePhysicalZipcode;
                    rb.AntivirusRequired = enrollment.AntivirusRequired;
                    rb.BankAccountNumber = enrollment.BankAccountNumber;
                    rb.BankAccountType = enrollment.BankAccountType;
                    rb.BankRoutingNumber = enrollment.BankRoutingNumber;
                    rb.BusinessEIN = enrollment.BusinessEIN;

                    rb.OfficeContactDOB = string.IsNullOrEmpty(enrollment.OfficeContactDOB) ? dd : DateTime.ParseExact(enrollment.OfficeContactDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.OfficeManagerPhone = enrollment.OfficeManagerPhone;
                    rb.OfficeManagerCellNo = enrollment.OfficeManagerCellNo;
                    rb.OfficeManagerEmail = enrollment.OfficeManagerEmail;

                    rb.BusinessIncorporation = string.IsNullOrEmpty(enrollment.BusinessIncorporation) ? dd : DateTime.ParseExact(enrollment.BusinessIncorporation.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.BusinessName = enrollment.BusinessName;
                    rb.CellPhoneNumber = enrollment.CellPhoneNumber.Replace("-", "");
                    rb.CheckingAccountName = enrollment.CheckingAccountName;
                    rb.ConsumerLending = enrollment.ConsumerLending;
                    rb.EFINOwnerDOB = string.IsNullOrEmpty(enrollment.EFINOwnerDOB) ? dd : DateTime.ParseExact(enrollment.EFINOwnerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.EFINOwnerFirstName = enrollment.EFINOwnerFirstName;
                    rb.EFINOwnerLastName = enrollment.EFINOwnerLastName;
                    rb.EFINOwnerSSN = enrollment.EFINOwnerSSN;
                    rb.EmailAddress = enrollment.EmailAddress;
                    rb.EROApplicattionDate = string.IsNullOrEmpty(enrollment.EROApplicattionDate) ? dd : DateTime.ParseExact(enrollment.EROApplicattionDate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    rb.EROParticipation = enrollment.EROParticipation;
                    rb.EROReadTAndC = enrollment.EROReadTAndC;
                    rb.FAXNumber = enrollment.FAXNumber;
                    rb.FulfillmentShippingAddress = enrollment.FulfillmentShippingAddress;
                    rb.FulfillmentShippingCity = enrollment.FulfillmentShippingCity;
                    rb.FulfillmentShippingState = enrollment.FulfillmentShippingState;
                    rb.FulfillmentShippingZip = enrollment.FulfillmentShippingZip;
                    rb.HasFirewall = enrollment.HasFirewall;
                    rb.IsAsPerComplainceLaw = enrollment.IsAsPerComplainceLaw;
                    rb.IsAsPerProcessLaw = enrollment.IsAsPerProcessLaw;
                    rb.IsLimitAccess = enrollment.IsLimitAccess;
                    rb.IsLockedStore_Checks = enrollment.IsLockedStore_Checks;
                    rb.IsLockedStore_Documents = enrollment.IsLockedStore_Documents;
                    rb.IsLocked_Office = enrollment.IsLocked_Office;
                    rb.IsMultiOffice = enrollment.IsMultiOffice;
                    rb.IsOfficeTransmit = enrollment.IsOfficeTransmit;
                    rb.IsPTIN = enrollment.IsPTIN;
                    rb.LegarEntityStatus = enrollment.LegarEntityStatus;
                    rb.LLCMembershipRegistration = enrollment.LLCMembershipRegistration;
                    rb.LoginAccesstoEmployees = enrollment.LoginAccesstoEmployees;
                    rb.MailingAddress = enrollment.MailingAddress;
                    rb.MailingCity = enrollment.MailingCity;
                    rb.MailingState = enrollment.MailingState;
                    rb.MailingZip = enrollment.MailingZip;
                    rb.NoofBankProductsLastYear = enrollment.NoofBankProductsLastYear;
                    rb.NoofPersoneel = enrollment.NoofPersoneel;
                    rb.OfficeContactFirstName = enrollment.OfficeContactFirstName;
                    rb.OfficeContactLastName = enrollment.OfficeContactLastName;
                    rb.OfficeContactSSN = enrollment.OfficeContactSSN;
                    rb.OfficeManageLastName = enrollment.OfficeManageLastName;
                    if (!string.IsNullOrEmpty(enrollment.OfficeManagerDOB))
                        rb.OfficeManagerDOB = DateTime.ParseExact(enrollment.OfficeManagerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.OfficeManagerFirstName = enrollment.OfficeManagerFirstName;
                    rb.OfficeManagerSSN = enrollment.OfficeManagerSSN;
                    rb.OfficeName = enrollment.OfficeName;
                    rb.OfficePhoneNumber = enrollment.OfficePhoneNumber.Replace("-", "");
                    rb.OfficePhysicalAddress = enrollment.OfficePhysicalAddress;
                    rb.OfficePhysicalCity = enrollment.OfficePhysicalCity;
                    rb.OfficePhysicalState = enrollment.OfficePhysicalState;
                    rb.OfficePhysicalZip = enrollment.OfficePhysicalZip;
                    rb.OnlineTraining = enrollment.OnlineTraining;
                    if (!string.IsNullOrEmpty(enrollment.OnwerDOB))
                        rb.OnwerDOB = DateTime.ParseExact(enrollment.OnwerDOB.Replace("-", "/"), "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    rb.OwnerAddress = enrollment.OwnerAddress;
                    rb.OwnerCity = enrollment.OwnerCity;
                    rb.OwnerFirstName = enrollment.OwnerFirstName;
                    rb.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    rb.OwnerLastName = enrollment.OwnerLastName;
                    rb.OwnerSSN = enrollment.OwnerSSN;
                    rb.OwnerState = enrollment.OwnerState;
                    rb.OwnerZip = enrollment.OwnerZip;
                    rb.PasswordRequired = enrollment.PasswordRequired;
                    rb.PlantoDispose = enrollment.PlantoDispose;
                    rb.PreviousBankProductFacilitator = enrollment.PreviousBankProductFacilitator;
                    rb.ProductsOffering = enrollment.ProductsOffering;
                    rb.RetailPricingMethod = enrollment.RetailPricingMethod;
                    rb.SPAAmount = enrollment.SPAAmount;
                    rb.SPADate = string.IsNullOrEmpty(enrollment.SPADate) ? dd : DateTime.ParseExact(enrollment.SPADate.Replace("-", "/"), "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture);
                    rb.WebsiteAddress = enrollment.WebsiteAddress;
                    rb.YearsinBusiness = enrollment.YearsinBusiness;

                    rb.CustomerId = enrollment.CustomerId;
                    rb.Id = Guid.NewGuid();
                    rb.EFINOwnerTitle = enrollment.EFINOwnerTitle;
                    rb.EFINOwnerAddress = enrollment.EFINOwnerAddress;
                    rb.EFINOwnerCity = enrollment.EFINOwnerCity;
                    rb.EFINOwnerState = enrollment.EFINOwnerState;
                    rb.EFINOwnerZip = enrollment.EFINOwnerZip;
                    rb.EFINOwnerPhone = enrollment.EFINOwnerPhone;
                    rb.EFINOwnerMobile = enrollment.EFINOwnerMobile;
                    rb.EFINOwnerEmail = enrollment.EFINOwnerEmail;
                    rb.EFINOwnerIDNumber = enrollment.EFINOwnerIDNumber;
                    rb.EFINOwnerIDState = enrollment.EFINOwnerIDState;
                    rb.EFINOwnerEIN = enrollment.EFINOwnerEIN;
                    rb.SupportOS = enrollment.SupportOS;
                    rb.BankName = enrollment.BankName;
                    rb.SBFeeonAll = enrollment.SBFeeonAll;
                    rb.SBFee = enrollment.SBFee;
                    rb.TransimissionAddon = enrollment.TransimissionAddon;
                    rb.PrepaidCardProgram = enrollment.PrepaidCardProgram;
                    rb.TandC = enrollment.TandC;
                    rb.StatusCode = EMPConstants.Active;
                    rb.IsEnrtyCompleted = false;
                    rb.EntryLevel = enrollment.EntryLevel;

                    rb.CreatedBy = enrollment.UserId;
                    rb.CreatedDate = DateTime.Now;
                    rb.UpdatedBy = enrollment.UserId;
                    rb.UpdatedDate = DateTime.Now;

                    db.BankEnrollmentForRBs.Add(rb);
                    db.SaveChanges();

                    //if (enrollment.EntryLevel > 6)
                    //{
                    //    saveEnrollmentCompletedStatus(enrollment.UserId ?? Guid.Empty, enrollment.CustomerId);
                    //}
                }
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveNextRBBankEnrollment", Guid.Empty);
                return false;
            }
        }

        public bool SaveNextRABankEnrollment(RABankEnrollment enrollment)
        {
            try
            {
                var isExist = db.BankEnrollmentForRAs.Where(x => x.CustomerId == enrollment.CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (isExist != null)
                {
                    isExist.BankAccountNumber = enrollment.BankAccountNumber;
                    isExist.BankAccountType = enrollment.BankAccountType;
                    isExist.BankRoutingNumber = enrollment.BankRoutingNumber;
                    isExist.BusinessEIN = enrollment.BusinessEIN;
                    isExist.BusinessFederalIDNumber = enrollment.BusinessFederalIDNumber;
                    isExist.CollectionofBusinessOwners = enrollment.CollectionofBusinessOwners;
                    isExist.CollectionOfOtherOwners = enrollment.CollectionOfOtherOwners;
                    isExist.CorporationType = enrollment.CorporationType;
                    isExist.EFINOwnersSite = enrollment.EFINOwnersSite;
                    isExist.EROMailingCity = enrollment.EROMailingCity;
                    isExist.EROMailingState = enrollment.EROMailingState;
                    isExist.EROMailingZipcode = enrollment.EROMailingZipcode;
                    isExist.EROMaillingAddress = enrollment.EROMaillingAddress;
                    isExist.EROOfficeAddress = enrollment.EROOfficeAddress;
                    isExist.EROOfficeCity = enrollment.EROOfficeCity;
                    isExist.EROOfficeName = enrollment.EROOfficeName;

                    if (!string.IsNullOrEmpty(enrollment.EROOfficePhone))
                    {
                        isExist.EROOfficePhone = enrollment.EROOfficePhone.Replace("-", "");
                    }

                    isExist.EROOfficeState = enrollment.EROOfficeState;
                    isExist.EROOfficeZipCoce = enrollment.EROOfficeZipCoce;
                    isExist.EROShippingAddress = enrollment.EROShippingAddress;
                    isExist.EROShippingCity = enrollment.EROShippingCity;
                    isExist.EROShippingState = enrollment.EROShippingState;
                    isExist.EROShippingState = enrollment.EROShippingState;
                    isExist.EROShippingZip = enrollment.EROShippingZip;
                    isExist.ExpectedCurrentYearVolume = enrollment.ExpectedCurrentYearVolume;
                    isExist.HasAssociatedWithVictims = enrollment.HasAssociatedWithVictims;
                    isExist.IRSAddress = enrollment.IRSAddress;
                    isExist.IRSCity = enrollment.IRSCity;
                    isExist.IRSState = enrollment.IRSState;
                    isExist.IRSZipcode = enrollment.IRSZipcode;
                    isExist.IsLastYearClient = enrollment.IsLastYearClient;
                    isExist.NoofYearsExperience = enrollment.NoofYearsExperience;
                    isExist.OwnerAddress = enrollment.OwnerAddress;

                    if (!string.IsNullOrEmpty(enrollment.OwnerCellPhone))
                    {
                        isExist.OwnerCellPhone = enrollment.OwnerCellPhone.Replace("-", "");
                    }

                    isExist.OwnerCity = enrollment.OwnerCity;

                    if (!string.IsNullOrEmpty(enrollment.OwnerDOB))
                    {
                        isExist.OwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }

                    isExist.OwnerEmail = enrollment.OwnerEmail;
                    isExist.OwnerFirstName = enrollment.OwnerFirstName;
                    if (!string.IsNullOrEmpty(enrollment.OwnerHomePhone))
                    {
                        isExist.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    }
                    isExist.OwnerIssuingState = enrollment.OwnerIssuingState;
                    isExist.OwnerLastName = enrollment.OwnerLastName;
                    isExist.OwnerSSN = enrollment.OwnerSSN;
                    isExist.OwnerState = enrollment.OwnerState;
                    isExist.OwnerStateIssuedIdNumber = enrollment.OwnerStateIssuedIdNumber;
                    isExist.OwnerZipCode = enrollment.OwnerZipCode;
                    isExist.PreviousBankName = enrollment.PreviousBankName;
                    isExist.PreviousYearVolume = enrollment.PreviousYearVolume;

                    isExist.OwnerTitle = enrollment.OwnerTitle;
                    isExist.SbFeeall = enrollment.SbFeeall;
                    isExist.TransmissionAddon = enrollment.TransmissionAddon;
                    isExist.SbFee = enrollment.SbFee;
                    isExist.ElectronicFee = enrollment.ElectronicFee;
                    isExist.AgreeTandC = enrollment.AgreeTandC;
                    isExist.BankName = enrollment.BankName;
                    isExist.AccountName = enrollment.AccountName;
                    isExist.MainContactFirstName = enrollment.MainContactFirstName;
                    isExist.MainContactLastName = enrollment.MainContactLastName;
                    isExist.MainContactPhone = enrollment.MainContactPhone;
                    isExist.TextMessages = enrollment.TextMessages;
                    isExist.LegalIssues = enrollment.LegalIssues;
                    isExist.StateOfIncorporation = enrollment.StateOfIncorporation;
                    isExist.EntryLevel = isExist.EntryLevel != 4 ? enrollment.EntryLevel : isExist.EntryLevel;
                    if (isExist.EntryLevel > 4)
                    {
                        isExist.IsEnrtyCompleted = true;
                    }

                    isExist.UpdatedBy = enrollment.UserId;
                    isExist.UpdatedDate = DateTime.Now;

                    db.SaveChanges();
                    if (isExist.EntryLevel >= 2)
                    {
                        SaveRAEFINOwnerInfo(enrollment.RAEFINOwnerInfo, enrollment.UserId ?? Guid.Empty, isExist.Id);
                    }
                }
                else
                {
                    BankEnrollmentForRA ra = new BankEnrollmentForRA();
                    ra.BankAccountNumber = enrollment.BankAccountNumber;
                    ra.BankAccountType = enrollment.BankAccountType;
                    ra.BankRoutingNumber = enrollment.BankRoutingNumber;
                    ra.BusinessEIN = enrollment.BusinessEIN;
                    ra.BusinessFederalIDNumber = enrollment.BusinessFederalIDNumber;
                    ra.CollectionofBusinessOwners = enrollment.CollectionofBusinessOwners;
                    ra.CollectionOfOtherOwners = enrollment.CollectionOfOtherOwners;
                    ra.CorporationType = enrollment.CorporationType;
                    ra.EFINOwnersSite = enrollment.EFINOwnersSite;
                    ra.EROMailingCity = enrollment.EROMailingCity;
                    ra.EROMailingState = enrollment.EROMailingState;
                    ra.EROMailingZipcode = enrollment.EROMailingZipcode;
                    ra.EROMaillingAddress = enrollment.EROMaillingAddress;
                    ra.EROOfficeAddress = enrollment.EROOfficeAddress;
                    ra.EROOfficeCity = enrollment.EROOfficeCity;
                    ra.EROOfficeName = enrollment.EROOfficeName;
                    if (!string.IsNullOrEmpty(enrollment.EROOfficePhone))
                    {
                        ra.EROOfficePhone = enrollment.EROOfficePhone.Replace("-", "");
                    }
                    ra.EROOfficeState = enrollment.EROOfficeState;
                    ra.EROOfficeZipCoce = enrollment.EROOfficeZipCoce;
                    ra.EROShippingAddress = enrollment.EROShippingAddress;
                    ra.EROShippingCity = enrollment.EROShippingCity;
                    ra.EROShippingState = enrollment.EROShippingState;
                    ra.EROShippingZip = enrollment.EROShippingZip;
                    ra.ExpectedCurrentYearVolume = enrollment.ExpectedCurrentYearVolume;
                    ra.HasAssociatedWithVictims = enrollment.HasAssociatedWithVictims;
                    ra.IRSAddress = enrollment.IRSAddress;
                    ra.IRSCity = enrollment.IRSCity;
                    ra.IRSState = enrollment.IRSState;
                    ra.IRSZipcode = enrollment.IRSZipcode;
                    ra.IsLastYearClient = enrollment.IsLastYearClient;
                    ra.NoofYearsExperience = enrollment.NoofYearsExperience;
                    ra.OwnerAddress = enrollment.OwnerAddress;
                    ra.StateOfIncorporation = enrollment.StateOfIncorporation;

                    if (!string.IsNullOrEmpty(enrollment.OwnerCellPhone))
                    {
                        ra.OwnerCellPhone = enrollment.OwnerCellPhone.Replace("-", "");
                    }

                    ra.OwnerCity = enrollment.OwnerCity;

                    if (!string.IsNullOrEmpty(enrollment.OwnerDOB))
                    {
                        ra.OwnerDOB = DateTime.ParseExact(enrollment.OwnerDOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
                    }

                    ra.OwnerEmail = enrollment.OwnerEmail;
                    ra.OwnerFirstName = enrollment.OwnerFirstName;

                    if (!string.IsNullOrEmpty(enrollment.OwnerHomePhone))
                    {
                        ra.OwnerHomePhone = enrollment.OwnerHomePhone.Replace("-", "");
                    }

                    ra.OwnerIssuingState = enrollment.OwnerIssuingState;
                    ra.OwnerLastName = enrollment.OwnerLastName;
                    ra.OwnerSSN = enrollment.OwnerSSN;
                    ra.OwnerState = enrollment.OwnerState;
                    ra.OwnerStateIssuedIdNumber = enrollment.OwnerStateIssuedIdNumber;
                    ra.OwnerZipCode = enrollment.OwnerZipCode;
                    ra.PreviousBankName = enrollment.PreviousBankName;
                    ra.PreviousYearVolume = enrollment.PreviousYearVolume;

                    ra.CustomerId = enrollment.CustomerId;
                    int? efin = db.emp_CustomerInformation.Where(x => x.Id == enrollment.CustomerId).Select(x => x.EFIN).FirstOrDefault();
                    ra.EfinID = efin == null ? "" : efin.ToString().PadLeft(6, '0');
                    ra.Id = Guid.NewGuid();

                    ra.OwnerTitle = enrollment.OwnerTitle;
                    ra.SbFeeall = enrollment.SbFeeall;
                    ra.TransmissionAddon = enrollment.TransmissionAddon;
                    ra.SbFee = enrollment.SbFee;
                    ra.ElectronicFee = enrollment.ElectronicFee;
                    ra.AgreeTandC = enrollment.AgreeTandC;
                    ra.BankName = enrollment.BankName;
                    ra.AccountName = enrollment.AccountName;
                    ra.MainContactFirstName = enrollment.MainContactFirstName;
                    ra.MainContactLastName = enrollment.MainContactLastName;
                    ra.MainContactPhone = enrollment.MainContactPhone;
                    ra.TextMessages = enrollment.TextMessages;
                    ra.LegalIssues = enrollment.LegalIssues;
                    ra.StatusCode = EMPConstants.Active;
                    ra.IsEnrtyCompleted = false;
                    ra.EntryLevel = ra.EntryLevel != 4 ? enrollment.EntryLevel : ra.EntryLevel;

                    ra.CreatedBy = enrollment.UserId;
                    ra.CreatedDate = DateTime.Now;
                    ra.UpdatedBy = enrollment.UserId;
                    ra.UpdatedDate = DateTime.Now;

                    db.BankEnrollmentForRAs.Add(ra);
                    db.SaveChanges();
                    if (ra.EntryLevel == 2)
                    {
                        SaveRAEFINOwnerInfo(enrollment.RAEFINOwnerInfo, enrollment.UserId ?? Guid.Empty, ra.Id);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveNextRABankEnrollment", Guid.Empty);
                return false;
            }
        }

        public TPGBankEnrollment getTPGBankObjectInfo(Guid CustomerId)
        {
            TPGBankEnrollment tpg = new DTO.TPGBankEnrollment();
            bool _isExist = false;
            try
            {
                var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (info != null)
                {
                    var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                    //string CLPassword = PasswordManager.DecryptText(logininfo.CrossLinkPassword);

                    string MasterId = logininfo.MasterIdentifier;
                    string Password = "";
                    int CrossLinkUserId = 0;
                    getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, logininfo.MasterIdentifier, info.ParentId ?? Guid.Empty, info.EntityId ?? 0);

                    string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = MasterId;
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            int efinid = _apiObj.getEFINID(info.EFIN.Value);
                            var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, "S");//TPG - S
                            if (latestbankApp.BankAppID != 0)
                            {
                                var bankinfo = _apiObj.getSBTPGApp(_objAuth, latestbankApp.BankAppID);
                                if (bankinfo.Response.success)
                                {
                                    _isExist = true;
                                    tpg.BankStatus = latestbankApp.RegisteredDescription;
                                    tpg.ManagerFirstName = bankinfo.ManagerFName;
                                    tpg.ManagerLastName = bankinfo.ManagerLName;
                                    tpg.ManagerEmail = bankinfo.ManagerEmail;
                                    tpg.LastYearBank = bankinfo.RalBankLY;
                                    tpg.LastYearEFIN = bankinfo.EFINLY.HasValue ? bankinfo.EFINLY.Value.ToString() : "";
                                    tpg.LastYearVolume = bankinfo.VolumeLY.HasValue ? bankinfo.VolumeLY.Value.ToString() : "";
                                    tpg.BankProductFund = bankinfo.BankProductsLY.HasValue ? bankinfo.BankProductsLY.Value.ToString() : "";
                                    tpg.CheckPrint = bankinfo.CheckPrint;
                                    tpg.CompanyName = bankinfo.CompanyName;
                                    tpg.OfficeAddress = bankinfo.OfficeAddr;
                                    tpg.OfficeCity = bankinfo.OfficeCity;
                                    tpg.OfficeState = bankinfo.OfficeState;
                                    tpg.OfficeTelephone = bankinfo.OfficePhone;
                                    tpg.OfficeZip = bankinfo.OfficeZip;
                                    tpg.OfficeFax = bankinfo.OfficeFax == null ? "" : bankinfo.OfficeFax;
                                    tpg.ShippingAddress = bankinfo.ShipAddress;
                                    tpg.ShippingCity = bankinfo.ShipCity;
                                    tpg.ShippingState = bankinfo.ShipState;
                                    tpg.ShippingZip = bankinfo.ShipZip;
                                    tpg.Addonfee = bankinfo.EROTranFee.HasValue ? bankinfo.EROTranFee.Value.ToString("0.00") : "";
                                    tpg.ServiceBureaufee = bankinfo.SBPrepFee.HasValue ? bankinfo.SBPrepFee.Value.ToString("0.00") : "";
                                    tpg.DocPrepFee = bankinfo.DocPrepFee.HasValue ? bankinfo.DocPrepFee.Value.ToString("0.00") : "";
                                    tpg.LatestAppRawXml = latestbankApp;
                                    tpg.TPGRawXml = bankinfo;
                                }
                            }
                            var efinobj = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                            tpg.OwnerEIN = efinobj.EIN;
                            tpg.OwnerSSn = efinobj.SSN;
                            tpg.OwnerFirstName = efinobj.FName;
                            tpg.OwnerLastName = efinobj.LName;
                            tpg.EfinOwnerTitle = efinobj.Title;
                            tpg.OwnerTelephone = efinobj.Phone;
                            tpg.EfinOwnerMobile = efinobj.Mobile;
                            tpg.OwnerCity = efinobj.City;
                            tpg.OwnerState = efinobj.State;
                            tpg.OwnerZip = efinobj.Zip;
                            tpg.OwnerDOB = efinobj.DOB.HasValue ? efinobj.DOB.Value.ToString("MM'/'dd'/'yyyy") : "";
                            tpg.EfinIDNumber = efinobj.IDNumber;
                            tpg.EfinIdState = efinobj.IDState;
                            tpg.OwnerEmail = efinobj.Email;
                            tpg.OwnerAddress = efinobj.Address;
                            tpg.BankName = efinobj.BankName;
                            tpg.AccountName = efinobj.AcctName;
                            tpg.OfficeRTN = efinobj.RTN;
                            tpg.OfficeDAN = efinobj.DAN;
                            tpg.AccountType = efinobj.AcctType;
                            tpg.SbfeeAll = efinobj.SBFeeAll;
                            tpg.EfinRawXml = efinobj;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getTPGBankObjectInfo", CustomerId);
                _isExist = false;
            }
            if (_isExist)
                return tpg;
            else
                return null;
        }

        public RABankEnrollment getRABankObjectInfo(Guid CustomerId)
        {
            RABankEnrollment ra = new DTO.RABankEnrollment();
            bool _isExist = false;
            try
            {
                var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (info != null)
                {
                    var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                    //string CLPassword = PasswordManager.DecryptText(logininfo.CrossLinkPassword);

                    string MasterId = logininfo.MasterIdentifier;
                    string Password = "";
                    int CrossLinkUserId = 0;
                    getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, logininfo.MasterIdentifier, info.ParentId ?? Guid.Empty, info.EntityId ?? 0);

                    string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = MasterId;
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            int efinid = _apiObj.getEFINID(info.EFIN.Value);
                            var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, "V");

                            if (latestbankApp.BankAppID != 0)
                            {
                                var bankinfo = _apiObj.getRefundAdvantageApp(_objAuth, latestbankApp.BankAppID);
                                if (bankinfo.Response.success)
                                {
                                    _isExist = true;
                                    ra.BankStatus = latestbankApp.RegisteredDescription;
                                    ra.MainContactFirstName = bankinfo.ContactFirstName;
                                    ra.MainContactLastName = bankinfo.ContactLastName;
                                    ra.MainContactPhone = bankinfo.ContactPhone;
                                    ra.EROMaillingAddress = bankinfo.MailingAddressStreet;
                                    ra.EROMailingCity = bankinfo.MailingAddressCity;
                                    ra.EROMailingState = bankinfo.MailingAddressState;
                                    ra.EROMailingZipcode = bankinfo.MailingAddressZip;
                                    ra.CorporationType = bankinfo.CorporationType;

                                    var owners = _apiObj.getRefundAdvantageOwners(_objAuth, latestbankApp.BankAppID);
                                    var ownerlist = (from s in owners
                                                     select new EnrollmentBankEFINOwnerRADTO
                                                     {
                                                         Address = s.Address,
                                                         BankEnrollmentRAId = Guid.Empty,
                                                         City = s.City,
                                                         DateofBirth = s.DOB.HasValue ? s.DOB.Value.ToString("dd'/'MM'/'yyyy") : "",
                                                         EmailId = "",
                                                         FirstName = s.FirstName,
                                                         HomePhone = s.Phone,
                                                         Id = Guid.Empty,
                                                         IDNumber = s.IdNumber,
                                                         IDState = s.IdState,
                                                         LastName = s.LastName,
                                                         MobilePhone = "",
                                                         PercentageOwned = s.PercentOwned.HasValue ? s.PercentOwned.Value : 0,
                                                         SSN = s.SSN,
                                                         StateId = s.State,
                                                         ZipCode = s.Zip
                                                     }).ToList();
                                    ra.RAEFINOwnerInfo = ownerlist;
                                    ra.PreviousYearVolume = bankinfo.PriorYearVolume.HasValue ? bankinfo.PriorYearVolume.Value : 0;
                                    ra.ExpectedCurrentYearVolume = bankinfo.CurrentYearVolume.HasValue ? bankinfo.CurrentYearVolume.Value : 0;
                                    ra.PreviousBankName = bankinfo.PriorYearBank;
                                    ra.NoofYearsExperience = bankinfo.ExperienceFiling.HasValue ? bankinfo.ExperienceFiling.Value : 0;
                                    ra.EROOfficeAddress = bankinfo.OfficeAddressStreet;
                                    ra.EROOfficeCity = bankinfo.OfficeAddressCity;
                                    ra.EROOfficeState = bankinfo.OfficeAddressState;
                                    ra.EROOfficeZipCoce = bankinfo.OfficeAddressZip;
                                    ra.EROOfficePhone = bankinfo.OfficePhone;
                                    ra.EROMaillingAddress = bankinfo.MailingAddressStreet;
                                    ra.EROMailingCity = bankinfo.MailingAddressCity;
                                    ra.EROMailingState = bankinfo.MailingAddressState;
                                    ra.EROMailingZipcode = bankinfo.MailingAddressZip;
                                    ra.EROShippingAddress = bankinfo.ShippingAddressStreet;
                                    ra.EROShippingCity = bankinfo.ShippingAddressCity;
                                    ra.EROShippingState = bankinfo.ShippingAddressState;
                                    ra.EROShippingZip = bankinfo.ShippingAddressZip;
                                    ra.CorporationType = bankinfo.CorporationType;
                                    ra.TransmissionAddon = bankinfo.EROTranFee.HasValue ? bankinfo.EROTranFee.Value.ToString("0.00") : "";
                                    ra.SbFee = bankinfo.SBFee.HasValue ? bankinfo.SBFee.Value.ToString("0.00") : "";
                                    ra.ElectronicFee = bankinfo.EFilingFee.HasValue ? bankinfo.EFilingFee.Value.ToString("0.00") : "";
                                    ra.AgreeTandC = true;
                                    ra.TextMessages = bankinfo.TextMessage.Value;
                                    ra.LegalIssues = bankinfo.LegalIssues.Value;
                                    ra.StateOfIncorporation = bankinfo.StateofIncorporation;
                                    ra.LatestAppRawXml = latestbankApp;
                                    ra.RARawXml = bankinfo;
                                }
                            }
                            var efinobj = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                            ra.OwnerFirstName = efinobj.FName;
                            ra.OwnerLastName = efinobj.LName;
                            ra.OwnerSSN = efinobj.SSN;
                            ra.OwnerTitle = efinobj.Title;
                            ra.OwnerDOB = efinobj.DOB.HasValue ? efinobj.DOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                            ra.OwnerEmail = efinobj.Email;
                            ra.OwnerCellPhone = efinobj.Mobile;
                            ra.OwnerHomePhone = efinobj.Phone;
                            ra.OwnerAddress = efinobj.Address;
                            ra.OwnerCity = efinobj.City;
                            ra.OwnerState = efinobj.State;
                            ra.OwnerZipCode = efinobj.Zip;
                            ra.OwnerStateIssuedIdNumber = efinobj.IDNumber;
                            ra.OwnerIssuingState = efinobj.IDState;
                            ra.EROOfficeName = efinobj.Company;
                            ra.BusinessEIN = efinobj.EIN;
                            ra.BankRoutingNumber = efinobj.RTN;
                            ra.BankAccountNumber = efinobj.DAN;
                            ra.BankAccountType = efinobj.AcctType;
                            ra.SbFeeall = efinobj.SBFeeAll;
                            ra.BankName = efinobj.BankName;
                            ra.AccountName = efinobj.AcctName;
                            ra.EfinRawXml = efinobj;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getRABankObjectInfo", CustomerId);
                _isExist = false;
            }
            if (_isExist)
                return ra;
            else
                return null;
        }

        public RBBankEnrollment getRBBankObjectInfo(Guid CustomerId)
        {
            RBBankEnrollment rb = new DTO.RBBankEnrollment();
            bool _isExist = false;
            try
            {
                var info = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (info != null)
                {
                    var logininfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).FirstOrDefault();
                    //string CLPassword = PasswordManager.DecryptText(logininfo.CrossLinkPassword);

                    string MasterId = logininfo.MasterIdentifier;
                    string Password = "";
                    int CrossLinkUserId = 0;
                    getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, logininfo.CrossLinkUserId, logininfo.CLAccountId, logininfo.CLAccountPassword, logininfo.CLLogin, logininfo.MasterIdentifier, info.ParentId ?? Guid.Empty, info.EntityId ?? 0);

                    string _accesskey = _apiObj.getAccessKey(logininfo.MasterIdentifier, Password);
                    if (_accesskey != "")
                    {
                        AuthObject _objAuth = new AuthObject();
                        _objAuth.AccessKey = _accesskey;
                        _objAuth.UserID = MasterId;
                        XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                        if (isValid.success)
                        {
                            int efinid = _apiObj.getEFINID(info.EFIN.Value);
                            var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, "R");
                            if (latestbankApp.BankAppID != 0)
                            {
                                var bankinfo = _apiObj.getRepublicApp(_objAuth, latestbankApp.BankAppID);
                                if (bankinfo.Response.success)
                                {
                                    _isExist = true;
                                    rb.BankStatus = latestbankApp.RegisteredDescription;
                                    rb.OfficeContactFirstName = bankinfo.OfficeContactFirstName;
                                    rb.OfficeContactLastName = bankinfo.OfficeContactLastName;
                                    rb.OfficeContactSSN = bankinfo.OfficeContactSSN;
                                    rb.CellPhoneNumber = bankinfo.CellPhoneNumber;
                                    rb.OfficeManagerFirstName = bankinfo.OfficeManagerFirstName;
                                    rb.OfficeManageLastName = bankinfo.OfficeManagerLastName;
                                    rb.OfficeManagerDOB = bankinfo.OfficeManagerDOB.HasValue ? bankinfo.OfficeManagerDOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                                    rb.OfficeManagerSSN = bankinfo.OfficeManagerSSN;
                                    rb.MailingAddress = bankinfo.MailingAddress;
                                    rb.MailingCity = bankinfo.MailingCity;
                                    rb.MailingState = bankinfo.MailingState;
                                    rb.MailingZip = bankinfo.MailingZip;
                                    rb.WebsiteAddress = bankinfo.WebsiteAddress;
                                    rb.YearsinBusiness = bankinfo.YearsInBusiness.HasValue ? bankinfo.YearsInBusiness.Value : 0;
                                    rb.NoofBankProductsLastYear = bankinfo.LastYearBankProducts.HasValue ? bankinfo.LastYearBankProducts.Value : 0;
                                    rb.PreviousBankProductFacilitator = bankinfo.BankProductFacilitator;
                                    rb.OwnerFirstName = bankinfo.OwnerFirstName;
                                    rb.OwnerLastName = bankinfo.OwnerLastName;
                                    rb.OwnerHomePhone = bankinfo.OwnerHomePhone;
                                    rb.OwnerSSN = bankinfo.OwnerSSN;
                                    rb.OwnerState = bankinfo.OwnerState;
                                    rb.OnwerDOB = bankinfo.OwnerDOB.HasValue ? bankinfo.OwnerDOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                                    rb.OwnerAddress = bankinfo.OwnerAddress;
                                    rb.OwnerCity = bankinfo.OwnerCity;
                                    rb.OwnerZip = bankinfo.OwnerZip;
                                    rb.LegarEntityStatus = bankinfo.LegalEntityStatusInd;
                                    rb.LLCMembershipRegistration = bankinfo.LLCMembershipRegistration;
                                    rb.BusinessEIN = bankinfo.EIN;
                                    rb.OfficeName = bankinfo.OfficeName;
                                    rb.EmailAddress = bankinfo.EmailAddress;
                                    rb.OfficePhysicalAddress = bankinfo.OfficePhysicalStreet;
                                    rb.OfficePhysicalCity = bankinfo.OfficePhysicalCity;
                                    rb.OfficePhysicalState = bankinfo.OfficePhysicalState;
                                    rb.OfficePhysicalZip = bankinfo.OfficePhysicalZip;
                                    rb.OfficePhoneNumber = bankinfo.OfficePhoneNumber;
                                    rb.OfficeManagerPhone = bankinfo.OfficePhoneNumber;
                                    rb.OfficeManagerCellNo = bankinfo.CellPhoneNumber;
                                    rb.OfficeManagerEmail = bankinfo.EmailAddress;
                                    rb.MailingAddress = bankinfo.MailingAddress;
                                    rb.MailingCity = bankinfo.MailingCity;
                                    rb.MailingState = bankinfo.MailingState;
                                    rb.MailingZip = bankinfo.MailingZip;
                                    rb.FulfillmentShippingAddress = bankinfo.FulfillmentShippingStreet;
                                    rb.FulfillmentShippingCity = bankinfo.FulfillmentShippingCity;
                                    rb.FulfillmentShippingState = bankinfo.FulfillmentShippingState;
                                    rb.FulfillmentShippingZip = bankinfo.FulfillmentShippingZip;
                                    rb.ActualNoofBankProductsLastYear = bankinfo.ActualNumberBankProducts.HasValue ? bankinfo.ActualNumberBankProducts.Value : 0;
                                    rb.BusinessName = bankinfo.OfficeName;
                                    rb.IsMultiOffice = bankinfo.MultiOffice;
                                    rb.ProductsOffering = Convert.ToInt32(bankinfo.DebitProgramInd);
                                    rb.IsPTIN = bankinfo.PTINInd;
                                    rb.IsAsPerProcessLaw = bankinfo.TaxPrepLicensing;
                                    rb.IsAsPerComplainceLaw = bankinfo.ComplianceWithLawInd;
                                    rb.ConsumerLending = bankinfo.PreviousViolationFineInd;
                                    rb.NoofPersoneel = bankinfo.NumOfPersonnel.Value;
                                    rb.AdvertisingApproval = bankinfo.AdvertisingInd;
                                    rb.IsLockedStore_Documents = bankinfo.DocumentStorageInd;
                                    rb.IsLockedStore_Checks = bankinfo.CheckCardStorageInd;
                                    rb.IsLocked_Office = bankinfo.OfficeDoorInd;
                                    rb.IsLimitAccess = bankinfo.DocumentAccessInd;
                                    rb.PlantoDispose = bankinfo.SensitiveDocumentDestInd;
                                    rb.LoginAccesstoEmployees = bankinfo.LoginPassInd;
                                    rb.AntivirusRequired = bankinfo.AntiVirusInd;
                                    rb.HasFirewall = bankinfo.FirewallInd;
                                    rb.OnlineTraining = bankinfo.ProductTrainingInd;
                                    rb.PasswordRequired = bankinfo.WirelessInd;
                                    rb.SupportOS = bankinfo.SupportedOsInd;
                                    rb.SBFee = bankinfo.SBPrepFee.HasValue ? bankinfo.SBPrepFee.Value.ToString("0.00") : "";
                                    rb.TransimissionAddon = bankinfo.EROTranFee.HasValue ? bankinfo.EROTranFee.Value.ToString("0.00") : "";
                                    rb.PrepaidCardProgram = bankinfo.CardProgram;
                                    rb.LatestAppRawXml = latestbankApp;
                                    rb.RBRawXml = bankinfo;
                                }
                            }
                            var efinobj = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                            rb.EFINOwnerFirstName = efinobj.FName;
                            rb.EFINOwnerLastName = efinobj.LName;
                            rb.EFINOwnerTitle = efinobj.Title;
                            rb.EFINOwnerSSN = efinobj.SSN;
                            rb.EFINOwnerAddress = efinobj.Address;
                            rb.EFINOwnerCity = efinobj.City;
                            rb.EFINOwnerDOB = efinobj.DOB.HasValue ? efinobj.DOB.Value.ToString("dd'/'MM'/'yyyy") : "";
                            rb.EFINOwnerEIN = efinobj.EIN;
                            rb.EFINOwnerEmail = efinobj.Email;
                            rb.EFINOwnerMobile = efinobj.Mobile;
                            rb.EFINOwnerPhone = efinobj.Phone;
                            rb.EFINOwnerState = efinobj.State;
                            rb.EFINOwnerZip = efinobj.Zip;
                            rb.CheckingAccountName = efinobj.AcctName;
                            rb.BankRoutingNumber = efinobj.RTN;
                            rb.BankAccountNumber = efinobj.DAN;
                            rb.BankAccountType = efinobj.AcctType;
                            rb.EFINOwnerIDNumber = efinobj.IDNumber;
                            rb.EFINOwnerIDState = efinobj.IDState;
                            rb.BankName = efinobj.BankName;
                            rb.SBFeeonAll = efinobj.SBFeeAll;
                            rb.EfinRawXml = efinobj;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getRBBankObjectInfo", CustomerId);
                _isExist = false;
            }
            if (_isExist)
                return rb;
            else
                return null;
        }

        private string getLatestAppXML(AppObject app)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(AppObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, app);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getSBTPGXML(SBTPGAppObject app)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(SBTPGAppObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, app);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getRefundAdvantageXML(RefundAdvantageAppObject appObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(RefundAdvantageAppObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, appObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getRepublicXML(RepublicAppObject appObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(RepublicAppObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, appObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getEfinObjecyXml(EfinObject obj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(EfinObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, obj);
                var xml = sww.ToString();
                return xml.Replace(">", ">\n");
            }
        }

        private string getRAOwnerObjecyXml(RefundAdvantageOwnerObject _onwerObj)
        {
            XmlSerializer xsSubmit = new XmlSerializer(typeof(RefundAdvantageOwnerObject));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, _onwerObj);
                var xml = sww.ToString();
                return xml;
            }
        }

        private string getsubmitBankAppXml(AuthObject _objAuth, int bankappId, int efinid, string accountId, string bankCode, string updatedby)
        {
            SubmitAppModel objModel = new SubmitAppModel();
            objModel.AccountId = accountId;
            objModel.Auth = _objAuth;
            objModel.BankAppId = bankappId;
            objModel.BankCode = bankCode;
            objModel.EFINId = efinid;
            objModel.UpdatedBy = updatedby;

            XmlSerializer xsSubmit = new XmlSerializer(typeof(SubmitAppModel));
            StringWriter sww = new StringWriter();
            using (XmlWriter writer = XmlWriter.Create(sww))
            {
                xsSubmit.Serialize(writer, objModel);
                var xml = sww.ToString();
                return xml;
            }
        }

        public bool AcceptBankEnrollment(Guid CustomerId, Guid UserId, Guid bankid)
        {
            try
            {
                var enrollment = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == bankid).FirstOrDefault();
                if (enrollment != null)
                {
                    enrollment.StatusCode = EMPConstants.Approved;
                    enrollment.UpdatedDate = DateTime.Now;
                    enrollment.UpdatedBy = UserId;
                    db.SaveChanges();

                    var apved = db.EnrollmentBankSelections.Where(x => x.BankId != bankid && x.CustomerId == CustomerId && x.BankSubmissionStatus == 1 && x.StatusCode == EMPConstants.Active).Count();
                    if (apved <= 0)
                    {
                        var dd = db.EnrollmentBankSelections.Where(x => x.BankId == bankid && x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                        if (dd != null)
                        {
                            dd.BankSubmissionStatus = 1;
                            dd.LastUpdatedBy = UserId;
                            dd.LastUpdatedDate = DateTime.Now;
                            db.SaveChanges();
                        }
                    }

                    BankEnrollmentHistory history = new BankEnrollmentHistory();
                    history.BankId = enrollment.BankId;

                    history.CustomerId = CustomerId;
                    history.EnrollmentId = enrollment.Id;
                    history.Message = EMPConstants.BankApproved;
                    history.Paramaeters = "";
                    history.Status = true;

                    history.CreatedBy = UserId;
                    history.CreatedDate = DateTime.Now;
                    history.UpdatedBy = UserId;
                    history.UpdatedDate = DateTime.Now;

                    db.BankEnrollmentHistories.Add(history);
                    db.SaveChanges();

                    BankEnrollmentStatu status = new BankEnrollmentStatu();
                    status.CreatedDate = DateTime.Now;
                    status.EnrollmentId = enrollment.Id;
                    status.Id = Guid.NewGuid();
                    status.IsUnlocked = false;
                    status.Reason = "";
                    status.Status = EMPConstants.ApprovedService;
                    status.UpdatedBy = UserId;
                    status.UpdatedDate = DateTime.Now;
                    db.BankEnrollmentStatus.Add(status);
                    db.SaveChanges();

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/AcceptBankEnrollment", CustomerId);
                return false;
            }
        }

        public bool RejectBankEnrollment(Guid CustomerId, Guid UserId, Guid bankid)
        {
            try
            {
                var enrollment = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == bankid).FirstOrDefault();
                if (enrollment != null)
                {
                    enrollment.StatusCode = EMPConstants.Rejected;
                    enrollment.UpdatedDate = DateTime.Now;
                    enrollment.UpdatedBy = UserId;
                    db.SaveChanges();

                    BankEnrollmentHistory history = new BankEnrollmentHistory();
                    history.BankId = enrollment.BankId;
                    history.CreatedDate = DateTime.Now;
                    history.CustomerId = CustomerId;
                    history.EnrollmentId = enrollment.Id;
                    history.Message = EMPConstants.BankRejected;
                    history.Paramaeters = "";
                    history.Status = true;

                    history.CreatedBy = UserId;
                    history.CreatedDate = DateTime.Now;
                    history.UpdatedBy = UserId;
                    history.UpdatedDate = DateTime.Now;

                    db.BankEnrollmentHistories.Add(history);
                    db.SaveChanges();

                    BankEnrollmentStatu status = new BankEnrollmentStatu();
                    status.CreatedDate = DateTime.Now;
                    status.EnrollmentId = enrollment.Id;
                    status.Id = Guid.NewGuid();
                    status.IsUnlocked = false;
                    status.Reason = "";
                    status.Status = EMPConstants.RejectedService;
                    status.UpdatedBy = UserId;
                    status.UpdatedDate = DateTime.Now;
                    db.BankEnrollmentStatus.Add(status);
                    db.SaveChanges();

                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/RejectBankEnrollment", CustomerId);
                return false;
            }
        }

        public XlinkResponseModel SubmitBankApptoXlink(Guid CustomerId, Guid UserId, Guid bankid)
        {
            try
            {
                var bank = (from x in db.EnrollmentBankSelections
                            join b in db.BankMasters on x.BankId equals b.Id
                            where x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == bankid
                            select new { x.BankId, b.BankCode }).FirstOrDefault();
                var customer = (from b in db.BankEnrollments
                                join c in db.emp_CustomerInformation on b.CustomerId equals c.Id
                                join l in db.emp_CustomerLoginInformation on c.Id equals l.CustomerOfficeId
                                where b.CustomerId == CustomerId && b.IsActive == true
                                && b.BankId == bankid
                                select new { c.EFIN, b.BankId, b.CustomerId, l.CrossLinkUserId, l.CrossLinkPassword, l.MasterIdentifier, b.Id, c.ParentId, c.EntityId, l.CLAccountId, l.CLAccountPassword, l.CLLogin }).FirstOrDefault();

                string MasterId = ""; //customer.MasterIdentifier;
                string Password = ""; //PasswordManager.DecryptText(customer.CrossLinkPassword);
                int CrossLinkUserId = 0;

                getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, customer.CrossLinkUserId, customer.CLAccountId, customer.CLAccountPassword, customer.CLLogin, customer.MasterIdentifier, customer.ParentId ?? Guid.Empty, customer.EntityId ?? 0);


                //bool IsMso = false;
                //if (customer.ParentId != null && customer.ParentId != Guid.Empty)
                //{
                //    var parentmso = db.emp_CustomerInformation.Where(x => x.Id == customer.ParentId).FirstOrDefault();
                //    if (parentmso.IsMSOUser ?? false)
                //        IsMso = true;
                //}

                //if (customer.EntityId == (int)EMPConstants.Entity.SVB_MO_AE_SS || customer.EntityId == (int)EMPConstants.Entity.SVB_AE_SS || customer.EntityId == (int)EMPConstants.Entity.MO_AE_SS || customer.EntityId == (int)EMPConstants.Entity.SOME_SS || IsMso)
                //{
                //    var parent = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == customer.ParentId.Value).FirstOrDefault();
                //    if (parent != null)
                //    {
                //        MasterId = parent.MasterIdentifier;
                //        Password = PasswordManager.DecryptText(parent.CrossLinkPassword);
                //        CrossLinkUserId = Convert.ToInt32(parent.CrossLinkUserId);
                //    }
                //}
                //else
                //    CrossLinkUserId = Convert.ToInt32(customer.CrossLinkUserId);

                string bankCode = bank.BankCode;
                Guid bankId = bank.BankId;
                int efin = customer.EFIN.HasValue ? customer.EFIN.Value : 0;
                Guid EnrollmentId = customer.Id;
                string ParentId = customer.ParentId.HasValue ? customer.ParentId.Value.ToString() : "";

                var efinresult = checkandUpdateEFIN(efin, MasterId, Password, ParentId, CustomerId, CrossLinkUserId, bankId, EnrollmentId, bankCode);

                if (!efinresult.Status)
                {
                    SaveEnrollmentHistory("Efin Update failed :: " + string.Join(",", efinresult.Messages.ToArray()), CustomerId, 0, bankId, EnrollmentId, "");
                    //SaveInvalidRecord(CustomerId, bankCode);
                    var errMsg = efinresult.Messages.ToList();
                    return new XlinkResponseModel() { IsXlink = true, Messages = errMsg, Status = efinresult.Status };
                }

                string _accesskey = _apiObj.getAccessKey(MasterId, Password);
                if (_accesskey != "")
                {
                    AuthObject _objAuth = new AuthObject();
                    _objAuth.AccessKey = _accesskey;
                    _objAuth.UserID = MasterId;

                    // checking the Authentication
                    XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                    if (isValid.success)
                    {
                        SaveEnrollmentHistory("Updating the application for Customer " + CustomerId + ", bank " + bankCode, CustomerId, 1, bankId, EnrollmentId);

                        int efinid = _apiObj.getEFINID(customer.EFIN.HasValue ? customer.EFIN.Value : 0);
                        var efinobj = _apiObj.getEFINbyEFIN(_objAuth, efin, MasterId, 0);
                        efinid = efinobj.EfinID;
                        var efinres = new XlinkResponseModel();

                        var isSecond = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true &&
                                    (x.StatusCode == EMPConstants.Approved || x.StatusCode == EMPConstants.EnrPending || x.StatusCode == EMPConstants.Submitted ||
                                    x.StatusCode == EMPConstants.Rejected || x.StatusCode == EMPConstants.Denied)).FirstOrDefault();
                        if (isSecond == null)
                        {
                            if (!(efinobj.Locked ?? false))
                                efinres = UpdateEfinObject(_objAuth, efin, efinid, MasterId, CustomerId, ParentId, CrossLinkUserId, bankId, EnrollmentId, bankCode);
                            else
                            {
                                efinres.Status = true;
                            }
                        }
                        else
                        {
                            efinres.Status = true;
                        }
                        if (efinres.Status)
                        {
                            if (efinid == 0)
                                efinid = _apiObj.getEFINbyEFIN(_objAuth, efin, MasterId, 0).EfinID;

                            XlinkResponse result = new XlinkResponse();

                            // getting the latest App by bank
                            var latestbankApp = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode);
                            if (bankCode == EMPConstants.TPGBank)
                            {
                                // updating the SBTPG bank
                                result = UpdateTPGBankApp(CustomerId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, CrossLinkUserId, bankId, EnrollmentId);
                            }
                            else if (bankCode == EMPConstants.RABank)
                            {
                                result = UpdateRABankApp(CustomerId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, CrossLinkUserId, bankId, EnrollmentId);
                                if (result.success)
                                {
                                    //var bankapps = _apiObj.getBankApps(_objAuth, efinid);
                                    var appid = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode).BankAppID;
                                    var ownerres = UpdateRAOwnerObject(CustomerId, appid, _objAuth, CrossLinkUserId, EnrollmentId);

                                    result = ownerres;
                                }
                            }
                            else if (bankCode == EMPConstants.RBBank)
                            {
                                // updating republic Bank
                                result = UpdateRBBankApp(CustomerId, _objAuth, efinid, latestbankApp.BankAppID, latestbankApp, CrossLinkUserId, bankId, EnrollmentId);
                            }

                            if (result.success)
                            {
                                //var banks = _apiObj.getBankApps(_objAuth, efinid);
                                //var bankappId = banks.Where(x => x.BankID == bankCode).Select(x => x.BankAppID).FirstOrDefault();
                                var bankappId = _apiObj.getLatestAppbyBank(_objAuth, efinid, bankCode).BankAppID;

                                SaveEnrollmentHistory("Submitting the application for Customer " + CustomerId + ", bank " + bankCode, CustomerId, 1, bankId, EnrollmentId);

                                string accountId = string.IsNullOrEmpty(latestbankApp.AccountID) ? MasterId : latestbankApp.AccountID;

                                string xml = getsubmitBankAppXml(_objAuth, bankappId, efinid, accountId, bankCode, accountId);

                                //Submitting the bank Application
                                var df = _apiObj.submitBankApplication(_objAuth, bankappId, efinid, accountId, bankCode, accountId);
                                if (df.success)
                                {
                                    UpdateDefaultBankStatus(CustomerId, UserId, bankid, false);
                                    saveBankEnrollment(CustomerId, UserId, bankid);
                                    SaveEnrollmentHistory("Submitted the application", CustomerId, 1, bankId, EnrollmentId, xml);
                                    SaveEnrollmentStatus("Submitted the Application", EnrollmentId, CustomerId);
                                    return new XlinkResponseModel() { IsXlink = true, Messages = df.message.ToList(), Status = df.success };
                                    //Data.InsertData("Update BankEnrollment set StatusCode = '" + SubmitState + "' where CustomerId = '" + CustomerId + "' and IsActive=1");

                                }
                                else
                                {
                                    UpdateDefaultBankStatus(CustomerId, UserId, bankid, true);
                                    SaveInvalidRecord(CustomerId, bankCode);
                                    SaveEnrollmentHistory("Submission failed :: " + string.Join(",", df.message.ToArray()), CustomerId, 0, bankId, EnrollmentId, xml);
                                    var errMsg = df.message.ToList();
                                    errMsg.Insert(0, "Bank Submission failed:");
                                    return new XlinkResponseModel() { IsXlink = true, Messages = errMsg, Status = df.success };
                                }
                            }
                            else
                            {
                                UpdateDefaultBankStatus(CustomerId, UserId, bankid, true);
                                SaveInvalidRecord(CustomerId, bankCode);
                                var errMsg = result.message.ToList();
                                errMsg.Insert(0, "Bank App Update failed:");
                                return new XlinkResponseModel() { IsXlink = true, Messages = errMsg, Status = result.success };
                            }
                        }
                        else
                        {
                            UpdateDefaultBankStatus(CustomerId, UserId, bankid, true);
                            SaveInvalidRecord(CustomerId, bankCode);
                            var errMsg = efinres.Messages.ToList();
                            errMsg.Insert(0, "EFIN Update failed:");
                            return new XlinkResponseModel() { IsXlink = true, Messages = errMsg, Status = efinres.Status };
                        }
                    }
                    else
                    {
                        UpdateDefaultBankStatus(CustomerId, UserId, bankid, true);
                        SaveInvalidRecord(CustomerId, bankCode);
                        SaveEnrollmentHistory("Authentication Failed :: " + string.Join(",", isValid.message.ToArray()), CustomerId, 0, bankId, EnrollmentId);
                        var errMsg = isValid.message.ToList();
                        errMsg.Insert(0, "Authentication Failed:");
                        return new XlinkResponseModel() { IsXlink = true, Messages = errMsg, Status = isValid.success };
                    }
                }
                else
                {
                    UpdateDefaultBankStatus(CustomerId, UserId, bankid, true);
                    SaveInvalidRecord(CustomerId, bankCode);
                    var messages = new List<string>();
                    messages.Add("Authentication Error: Accesskey was not generated.");
                    return new XlinkResponseModel() { IsXlink = false, Messages = messages, Status = false };
                }
            }
            catch (Exception ex)
            {
                UpdateDefaultBankStatus(CustomerId, UserId, bankid, true);
                var messages = new List<string>();
                messages.Add("Submission failed.  Please contact support.");
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SubmitBankApptoXlink", CustomerId);
                return new XlinkResponseModel() { IsXlink = false, Messages = messages, Status = false };
            }
        }

        public XlinkResponseModel UpdateEfinObject(AuthObject auth, int Efin, int EfinId, string AccountId, Guid CustomerId, string ParentId, int UserId, Guid BankId, Guid EnrollmentId, string BankCode)
        {
            try
            {

                EfinObject _objefin = new EfinObject();
                if (BankCode == EMPConstants.TPGBank)
                {
                    #region TPG

                    var tpg = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (tpg != null)
                    {
                        _objefin.AccountID = AccountId;
                        _objefin.AcctName = tpg.AccountName;
                        _objefin.AcctType = tpg.OfficeAccountType;
                        _objefin.Address = tpg.EFINOwnerAddress;
                        _objefin.AgreePEIDate = DateTime.Now;
                        _objefin.AgreePEITerms = true;
                        _objefin.AgreeFeeOption = true;
                        _objefin.BankName = tpg.BankName;
                        _objefin.City = tpg.EFINOwnerCity;
                        _objefin.Company = tpg.CompanyName;
                        _objefin.CreatedBy = AccountId;
                        _objefin.CreatedDate = DateTime.Now;
                        _objefin.DAN = tpg.OfficeDAN;
                        _objefin.DOB = Convert.ToDateTime(tpg.EFINOwnerDOB);
                        _objefin.Efin = Efin;
                        _objefin.EfinID = EfinId;
                        _objefin.EFINType = "S"; //string.IsNullOrEmpty(ParentId) ? "M" : 
                        _objefin.EIN = tpg.EFINOwnerEIN;
                        _objefin.Email = tpg.EFINOwnerEmail;
                        _objefin.FivePlus = false;
                        _objefin.FName = tpg.EFINOwnerFirstName;
                        _objefin.IDNumber = tpg.OwnerIdNumber;
                        _objefin.IDState = tpg.OwnerIdState;
                        _objefin.LName = tpg.EFINOwnerLastName;
                        _objefin.Mobile = string.IsNullOrEmpty(tpg.OwnerMobile) ? "" : tpg.OwnerMobile.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        _objefin.Phone = tpg.EFINOwnerTelephone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        _objefin.RTN = tpg.OfficeRTN;
                        _objefin.SBFeeAll = tpg.FeeOnAll;
                        _objefin.SelectedBank = EMPConstants.TPGBank;
                        _objefin.SSN = tpg.EFINOwnerSSN;
                        _objefin.State = tpg.EFINOwnerState;
                        _objefin.Title = tpg.OwnerTitle;
                        _objefin.UpdatedBy = AccountId;
                        _objefin.UpdatedDate = DateTime.Now;
                        _objefin.UserID = UserId;
                        _objefin.Zip = tpg.EFINOwnerZip;
                    }
                    else
                    {
                        var messages = new List<string>();
                        messages.Add("Invalid Efin");
                        return new XlinkResponseModel() { IsXlink = false, Messages = messages, Status = false };
                    }
                    #endregion
                }
                else if (BankCode == EMPConstants.RABank)
                {
                    #region RA

                    var ra = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (ra != null)
                    {
                        _objefin.AccountID = AccountId;
                        _objefin.AcctName = ra.AccountName;
                        _objefin.AcctType = ra.BankAccountType;
                        _objefin.Address = ra.OwnerAddress;
                        _objefin.AgreePEIDate = DateTime.Now;
                        _objefin.AgreePEITerms = true;
                        _objefin.AgreeFeeOption = true;
                        _objefin.BankName = ra.BankName;
                        _objefin.City = ra.OwnerCity;
                        _objefin.Company = ra.EROOfficeName;
                        _objefin.CreatedBy = AccountId;
                        _objefin.CreatedDate = DateTime.Now;
                        _objefin.DAN = ra.BankAccountNumber;
                        _objefin.DOB = Convert.ToDateTime(ra.OwnerDOB);
                        _objefin.Efin = Efin;
                        _objefin.EfinID = EfinId;
                        _objefin.EFINType = "S";
                        _objefin.EIN = ra.BusinessEIN;
                        _objefin.Email = ra.OwnerEmail;
                        _objefin.FivePlus = false;
                        _objefin.FName = ra.OwnerFirstName;
                        _objefin.IDNumber = ra.OwnerStateIssuedIdNumber;
                        _objefin.IDState = ra.OwnerIssuingState;
                        _objefin.LName = ra.OwnerLastName;
                        _objefin.Mobile = !string.IsNullOrEmpty(ra.OwnerCellPhone) ? ra.OwnerCellPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "") : "";
                        _objefin.Phone = ra.OwnerHomePhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        _objefin.RTN = ra.BankRoutingNumber;
                        _objefin.SBFeeAll = ra.SbFeeall;
                        _objefin.SelectedBank = EMPConstants.RABank;
                        _objefin.SSN = ra.OwnerSSN;
                        _objefin.State = ra.OwnerState;
                        _objefin.Title = ra.OwnerTitle;
                        _objefin.UpdatedBy = AccountId;
                        _objefin.UpdatedDate = DateTime.Now;
                        _objefin.UserID = UserId;
                        _objefin.Zip = ra.OwnerZipCode;
                    }
                    else
                    {
                        var messages = new List<string>();
                        messages.Add("Invalid Efin");
                        return new XlinkResponseModel() { IsXlink = false, Messages = messages, Status = false };
                    }
                    #endregion
                }
                else if (BankCode == EMPConstants.RBBank)
                {
                    #region RB

                    var rb = db.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (rb != null)
                    {
                        _objefin.AccountID = AccountId;
                        _objefin.AcctName = rb.CheckingAccountName;
                        _objefin.AcctType = rb.BankAccountType;
                        _objefin.Address = rb.EFINOwnerAddress;
                        _objefin.AgreePEIDate = DateTime.Now;
                        _objefin.AgreePEITerms = true;
                        _objefin.AgreeFeeOption = true;
                        _objefin.BankName = rb.BankName;
                        _objefin.City = rb.EFINOwnerCity;
                        _objefin.Company = rb.OfficeName;
                        _objefin.CreatedBy = AccountId;
                        _objefin.CreatedDate = DateTime.Now;
                        _objefin.DAN = rb.BankAccountNumber;
                        _objefin.DOB = Convert.ToDateTime(rb.EFINOwnerDOB);
                        _objefin.Efin = Efin;
                        _objefin.EfinID = EfinId;
                        _objefin.EFINType = "S";
                        _objefin.EIN = rb.EFINOwnerEIN;
                        _objefin.Email = rb.EFINOwnerEmail;
                        _objefin.FivePlus = false;
                        _objefin.FName = rb.EFINOwnerFirstName;
                        _objefin.IDNumber = rb.EFINOwnerIDNumber;
                        _objefin.IDState = rb.EFINOwnerIDState;
                        _objefin.LName = rb.EFINOwnerLastName;
                        _objefin.Mobile = string.IsNullOrEmpty(rb.EFINOwnerMobile) ? "" : rb.EFINOwnerMobile.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        _objefin.Phone = string.IsNullOrEmpty(rb.EFINOwnerPhone) ? "" : rb.EFINOwnerPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                        _objefin.RTN = rb.BankRoutingNumber;
                        _objefin.SBFeeAll = rb.SBFeeonAll;
                        _objefin.SelectedBank = "R";
                        _objefin.SSN = rb.EFINOwnerSSN;
                        _objefin.State = rb.EFINOwnerState;
                        _objefin.Title = rb.EFINOwnerTitle;
                        _objefin.UpdatedBy = AccountId;
                        _objefin.UpdatedDate = DateTime.Now;
                        _objefin.UserID = UserId;
                        _objefin.Zip = rb.EFINOwnerZip;
                    }
                    else
                    {
                        var messages = new List<string>();
                        messages.Add("Invalid EFIN");
                        return new XlinkResponseModel() { IsXlink = false, Messages = messages, Status = false };
                    }
                    #endregion
                }

                string xml = getEfinObjecyXml(_objefin);
                XlinkResponse isValid = _apiObj.validateEfinObject(_objefin, false);
                if (!isValid.success)
                {
                    if (isValid.message != null)
                    {
                        SaveEnrollmentHistory("EFIN Update failed :: " + string.Join(",", isValid.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml);
                    }
                    return new XlinkResponseModel() { IsXlink = true, Messages = isValid.message.ToList(), Status = isValid.success };
                }
                XlinkResponse res = _apiObj.updateEFIN(auth, _objefin, AccountId, 0, false);
                if (!res.success)
                {
                    if (res.message != null)
                    {
                        if (res.message.Count() > 0)
                            SaveEnrollmentHistory("EFIN Update failed :: " + string.Join(",", res.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml);
                        else
                            SaveEnrollmentHistory("EFIN Update failed", CustomerId, 0, BankId, EnrollmentId, xml);
                    }
                    return new XlinkResponseModel() { IsXlink = true, Messages = res.message.ToList(), Status = res.success };
                }
                else
                    SaveEnrollmentHistory("EFIN Updated ", CustomerId, 1, BankId, EnrollmentId, xml);
                return new XlinkResponseModel() { IsXlink = true, Messages = res.message.ToList(), Status = res.success };
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/UpdateEfinObject", CustomerId);
                return new XlinkResponseModel() { IsXlink = false, Messages = new List<string>(), Status = false };
            }
        }

        public XlinkResponse UpdateTPGBankApp(Guid CustomerId, AuthObject auth, int EfinId, int AppId, AppObject latestbankApp, int UserId, Guid BankId, Guid EnrollmentId)
        {
            try
            {
                var tpg = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (tpg != null)
                {
                    DateTime? dt = null;

                    SBTPGAppObject appObj = new SBTPGAppObject();
                    appObj.Delivered = latestbankApp.Delivered.HasValue ? latestbankApp.Delivered.Value : false;
                    appObj.DeliveredDate = latestbankApp.DeliveredDate.HasValue ? latestbankApp.DeliveredDate.Value : dt;
                    appObj.CompanyName = tpg.CompanyName;
                    appObj.Deleted = false;
                    appObj.EfinID = EfinId;
                    appObj.RalBankLY = tpg.BankUsedLastYear;
                    appObj.ManagerEmail = tpg.ManagerEmail;
                    appObj.ManagerFName = tpg.ManagerFirstName;
                    appObj.ManagerLName = tpg.ManagerLastName;
                    appObj.OfficeAddr = tpg.OfficeAddress;
                    appObj.OfficeCity = tpg.OfficeCity;
                    appObj.OfficeFax = tpg.OfficeFAX;
                    appObj.OfficePhone = tpg.OfficeTelephone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.OfficeState = tpg.OfficeState;
                    appObj.OfficeZip = tpg.OfficeZip;
                    appObj.OwnerAddr = tpg.EFINOwnerAddress;
                    appObj.OwnerCity = tpg.EFINOwnerCity;
                    if (tpg.EFINOwnerDOB.HasValue)
                        appObj.OwnerDOB = Convert.ToDateTime(tpg.EFINOwnerDOB.Value);
                    appObj.OwnerEIN = tpg.EFINOwnerEIN;
                    appObj.OwnerEmail = tpg.EFINOwnerEmail;
                    appObj.OwnerFName = tpg.EFINOwnerFirstName;
                    appObj.OwnerLName = tpg.EFINOwnerLastName;
                    appObj.OwnerPhone = tpg.EFINOwnerTelephone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.OwnerSSN = tpg.EFINOwnerSSN;
                    appObj.OwnerState = tpg.EFINOwnerState;
                    appObj.OwnerZip = tpg.EFINOwnerZip;
                    appObj.SBTPGBankAppID = AppId;
                    appObj.ShipAddress = tpg.ShippingAddress;
                    appObj.ShipCity = tpg.ShippingCity;
                    appObj.ShipState = tpg.ShippingState;
                    appObj.ShipZip = tpg.ShippingZip;
                    appObj.UpdatedBy = auth.UserID;
                    appObj.UpdatedDate = DateTime.Now;
                    appObj.UserID = UserId;
                    appObj.Sent = latestbankApp.Sent.HasValue ? latestbankApp.Sent.Value : false;
                    appObj.SentDate = latestbankApp.SentDate.HasValue ? latestbankApp.SentDate.Value : dt;
                    appObj.EROTranFee = Convert.ToDecimal(tpg.AddonFee);
                    appObj.SBPrepFee = Convert.ToDecimal(tpg.ServiceBureauFee);
                    appObj.DocPrepFee = Convert.ToDecimal(tpg.DocPrepFee);
                    appObj.SuperSBid = 0;
                    appObj.CheckPrint = "D";
                    appObj.Hidden = false;
                    appObj.AgreeBank = Convert.ToBoolean(tpg.AgreeBank);
                    appObj.AgreeDate = DateTime.Now;
                    if (!string.IsNullOrEmpty(tpg.PriorYearEFIN))
                        appObj.EFINLY = Convert.ToInt32(tpg.PriorYearEFIN);
                    if (!string.IsNullOrEmpty(tpg.PriorYearVolume))
                        appObj.VolumeLY = Convert.ToInt32(tpg.PriorYearVolume);
                    else
                        appObj.VolumeLY = 0;
                    if (!string.IsNullOrEmpty(tpg.PriorYearFund))
                        appObj.BankProductsLY = Convert.ToInt32(tpg.PriorYearFund);
                    else
                        appObj.BankProductsLY = 0;

                    var addonMasterfees = (from f in db.FeeMasters
                                           where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                           select f.Amount).FirstOrDefault();
                    decimal xlinkaddon = addonMasterfees.Value;
                    appObj.EROTranFee = appObj.EROTranFee - xlinkaddon;

                    #region changes as per demo

                    appObj.AntiVirus = true;
                    appObj.Firewall = true;
                    appObj.LockedWhenVacant = true;
                    appObj.SecureDocumentStorage = true;
                    appObj.SecureLogin = true;
                    appObj.SecureStorage = true;

                    #endregion


                    string xml = getSBTPGXML(appObj);
                    var IsValid = _apiObj.validateSBTPGApp(appObj, false);
                    if (!IsValid.success)
                    {
                        if (IsValid.message != null)
                        {
                            SaveEnrollmentHistory("Updating App Failed :: " + string.Join(",", IsValid.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml);
                        }
                        return IsValid;
                    }
                    XlinkResponse response = _apiObj.updateSBTPGApp(auth, appObj, false);
                    if (response.success)
                    {
                        SaveEnrollmentHistory("Updated", CustomerId, 1, BankId, EnrollmentId, xml);
                        SaveEnrollmentStatus("App Updated", EnrollmentId, CustomerId);
                    }
                    else { SaveEnrollmentHistory(string.Join(",", response.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml); }

                    return response;
                }
                else
                {
                    SaveEnrollmentHistory("The record is not availble in TPG.", CustomerId, 0, BankId, EnrollmentId);
                    return new XlinkResponse() { success = false };
                }
            }
            catch (Exception ex)
            {
                SaveEnrollmentHistory(ex.Message + ":: TPG.", CustomerId, 0, BankId, EnrollmentId);
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/UpdateTPGBankApp", CustomerId);
                return new XlinkResponse() { success = false };
            }
        }

        public XlinkResponse UpdateRABankApp(Guid CustomerId, AuthObject auth, int EfinId, int AppId, AppObject latestbankApp, int UserId, Guid BankId, Guid EnrollmentId)
        {
            try
            {
                var ra = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (ra != null)
                {
                    DateTime? dt = null;
                    RefundAdvantageAppObject appObj = new RefundAdvantageAppObject();
                    appObj.Deleted = false;
                    appObj.AgreeBank = true;
                    appObj.AgreeDate = DateTime.Now;
                    appObj.EfinID = EfinId;
                    appObj.CorporationType = ra.CorporationType;
                    if (ra.ExpectedCurrentYearVolume.HasValue)
                        appObj.CurrentYearVolume = Convert.ToInt32(ra.ExpectedCurrentYearVolume.Value);
                    appObj.RefundAdvantageBankAppID = AppId;
                    appObj.EFINAddressCity = ra.OwnerCity;
                    appObj.EFINAddressState = ra.OwnerState;
                    appObj.EFINAddressStreet = ra.OwnerAddress;
                    appObj.EFINAddressZip = ra.OwnerZipCode;
                    appObj.EFINOwnerHomePhone = ra.OwnerHomePhone;
                    appObj.EFINOwnerIDNumber = ra.OwnerStateIssuedIdNumber;
                    appObj.EFINOwnerIDState = ra.OwnerIssuingState;
                    appObj.MailingAddressCity = ra.EROMailingCity;
                    appObj.MailingAddressState = ra.EROMailingState;
                    appObj.MailingAddressStreet = ra.EROMaillingAddress;
                    appObj.MailingAddressZip = ra.EROMailingZipcode;
                    appObj.OfficeAddressCity = ra.EROOfficeAddress;
                    appObj.OfficeAddressState = ra.EROOfficeState;
                    appObj.OfficeAddressStreet = ra.EROOfficeAddress;
                    appObj.OfficeAddressZip = ra.EROOfficeZipCoce;
                    appObj.OfficeName = ra.EROOfficeName;
                    appObj.OfficePhone = ra.EROOfficePhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.PriorYearBank = ra.PreviousBankName;
                    if (ra.PreviousYearVolume.HasValue)
                        appObj.PriorYearVolume = Convert.ToInt32(ra.PreviousYearVolume.Value);
                    appObj.ShippingAddressCity = ra.EROShippingCity;
                    appObj.ShippingAddressState = ra.EROShippingState;
                    appObj.ShippingAddressStreet = ra.EROShippingAddress;
                    appObj.ShippingAddressZip = ra.EROShippingZip;
                    appObj.UpdatedBy = auth.UserID;
                    appObj.UpdatedDate = DateTime.Now;
                    appObj.UserID = UserId;
                    appObj.Sent = latestbankApp.Sent.HasValue ? latestbankApp.Sent.Value : false;
                    appObj.SentDate = latestbankApp.SentDate.HasValue ? latestbankApp.SentDate.Value : dt;
                    appObj.SystemHold = latestbankApp.IsSystemHold.HasValue ? latestbankApp.IsSystemHold.Value : false;
                    appObj.ContactPhone = ra.MainContactPhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.ContactFirstName = ra.MainContactFirstName;
                    appObj.ContactLastName = ra.MainContactLastName;
                    appObj.EFilingFee = Convert.ToDecimal(ra.ElectronicFee);
                    appObj.EROTranFee = Convert.ToDecimal(ra.TransmissionAddon);
                    appObj.ExperienceFiling = Convert.ToInt32(ra.NoofYearsExperience);
                    appObj.LegalIssues = Convert.ToBoolean(ra.LegalIssues);
                    if (!string.IsNullOrEmpty(ra.SbFee))
                        appObj.SBFee = Convert.ToDecimal(ra.SbFee);
                    appObj.Sent = true;
                    appObj.StateofIncorporation = ra.StateOfIncorporation;
                    appObj.TextMessage = Convert.ToBoolean(ra.TextMessages);

                    var addonMasterfees = (from f in db.FeeMasters
                                           where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                           select f.Amount).FirstOrDefault();
                    decimal xlinkaddon = addonMasterfees.Value;
                    appObj.EROTranFee = appObj.EROTranFee - xlinkaddon;

                    string xml = getRefundAdvantageXML(appObj);
                    XlinkResponse response = _apiObj.updateRefundAdvantageApp(auth, appObj, false);
                    if (response.success)
                    {
                        SaveEnrollmentHistory("Updated the RA application for Customer " + CustomerId, CustomerId, 1, BankId, EnrollmentId, xml);
                        SaveEnrollmentStatus("App Updated", EnrollmentId, CustomerId);
                    }
                    else { SaveEnrollmentHistory(string.Join(",", response.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml); }

                    return response;
                }
                else
                {
                    SaveEnrollmentHistory("The record is not availble in RA", CustomerId, 0, BankId, EnrollmentId);
                    return new XlinkResponse() { success = false };
                }
            }
            catch (Exception ex)
            {
                SaveEnrollmentHistory(ex.Message + ":: RA", CustomerId, 0, BankId, EnrollmentId);
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/UpdateRABankApp", CustomerId);
                return new XlinkResponse() { success = false };
            }
        }

        public XlinkResponse UpdateRBBankApp(Guid CustomerId, AuthObject auth, int EfinId, int AppId, AppObject latestbankApp, int UserId, Guid BankId, Guid EnrollmentId)
        {
            try
            {
                var rb = db.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (rb != null)
                {
                    DateTime? dt = null;

                    RepublicAppObject appObj = new RepublicAppObject();
                    appObj.ActualNumberBankProducts = Convert.ToInt32(rb.ActualNoofBankProductsLastYear);
                    appObj.AdvertisingInd = rb.AdvertisingApproval;
                    appObj.AntiVirusInd = rb.AntivirusRequired;
                    appObj.BankProductFacilitator = rb.PreviousBankProductFacilitator;
                    appObj.CheckCardStorageInd = rb.IsLockedStore_Checks;
                    appObj.AgreeBank = true;
                    appObj.AgreeDate = DateTime.Now;
                    appObj.Delivered = latestbankApp.Delivered.HasValue ? latestbankApp.Delivered.Value : false;
                    appObj.DeliveredDate = latestbankApp.DeliveredDate.HasValue ? latestbankApp.DeliveredDate.Value : dt;
                    appObj.CellPhoneNumber = rb.CellPhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.ComplianceWithLawInd = rb.IsAsPerComplainceLaw;
                    appObj.Deleted = false;
                    appObj.DebitProgramInd = rb.ProductsOffering.Value.ToString();
                    appObj.DocumentAccessInd = rb.IsLimitAccess;
                    appObj.DocumentStorageInd = rb.IsLockedStore_Documents;
                    appObj.EfinID = EfinId;
                    if (rb.EFINOwnerDOB.HasValue)
                        appObj.EFINOwnerDOB = Convert.ToDateTime(rb.EFINOwnerDOB.Value);
                    appObj.EfinOwnerFirstName = rb.EFINOwnerFirstName;
                    appObj.EfinOwnerLastName = rb.EFINOwnerLastName;
                    appObj.EfinOwnerSSN = rb.EFINOwnerSSN;
                    appObj.EIN = rb.BusinessEIN;
                    appObj.EmailAddress = rb.EmailAddress;
                    appObj.FaxNumber = rb.FAXNumber;
                    appObj.FirewallInd = rb.HasFirewall;
                    appObj.FulfillmentShippingCity = rb.FulfillmentShippingCity;
                    appObj.FulfillmentShippingState = rb.FulfillmentShippingState;
                    appObj.FulfillmentShippingStreet = rb.FulfillmentShippingAddress;
                    appObj.FulfillmentShippingZip = rb.FulfillmentShippingZip;
                    appObj.IRSTransmittingOfficeInd = rb.IsOfficeTransmit;
                    if (rb.NoofBankProductsLastYear.HasValue)
                        appObj.LastYearBankProducts = Convert.ToInt32(rb.NoofBankProductsLastYear);
                    appObj.LegalEntityStatusInd = rb.LegarEntityStatus;
                    appObj.LLCMembershipRegistration = rb.LLCMembershipRegistration;
                    appObj.LoginPassInd = rb.LoginAccesstoEmployees;
                    appObj.MailingAddress = rb.MailingAddress;
                    appObj.MailingCity = rb.MailingCity;
                    appObj.MailingState = rb.MailingState;
                    appObj.MailingZip = rb.MailingZip;
                    appObj.MasterID = latestbankApp.Master.HasValue ? latestbankApp.Master.Value : 0;
                    appObj.MultiOffice = rb.IsMultiOffice;
                    if (rb.NoofPersoneel.HasValue)
                        appObj.NumOfPersonnel = Convert.ToInt32(rb.NoofPersoneel);
                    appObj.OfficeContactFirstName = rb.OfficeContactFirstName;
                    appObj.OfficeContactLastName = rb.OfficeContactLastName;
                    appObj.OfficeContactSSN = rb.OfficeContactSSN;
                    appObj.OfficeDoorInd = rb.IsLocked_Office;
                    if (rb.OfficeManagerDOB.HasValue)
                        appObj.OfficeManagerDOB = Convert.ToDateTime(rb.OfficeManagerDOB);
                    appObj.OfficeManagerFirstName = rb.OfficeManagerFirstName;
                    appObj.OfficeManagerLastName = rb.OfficeManageLastName;
                    appObj.OfficeManagerSSN = rb.OfficeManagerSSN;
                    appObj.OfficeName = rb.OfficeName;
                    appObj.OfficePhoneNumber = rb.OfficePhoneNumber.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    appObj.OfficePhysicalCity = rb.OfficePhysicalCity;
                    appObj.OfficePhysicalState = rb.OfficePhysicalState;
                    appObj.OfficePhysicalStreet = rb.OfficePhysicalAddress;
                    appObj.OfficePhysicalZip = rb.OfficePhysicalZip;
                    appObj.OwnerAddress = rb.OwnerAddress;
                    appObj.OwnerCity = rb.OwnerCity;
                    if (rb.OnwerDOB.HasValue)
                        appObj.OwnerDOB = Convert.ToDateTime(rb.OnwerDOB);
                    appObj.OwnerFirstName = rb.OwnerFirstName;
                    appObj.OwnerHomePhone = rb.OwnerHomePhone;
                    appObj.OwnerLastName = rb.OwnerLastName;
                    appObj.OwnerSSN = rb.OwnerSSN;
                    appObj.OwnerState = rb.OwnerState;
                    appObj.OwnerZip = rb.OwnerZip;
                    appObj.PreviousViolationFineInd = rb.ConsumerLending;
                    appObj.ProductTrainingInd = rb.OnlineTraining;
                    appObj.PTINInd = rb.IsPTIN;
                    appObj.RepublicBankAppID = AppId;
                    appObj.SensitiveDocumentDestInd = rb.PlantoDispose;
                    appObj.UpdatedBy = auth.UserID;
                    appObj.UpdatedDate = DateTime.Now;
                    appObj.UserID = UserId;
                    appObj.Sent = latestbankApp.Sent.HasValue ? latestbankApp.Sent.Value : false;
                    appObj.SentDate = latestbankApp.SentDate.HasValue ? latestbankApp.SentDate.Value : dt;
                    appObj.SystemHold = latestbankApp.IsSystemHold.HasValue ? latestbankApp.IsSystemHold.Value : false;
                    appObj.WebsiteAddress = rb.WebsiteAddress;
                    appObj.WirelessInd = rb.PasswordRequired;
                    appObj.YearsInBusiness = Convert.ToInt32(rb.YearsinBusiness);
                    appObj.LastYearBankProducts = Convert.ToInt32(rb.NoofBankProductsLastYear);
                    appObj.TaxPrepLicensing = rb.IsAsPerProcessLaw;
                    appObj.SupportedOsInd = rb.SupportOS;
                    appObj.SBPrepFee = Convert.ToDecimal(rb.SBFee);
                    appObj.EROTranFee = Convert.ToDecimal(rb.TransimissionAddon);
                    appObj.CardProgram = rb.PrepaidCardProgram;

                    var addonMasterfees = (from f in db.FeeMasters
                                           where f.FeesFor == 3 && f.StatusCode == EMPConstants.Active
                                           select f.Amount).FirstOrDefault();
                    decimal xlinkaddon = addonMasterfees.Value;
                    appObj.EROTranFee = appObj.EROTranFee - xlinkaddon;

                    string xml = getRepublicXML(appObj);
                    XlinkResponse response = _apiObj.updateRepublicApp(auth, appObj, false);
                    if (response.success)
                    {
                        SaveEnrollmentHistory("Updated", CustomerId, 1, BankId, EnrollmentId, xml);
                        SaveEnrollmentStatus("App Updated", EnrollmentId, CustomerId);
                    }
                    else { SaveEnrollmentHistory("Update failed :: " + string.Join(",", response.message.ToArray()), CustomerId, 0, BankId, EnrollmentId, xml); }

                    return response;
                }
                else
                {
                    SaveEnrollmentHistory("The record is not availble in RB.", CustomerId, 0, BankId, EnrollmentId);
                    return new XlinkResponse() { success = false };
                }
            }
            catch (Exception ex)
            {
                SaveEnrollmentHistory(ex.Message + ":: RB. CustId : " + CustomerId, CustomerId, 0, BankId, EnrollmentId);
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/UpdateRBBankApp", CustomerId);
                return new XlinkResponse() { success = false };
            }
        }

        public XlinkResponse UpdateRAOwnerObject(Guid CustomerId, int AppId, AuthObject auth, int UserId, Guid EnrollmentId)
        {
            var owners = _apiObj.getRefundAdvantageOwners(auth, AppId);
            foreach (var owner in owners)
            {
                XlinkResponse delres = _apiObj.deleteOwner(auth, owner);
            }

            var raid = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
            if (raid != null)
            {
                var raowners = db.BankEnrollmentEFINOwnersForRAs.Where(x => x.BankEnrollmentRAId == raid.Id).ToList();
                foreach (var item in raowners)
                {
                    RefundAdvantageOwnerObject _onwerObj = new RefundAdvantageOwnerObject();
                    _onwerObj.Address = item.Address;
                    _onwerObj.City = item.City;
                    _onwerObj.DOB = DateTime.ParseExact(item.DateofBirth, "MM'/'dd'/'yyyy", CultureInfo.InvariantCulture);
                    _onwerObj.FirstName = item.FirstName;
                    _onwerObj.IdNumber = item.IDNumber;
                    _onwerObj.IdState = item.IDState;
                    _onwerObj.LastName = item.LastName;
                    _onwerObj.PercentOwned = Convert.ToInt32(item.PercentageOwned);
                    _onwerObj.Phone = item.HomePhone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    _onwerObj.Refund_Advantage_BankAppID = AppId;
                    _onwerObj.SSN = item.SSN;
                    _onwerObj.State = item.StateId;
                    _onwerObj.Zip = item.ZipCode;

                    string xml = getRAOwnerObjecyXml(_onwerObj);

                    XlinkResponse res = _apiObj.updateRefundAdvantageOwner(auth, _onwerObj, false);
                    if (!res.success)
                    {
                        SaveEnrollmentHistory("RAOwner Update failed :: " + string.Join(",", res.message.ToArray()), CustomerId, 0, Guid.Empty, EnrollmentId, xml);
                        return res;
                    }
                    else { SaveEnrollmentHistory("RAOwner Updated for CustId :: " + CustomerId + " " + string.Join(",", res.message.ToArray()), CustomerId, 1, Guid.Empty, EnrollmentId, xml); }
                }
                return new XlinkResponse() { success = true };
            }
            else
                return new XlinkResponse() { success = false };
        }

        public XlinkResponseModel checkandUpdateEFIN(int efin, string MasterId, string Password, string ParentId, Guid CustomerId, int UserId, Guid BankId, Guid EnrollmentId, string BankCode)
        {
            XlinkResponseModel res = new DTO.XlinkResponseModel();

            string _accesskey = _apiObj.getAccessKey(MasterId, Password);
            if (_accesskey != "")
            {
                AuthObject _objAuth = new AuthObject();
                _objAuth.AccessKey = _accesskey;
                _objAuth.UserID = MasterId;

                XlinkResponse isValid = _apiObj.isAuth(_objAuth);
                if (isValid.success)
                {
                    int efinid = _apiObj.getEFINID(efin);
                    var efinobject = _apiObj.getEFINbyEFIN(_objAuth, efin, MasterId, 0);
                    if (efinobject.EfinID == 0 && efinid != 0)
                    {
                        List<string> messages = new List<string>();
                        messages.Add("EFIN already exists.");
                        res.Messages = messages;
                        res.Status = false;
                        return res;
                    }
                    else
                    {
                        res.Status = true;
                        return res;
                    }
                    //int efinid = _apiObj.getEFINID(efin);
                    //var objefin = _apiObj.getEFIN(_objAuth, efinid, MasterId, 0);
                    //if (!objefin.Response.success)
                    //{

                    //}
                }
                else
                {
                    if (!string.IsNullOrEmpty(ParentId))
                    {
                        Guid Parent = new Guid(ParentId);
                        var parent = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == Parent).FirstOrDefault();
                        if (parent != null)
                        {
                            var parentinfo = db.emp_CustomerInformation.Where(x => x.Id == Parent).FirstOrDefault();

                            int CrossLinkUserId = 0;
                            getxlinkCredentials(ref MasterId, ref Password, ref CrossLinkUserId, parent.CrossLinkUserId, parent.CLAccountId, parent.CLAccountPassword, parent.CLLogin, parent.MasterIdentifier, parentinfo.ParentId ?? Guid.Empty, parentinfo.EntityId ?? 0);

                            string _parentaccesskey = _apiObj.getAccessKey(MasterId, Password);
                            if (_parentaccesskey != "")
                            {
                                AuthObject _parentobjAuth = new AuthObject();
                                _parentobjAuth.AccessKey = _parentaccesskey;
                                _parentobjAuth.UserID = MasterId;

                                XlinkResponse pisValid = _apiObj.isAuth(_parentobjAuth);
                                if (pisValid.success)
                                {
                                    int efinid = _apiObj.getEFINID(efin);
                                    var efinobject = _apiObj.getEFINbyEFIN(_objAuth, efin, MasterId, 0);
                                    if (efinobject.EfinID == 0 && efinid != 0)
                                    {
                                        List<string> messages = new List<string>();
                                        messages.Add("EFIN already exists, please contact support.");
                                        res.Messages = messages;
                                        res.Status = false;
                                        return res;
                                    }
                                    else if (efinobject.EfinID == 0)
                                    {
                                        //creating new efin object under
                                        UpdateEfinObject(_parentobjAuth, efin, efinobject.EfinID, MasterId, CustomerId, ParentId, UserId, BankId, EnrollmentId, BankCode);
                                        res.Status = true;
                                        return res;
                                    }
                                }
                                else
                                {
                                    List<string> messages = new List<string>();
                                    messages.Add("Authentication Failed.");
                                    res.Messages = messages;
                                    res.Status = false;
                                    return res;
                                }
                            }
                            else
                            {
                                List<string> messages = new List<string>();
                                messages.Add("Accesskey was not generated.");
                                res.Messages = messages;
                                res.Status = false;
                                return res;
                            }
                        }
                        else
                        {
                            List<string> messages = new List<string>();
                            messages.Add("Invalid User");
                            res.Messages = messages;
                            res.Status = false;
                            return res;
                        }
                    }
                    else
                    {
                        List<string> messages = new List<string>();
                        messages.Add("Invalid User");
                        res.Messages = messages;
                        res.Status = false;
                        return res;
                    }
                }
            }
            else
            {
                List<string> messages = new List<string>();
                messages.Add("Invalid User");
                res.Messages = messages;
                res.Status = false;
                return res;
            }
            return res;
        }

        public bool saveBankEnrollment(Guid CustomerId, Guid UserId, Guid BankId)
        {
            try
            {
                var bank = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == BankId).Select(x => x.BankId).FirstOrDefault();
                var customer = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).Select(x => x).FirstOrDefault();
                var isexist = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == BankId).FirstOrDefault();
                if (isexist != null)
                {
                    isexist.StatusCode = EMPConstants.Submitted;
                    isexist.UpdatedBy = UserId;
                    isexist.UpdatedDate = DateTime.Now;
                    //isexist.IsActive = false;
                    db.SaveChanges();
                }

                return true;

            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/saveBankEnrollment", UserId);
                return false;
            }
        }

        public void UpdateDefaultBankStatus(Guid CustomerId, Guid UserId, Guid BankId, bool Status)
        {
            try
            {
                var upbank = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == BankId).FirstOrDefault();
                if (upbank.BankSubmissionStatus == 1)
                {
                    bool updated = false;
                    var banks = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId != BankId).ToList();
                    foreach (var bnk in banks)
                    {
                        var isexist = db.BankEnrollments.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == bnk.BankId).FirstOrDefault();
                        if (isexist != null)
                        {
                            if (isexist.StatusCode == EMPConstants.Approved)
                            {
                                updated = true;
                                bnk.BankSubmissionStatus = 1;
                                bnk.LastUpdatedBy = UserId;
                                bnk.LastUpdatedDate = DateTime.Now;
                            }
                            db.SaveChanges();
                        }
                    }
                    if (updated || Status)
                    {
                        var prebanks = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).ToList();
                        foreach (var item in prebanks)
                        {
                            if (item.BankId != BankId)
                                item.IsPreferredBank = false;
                            else
                            {
                                item.BankSubmissionStatus = 0;
                                item.IsPreferredBank = true;
                            }
                        }
                        db.SaveChanges();
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public void SaveInvalidRecord(Guid CustomerId, string BankCode)
        {
            try
            {
                DateTime? updatedDate = new DateTime();
                if (BankCode == EMPConstants.TPGBank)
                {
                    updatedDate = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.UpdatedDate).FirstOrDefault();
                }
                else if (BankCode == EMPConstants.RABank)
                {
                    updatedDate = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.UpdatedDate).FirstOrDefault();
                }
                else if (BankCode == EMPConstants.RBBank)
                {
                    updatedDate = db.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.UpdatedDate).FirstOrDefault();
                }

                BankEnrollmentInvalid invalid = new BankEnrollmentInvalid();
                invalid.CreatedBy = CustomerId;
                invalid.CreatedDate = DateTime.Now;
                invalid.CustomerId = CustomerId;
                invalid.IsValid = false;
                invalid.LastUpdatedDate = updatedDate;
                db.BankEnrollmentInvalids.Add(invalid);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveInvalidRecord", CustomerId);
            }
        }

        public bool IsValidRecord(Guid CustomerId)
        {
            try
            {
                var bank = (from s in db.BankEnrollments
                            join b in db.BankMasters on s.BankId equals b.Id
                            where s.CustomerId == CustomerId && s.IsActive == true
                            select b.BankCode).FirstOrDefault();
                if (bank == EMPConstants.TPGBank)
                {
                    var updatedDate = db.BankEnrollmentForTPGs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.UpdatedDate).FirstOrDefault();
                    var isexist = db.BankEnrollmentInvalids.Where(x => x.CustomerId == CustomerId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (isexist != null)
                    {
                        if (isexist.LastUpdatedDate == updatedDate)
                            return false;
                        else
                            return true;
                    }
                    else
                        return true;
                }
                else if (bank == EMPConstants.RABank)
                {
                    var updatedDate = db.BankEnrollmentForRAs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.UpdatedDate).FirstOrDefault();
                    var isexist = db.BankEnrollmentInvalids.Where(x => x.CustomerId == CustomerId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (isexist != null)
                    {
                        if (isexist.LastUpdatedDate == updatedDate)
                            return false;
                        else
                            return true;
                    }
                    else
                        return true;
                }
                if (bank == EMPConstants.RBBank)
                {
                    var updatedDate = db.BankEnrollmentForRBs.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active).Select(x => x.UpdatedDate).FirstOrDefault();
                    var isexist = db.BankEnrollmentInvalids.Where(x => x.CustomerId == CustomerId).OrderByDescending(x => x.CreatedDate).FirstOrDefault();
                    if (isexist != null)
                    {
                        if (isexist.LastUpdatedDate == updatedDate)
                            return false;
                        else
                            return true;
                    }
                    else
                        return true;
                }
                else
                    return true;

            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/IsValidRecord", CustomerId);
                return true;
            }
        }

        public void SaveEnrollmentHistory(string Message, Guid CustomerId, int Status, Guid BankId, Guid EnrollmentId, string xml = "")
        {
            try
            {
                BankEnrollmentHistory history = new BankEnrollmentHistory();
                history.BankId = BankId;
                history.CreatedBy = CustomerId;
                history.EnrollmentId = EnrollmentId;
                history.Message = Message;
                history.Paramaeters = xml;
                history.Status = Status == 1 ? true : false;
                history.StatusCode = "";
                history.UpdatedBy = CustomerId;
                history.UpdatedDate = DateTime.Now;
                history.CustomerId = CustomerId;

                db.BankEnrollmentHistories.Add(history);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveEnrollmentHistory", CustomerId);
            }
        }

        public void SaveEnrollmentStatus(string Status, Guid EnrollmentId, Guid CustomerId)
        {
            try
            {
                BankEnrollmentStatu status = new BankEnrollmentStatu();
                status.CreatedDate = DateTime.Now;
                status.EnrollmentId = EnrollmentId;
                status.Id = Guid.NewGuid();
                status.IsUnlocked = false;
                status.Reason = "";
                status.Status = Status;
                status.UpdatedBy = CustomerId;
                status.UpdatedDate = DateTime.Now;
                db.BankEnrollmentStatus.Add(status);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/SaveEnrollmentStatus", CustomerId);
            }
        }

        public bool getAddonSelection(Guid CustomerId, Guid bankid)
        {
            try
            {
                var selection = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.BankId == bankid && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                if (selection != null)
                {
                    if ((selection.IsServiceBureauFee ?? false && selection.ServiceBureauBankAmount > 0) || (selection.IsTransmissionFee ?? false && selection.TransmissionBankAmount > 0))
                        return true;
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getAddonSelection", CustomerId);
                return true;
            }
        }

        public UpdateEnrollmentAddonRes UpdateAddonforEnrollment(UpdateEnrollmentAddon req)
        {
            UpdateEnrollmentAddonRes res = new UpdateEnrollmentAddonRes();
            try
            {
                var isexist = db.EnrollmentAddonStagings.Where(x => x.CustomerId == req.CustomerId && x.IsActive == true && x.BankId == req.BankId).FirstOrDefault();
                if (isexist != null)
                {
                    isexist.IsActive = false;
                    isexist.UpdatedBy = req.UserId;
                    isexist.UpdatedDate = DateTime.Now;
                    db.SaveChanges();
                }

                var enroll = db.EnrollmentBankSelections.Where(x => x.CustomerId == req.CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == req.BankId).FirstOrDefault();

                EnrollmentAddonStaging staging = new EnrollmentAddonStaging();
                staging.BankSelectionId = enroll == null ? Guid.Empty : enroll.Id;
                staging.BankId = req.BankId;
                staging.CreatedBy = req.UserId;
                staging.CreatedDate = DateTime.Now;
                staging.CustomerId = req.CustomerId;
                staging.Id = Guid.NewGuid();
                staging.IsActive = true;
                staging.IsSvbFee = req.IsSvbFee;
                staging.IsTransmissionFee = req.IsTransmissionFee;
                staging.SvbAddonAmount = req.SvbAmount;
                staging.TransmissionAddonAmount = req.TransmissionAmount;
                staging.UpdatedBy = req.UserId;
                staging.UpdatedDate = DateTime.Now;
                db.EnrollmentAddonStagings.Add(staging);
                db.SaveChanges();

                res.StagingId = staging.Id;
                res.Status = true;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/UpdateAddonforEnrollment", req.CustomerId);
                res.Status = false;
            }
            return res;
        }

        public bool UpdateAddon(Guid CustomerId, Guid UserId, Guid BankId)
        {
            try
            {
                var enrollment = db.EnrollmentBankSelections.Where(x => x.CustomerId == CustomerId && x.StatusCode == EMPConstants.Active && x.BankId == BankId).FirstOrDefault();
                if (enrollment != null)
                {
                    var staging = db.EnrollmentAddonStagings.Where(x => x.CustomerId == CustomerId && x.IsActive == true && x.BankId == enrollment.BankId).FirstOrDefault();
                    if (staging != null)
                    {
                        staging.IsActive = false;
                        staging.UpdatedBy = UserId;
                        staging.UpdatedDate = DateTime.Now;
                        db.SaveChanges();

                        EnrollmentBankSelectionDTO _objenr = new EnrollmentBankSelectionDTO();
                        _objenr.CustomerId = CustomerId;
                        _objenr.BankId = enrollment.BankId;
                        _objenr.IsServiceBureauFee = staging.IsSvbFee;
                        _objenr.IsTransmissionFee = staging.IsTransmissionFee;
                        _objenr.QuestionId = enrollment.QuestionId;
                        _objenr.ServiceBureauBankAmount = staging.SvbAddonAmount;
                        _objenr.TransmissionBankAmount = staging.TransmissionAddonAmount;
                        _objenr.UserId = UserId;

                        bool updated = EnrollmentBankSelectSave(_objenr);
                        return updated;
                    }
                    else
                        return false;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/UpdateAddon", CustomerId);
                return false;
            }
        }

        public UpdateEnrollmentAddon getStagingAddon(Guid CustomerId, Guid BankId)
        {
            UpdateEnrollmentAddon res = new DTO.UpdateEnrollmentAddon();
            try
            {
                var isexist = db.EnrollmentAddonStagings.Where(x => x.BankId == BankId && x.CustomerId == CustomerId && x.IsActive == true).FirstOrDefault();
                if (isexist != null)
                {
                    res.IsSvbFee = isexist.IsSvbFee;
                    res.IsTransmissionFee = isexist.IsTransmissionFee;
                    res.SvbAmount = isexist.SvbAddonAmount;
                    res.TransmissionAmount = isexist.TransmissionAddonAmount;
                    return res;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getStagingAddon", CustomerId);
                return null;
            }
        }

        public List<ApprovedBanks> getApprovedBanks(Guid CustomerId, Guid BankId)
        {
            try
            {
                var banks = (from s in db.BankEnrollments
                             join b in db.BankMasters on s.BankId equals b.Id
                             where s.CustomerId == CustomerId && s.IsActive == true && s.StatusCode == EMPConstants.Approved && s.ArchiveStatusCode != EMPConstants.Archive
                             select new ApprovedBanks
                             {
                                 BankName = b.BankName,
                                 BankId = b.Id
                             }).ToList();

                var currentbank = (from s in db.BankMasters
                                   where s.Id == BankId && s.StatusCode == EMPConstants.Active
                                   select new ApprovedBanks
                                   {
                                       BankId = s.Id,
                                       BankName = s.BankName
                                   }).FirstOrDefault();

                banks.Add(currentbank);
                return banks.DistinctBy(x => x.BankId).ToList();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentBankSelectionService/getApprovedBanks", CustomerId);
                return new List<ApprovedBanks>();
            }
        }

    }
}
