using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMP.Core.Utilities;
using System.Data.Entity;
using EMPPortal.Transactions.SubSiteFees.DTO;
using EMPPortal.Transactions.Configuration;
using EMPPortal.Transactions.DropDowns.DTO;
using EMPPortal.Transactions.DropDowns;

namespace EMPPortal.Transactions.SubSiteFees
{
    public class SubSiteFeeService : ISubSiteFeeService
    {
        DatabaseEntities db = new DatabaseEntities();

        /// <summary>
        /// This Method is used to Get the Main Office Configuration Details
        /// </summary>
        /// <returns></returns>
        public IQueryable<SubSiteFeeDTO> GetSubSiteFeeById(Guid UserId)
        {
            try
            {
                DropDownService ddService = new DropDownService();

                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(UserId);

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
                }



                db = new DatabaseEntities();
                var DBQuery = (from sfc in db.SubSiteFeeConfigs
                               where (sfc.StatusCode == EMPConstants.Active) && sfc.emp_CustomerInformation_ID == TopParentId
                               select sfc);
                List<SubSiteFeeDTO> lstsbfee = new List<SubSiteFeeDTO>();
                foreach (var qr in DBQuery)
                {
                    SubSiteFeeDTO odto = new SubSiteFeeDTO();
                    odto.Id = qr.ID.ToString();
                    odto.refId = qr.emp_CustomerInformation_ID.ToString();
                    odto.IsAddOnFeeCharge = qr.IsAddOnFeeCharge;
                    odto.IsSameforAll = qr.IsSameforAll;
                    odto.IsSubSiteAddonFee = qr.IsSubSiteAddonFee;
                    odto.ServiceorTransmission = qr.ServiceorTransmission;
                    odto.SubSiteBankFees = db.SubSiteBankFeesConfigs.Where(a => a.SubSiteFeeConfig_ID == qr.ID).Select(a => new SubSiteBankFeesDTO
                    {
                        BankMaster_ID = a.BankMaster_ID.ToString(),
                        BankMaxFees = a.BankMaxFees,
                        BankMaxFees_MSO = a.BankMaxFees_MSO
                    }).ToList();
                    lstsbfee.Add(odto);
                }
                return lstsbfee.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteFeeService/GetSubSiteFeeById", UserId);
                return new List<SubSiteFeeDTO>().AsQueryable();
            }
        }

        /// <summary>
        /// This method is used to Save the main office details 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int Save(SubSiteFeesDTO dto)
        {
            int entityState = 0;
            int iretval = 0;

            List<SubSiteFeeConfig> subsitefeeconfigs = new List<SubSiteFeeConfig>();
            try
            {
                if (dto != null)
                {
                    Guid newguid2 = Guid.NewGuid();

                    bool IsRefId = Guid.TryParse(dto.refId, out newguid2);
                    if (IsRefId)
                    {
                        var SubSiteFeeConfig = db.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == newguid2).ToList();
                        if (SubSiteFeeConfig.Count() > 0)
                        {
                            db.SubSiteFeeConfigs.RemoveRange(SubSiteFeeConfig);
                            db.SaveChanges();
                        }

                        var subsiteBankFeeconfig = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == newguid2).ToList();
                        if (subsiteBankFeeconfig.Count() > 0)
                        {
                            db.SubSiteBankFeesConfigs.RemoveRange(subsiteBankFeeconfig);
                            db.SaveChanges();
                        }
                    }

                    foreach (var item in dto.SubsiteFees)
                    {
                        SubSiteFeeConfig subsitefeeconfig = new SubSiteFeeConfig();

                        subsitefeeconfig.ID = Guid.NewGuid();
                        entityState = (int)System.Data.Entity.EntityState.Added;
                        SaveBankDetail(item.SubSiteBankFees, subsitefeeconfig.ID, newguid2, item.UserId ?? Guid.Empty);
                        subsitefeeconfig.emp_CustomerInformation_ID = newguid2; // newguid;
                        subsitefeeconfig.IsAddOnFeeCharge = item.IsAddOnFeeCharge;

                        if (subsitefeeconfig.IsAddOnFeeCharge)
                        {
                            subsitefeeconfig.IsSameforAll = item.IsSameforAll;
                        }
                        else
                        {
                            subsitefeeconfig.IsSameforAll = false;
                        }

                        subsitefeeconfig.IsSubSiteAddonFee = item.IsSubSiteAddonFee;
                        subsitefeeconfig.ServiceorTransmission = item.ServiceorTransmission;
                        subsitefeeconfig.StatusCode = EMPConstants.Active;
                        subsitefeeconfig.LastUpdatedBy = item.UserId ?? Guid.Empty;
                        subsitefeeconfig.LastUpdatedDate = System.DateTime.Now;

                        if (entityState == (int)System.Data.Entity.EntityState.Added)
                        {
                            subsitefeeconfig.CreatedBy = item.UserId ?? Guid.Empty;
                            subsitefeeconfig.CreatedDate = System.DateTime.Now;
                            subsitefeeconfigs.Add(subsitefeeconfig);
                        }
                        else
                        {

                            // For Sub-Site Fee Changes
                            if (subsitefeeconfig.IsSameforAll)
                            {
                                var childRecord = db.emp_CustomerInformation.Where(o => o.ParentId == subsitefeeconfig.emp_CustomerInformation_ID && o.IsActivationCompleted != 1).Select(o => new { o.Id, o.IsActivationCompleted }).ToList();
                                // var IsActiveChild = childRecord.ToList().Where(o => o.IsActivationCompleted == 1).Any();
                                //if (!IsActiveChild)
                                //{
                                foreach (var ChildItem in childRecord)
                                {
                                    var subsitebankfees = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == ChildItem.Id).ToList();
                                    foreach (var subsitebankfeeItem in subsitebankfees)
                                    {
                                        var ParentSubSiteBankFees = item.SubSiteBankFees.Where(o => o.BankMaster_ID == subsitebankfeeItem.BankMaster_ID.ToString()).FirstOrDefault();
                                        if (ParentSubSiteBankFees != null)
                                        {
                                            subsitebankfeeItem.BankMaxFees = ParentSubSiteBankFees.BankMaxFees;
                                            subsitebankfeeItem.BankMaxFees_MSO = ParentSubSiteBankFees.BankMaxFees_MSO ?? 0;

                                            db.Entry(subsitebankfeeItem).State = System.Data.Entity.EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                    }
                                }
                            }

                            db.Entry(subsitefeeconfig).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    db.SubSiteFeeConfigs.AddRange(subsitefeeconfigs);
                    db.SaveChanges();
                }

                db.Dispose();

                return 1;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteFeeService/GetSubSiteBankFeeById", dto.UserId);
                iretval = 0;
                return iretval;
                throw;
            }
        }

        /// <summary>
        /// This method is used to save bank details based on subsite id
        /// </summary>
        /// <param name="sbDto"></param>
        /// <param name="SubSiteID"></param>
        /// <param name="RefID"></param>
        /// <returns></returns>
        public bool SaveBankDetail(List<SubSiteBankFeesDTO> sbDto, Guid SubSiteID, Guid RefID, Guid UserId)
        {
            bool iretval = false;
            try
            {
                foreach (var it in sbDto)
                {
                    Guid newGenId = Guid.NewGuid();
                    SubSiteBankFeesConfig subsitebankfeesconfig = new SubSiteBankFeesConfig();
                    subsitebankfeesconfig.ID = newGenId;
                    subsitebankfeesconfig.emp_CustomerInformation_ID = RefID;
                    subsitebankfeesconfig.BankMaster_ID = new Guid(it.BankMaster_ID);
                    subsitebankfeesconfig.BankMaxFees = it.BankMaxFees;
                    subsitebankfeesconfig.BankMaxFees_MSO = it.BankMaxFees_MSO;
                    subsitebankfeesconfig.ServiceOrTransmitter = it.ServiceorTransmission;
                    subsitebankfeesconfig.SubSiteFeeConfig_ID = SubSiteID;

                    subsitebankfeesconfig.CreatedBy = UserId;
                    subsitebankfeesconfig.CreatedDate = DateTime.Now;
                    subsitebankfeesconfig.LastUpdatedBy = UserId;
                    subsitebankfeesconfig.LastUpdatedDate = DateTime.Now;

                    db.SubSiteBankFeesConfigs.Add(subsitebankfeesconfig);
                    db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteFeeService/SaveBankDetail", Guid.Empty);
                return false;
            }
            return iretval;
        }

        /// <summary>
        /// This method is used to save the transmitter fees details
        /// </summary>
        /// <param name="tmDto"></param>
        /// <param name="RefID"></param>
        /// <returns></returns>
        public bool SaveCustomerAssociatedFees(TransmitterFeeDTO tmDto)
        {
            bool iretval = false;
            //int entityState = 0;
            try
            {
                if (tmDto != null)
                {
                    Guid refId;
                    bool IsRefId1 = Guid.TryParse(tmDto.refId, out refId);

                    var FeeMasterEntity = db.FeeMasters;
                    List<CustomerAssociatedFee> customerassociatedfeelst = new List<CustomerAssociatedFee>();
                    foreach (var itm in FeeMasterEntity)
                    {
                        if (db.CustomerAssociatedFees.Where(a => a.FeeMaster_ID == itm.Id && a.emp_CustomerInformation_ID == refId).Count() == 0)
                        {
                            CustomerAssociatedFee customerassociatedfee = new CustomerAssociatedFee();
                            Guid newGenId = Guid.NewGuid();
                            customerassociatedfee.ID = newGenId;
                            customerassociatedfee.emp_CustomerInformation_ID = refId;
                            customerassociatedfee.FeeMaster_ID = itm.Id;
                            customerassociatedfee.Amount = itm.Amount ?? 0;
                            customerassociatedfee.IsActive = true;
                            if (itm.FeeTypeId == 2)
                                customerassociatedfee.IsEdit = true;
                            else
                                customerassociatedfee.IsEdit = false;
                            customerassociatedfee.CreatedBy = tmDto.UserId ?? Guid.Empty;
                            customerassociatedfee.CreatedDate = System.DateTime.Now;
                            customerassociatedfee.LastUpdatedBy = tmDto.UserId ?? Guid.Empty;
                            customerassociatedfee.LastUpdatedDate = System.DateTime.Now;
                            customerassociatedfeelst.Add(customerassociatedfee);
                        }
                    }

                    if (customerassociatedfeelst.Count > 0)
                    {
                        db.CustomerAssociatedFees.AddRange(customerassociatedfeelst);
                        db.SaveChanges();
                    }
                    iretval = true;
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteFeeService/GetSubSiteBankFeeById", Guid.Empty);
                return false;
            }
            return iretval;
        }

        /// <summary>
        /// This Method is used to update Amount for Customer Associated Fees
        /// </summary>
        /// <param name="tmDto"></param>
        /// <returns></returns>
        public bool UpdateCustomerAssociatedFees(TransmitterFeeDTO tmDto)
        {
            bool iretval = false;
            //int entityState = 0;
            try
            {
                if (tmDto != null)
                {
                    Guid newguid, newguid2;

                    bool IsRefId = Guid.TryParse(tmDto.FeeMaster_ID, out newguid2);
                    bool IsRefId1 = Guid.TryParse(tmDto.refId, out newguid);
                    CustomerAssociatedFee customerassociatedfee = new CustomerAssociatedFee();

                    var data = db.CustomerAssociatedFees.Where(a => a.emp_CustomerInformation_ID == newguid && a.FeeMaster_ID == newguid2 && a.IsActive == true).FirstOrDefault();
                    if (data != null)
                    {
                        customerassociatedfee = data;
                        customerassociatedfee.LastUpdatedBy = tmDto.UserId ?? Guid.Empty;
                        customerassociatedfee.LastUpdatedDate = System.DateTime.Now;
                        customerassociatedfee.IsActive = false;
                        db.Entry(customerassociatedfee).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        db.Dispose();
                        //EMP.Core.Utilities.AuditLogger.AudittrailLog(customerassociatedfee, data, "Modify", tmDto.UserId ?? Guid.Empty);
                    }

                    db = new DatabaseEntities();
                    CustomerAssociatedFee customerassociatedfeeSave = new CustomerAssociatedFee();

                    customerassociatedfeeSave = data;
                    customerassociatedfeeSave.ID = Guid.NewGuid();
                    customerassociatedfeeSave.Amount = tmDto.Amount;
                    customerassociatedfeeSave.IsActive = true;
                    customerassociatedfeeSave.CreatedBy = tmDto.UserId ?? Guid.Empty;
                    customerassociatedfeeSave.CreatedDate = System.DateTime.Now;
                    customerassociatedfeeSave.LastUpdatedBy = tmDto.UserId ?? Guid.Empty;
                    customerassociatedfeeSave.LastUpdatedDate = System.DateTime.Now;
                    db.CustomerAssociatedFees.Add(customerassociatedfeeSave);

                    db.SaveChanges();
                    db.Dispose();
                    //EMP.Core.Utilities.AuditLogger.AudittrailLog(customerassociatedfeeSave, null, "Add", tmDto.UserId ?? Guid.Empty);
                    iretval = true;
                }
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteFeeService/UpdateCustomerAssociatedFees", Guid.Empty);
                return false;
            }
            return iretval;
        }

        public bool IsEnrollmentSubmit(Guid Id)
        {
            try
            {
                var BankEnroll = db.BankEnrollments.Any(a => a.CustomerId == Id && a.IsActive == true && a.StatusCode != EMPConstants.Ready);
                if (BankEnroll)
                {
                    return true;
                }
                else
                {
                    var custominfo = db.emp_CustomerInformation.Where(a => a.ParentId == Id);
                    if (custominfo != null)
                    {
                        foreach (var cu in custominfo)
                        {
                            var BankEnrolls = db.BankEnrollments.Any(a => a.CustomerId == cu.Id && a.IsActive == true && a.StatusCode != EMPConstants.Ready);
                            if (BankEnrolls)
                            {
                                return true;
                            }
                            var subcustominfo = db.emp_CustomerInformation.Where(a => a.ParentId == cu.Id);
                            foreach (var item in subcustominfo)
                            {
                                var SubBankEnrolls = db.BankEnrollments.Any(a => a.CustomerId == item.Id && a.IsActive == true && a.StatusCode != EMPConstants.Ready);
                                if (SubBankEnrolls)
                                {
                                    return true;
                                }
                                var subsubcustominfo = db.emp_CustomerInformation.Where(a => a.ParentId == item.Id);
                                foreach (var item1 in subsubcustominfo)
                                {
                                    var SubBankEnrolls1 = db.BankEnrollments.Any(a => a.CustomerId == item1.Id && a.IsActive == true && a.StatusCode != EMPConstants.Ready);
                                    if (SubBankEnrolls1)
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteFeeService/IsEnrollmentSubmit", Guid.Empty);
                return false;
            }
        }

        public IQueryable<BankDateCrossDTO> IsSalesYearBank(Guid Id)
        {
            try
            {
                List<BankDateCrossDTO> Bankdatelst = new List<BankDateCrossDTO>();
                var SyId = db.emp_CustomerInformation.Where(a => a.Id == Id).Select(a => a.SalesYearID).FirstOrDefault();
                if (SyId != null)
                {
                    var data = (from sy in db.SalesYearMasters
                                join bm in db.BankAssociatedCutofDates on sy.Id equals bm.SalesYearID
                                where sy.Id == SyId
                                select bm);
                    if (data != null)
                    {
                        foreach (var itm in data)
                        {
                            BankDateCrossDTO obdct = new BankDateCrossDTO();
                            DateTime currentdate = System.DateTime.Now;
                            obdct.BankId = itm.BankID;
                            obdct.SalesYearID = itm.SalesYearID;

                            if (currentdate.Date > Convert.ToDateTime(itm.CutofDate))
                            {
                                obdct.Active = false;
                            }
                            else
                            {
                                obdct.Active = true;
                            }

                            //var irtval = db.SubSiteBankConfigs.Any(a => a.emp_CustomerInformation_ID == Id && a.BankMaster_ID == itm.BankID);
                            //if (irtval)
                            //{
                            //    if (currentdate.Date > Convert.ToDateTime(itm.CutofDate))
                            //    {
                            //        obdct.Active = false;
                            //    }
                            //    else
                            //    {
                            //        obdct.Active = true;
                            //    }
                            //}
                            //else
                            //{
                            //    obdct.Active = true;
                            //}

                            Bankdatelst.Add(obdct);
                        }
                    }
                }
                return Bankdatelst.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteFeeService/IsSalesYearBank", Id);
                return new List<BankDateCrossDTO>().AsQueryable();
            }
        }

        public bool GetFeeLink(Guid CustomerId)
        {

            var custInfo = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
            bool ipsel = true;
            // iProtect GUID
            Guid affGuid = new Guid("25a4379b-4df1-4a65-aec1-30dcd587eeb7");
            // checking the iprotect affiliate condition

            var ismapped = db.AffiliationProgramEntityMaps.Where(x => x.AffiliateProgramId == affGuid && x.EntityId == custInfo.EntityId).FirstOrDefault();
            if (ismapped != null)
            {
                var isselected = db.SubSiteAffiliateProgramConfigs.Where(x => x.emp_CustomerInformation_ID == CustomerId && x.AffiliateProgramMaster_ID == affGuid).FirstOrDefault();
                if (isselected == null)
                    ipsel = false;
            }

            Sitemap.SitemapService objsite = new Sitemap.SitemapService();
            bool manuFlag = objsite.GetServiceTransmeterFeeLink(CustomerId);
            if (manuFlag || ipsel || custInfo.Quote_Rebate__c)
            {
                return true;
            }

            return false;
        }
    }
}
