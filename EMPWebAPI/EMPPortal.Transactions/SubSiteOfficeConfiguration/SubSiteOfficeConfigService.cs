using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.SubSiteOfficeConfiguration.DTO;
using System.Data.Entity;
using EMP.Core.Utilities;
using EMPPortal.Transactions.Configuration;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;
using EMPPortal.Transactions.CustomerInformation;

namespace EMPPortal.Transactions.SubSiteOfficeConfiguration
{
    public class SubSiteOfficeConfigService : ISubSiteOfficeConfigService
    {
        DatabaseEntities db = new DatabaseEntities();

        #region "SUB-SITE OFFICE CONFIGURATION"
        /// <summary>
        /// This Method is used to Get SubSite Office Configuration Details
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public async Task<SubSiteOfficeConfigDTO> GetSubSiteOfficeConfigById(Guid userid, Guid parentId)
        {
            try
            {
                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(userid);

                Guid ParentId = Guid.Empty;

                if (EntityHierarchyDTOs.Count > 2)
                {
                    var LevelOne = EntityHierarchyDTOs.Where(o => o.Customer_Level == 1).FirstOrDefault();
                    if (LevelOne != null)
                    {
                        ParentId = LevelOne.CustomerId ?? Guid.Empty;
                    }
                    else
                    {
                        ParentId = EntityHierarchyDTOs.Where(o => o.Customer_Level == 0).Select(o => o.CustomerId).FirstOrDefault() ?? Guid.Empty;
                    }
                }


                db = new DatabaseEntities();
                var data = await db.SubSiteOfficeConfigs.Where(o => o.RefId == userid).Select(o => new SubSiteOfficeConfigDTO
                {
                    Id = o.Id.ToString(),
                    RefId = o.RefId.ToString(),
                    EFINListedOtherOffice = o.EFINListedOtherOffice,
                    SiteOwnthisEFIN = o.SiteOwnthisEFIN,
                    EFINOwnerSite = o.EFINOwnerSite,
                    SOorSSorEFIN = o.SOorSSorEFIN.ToString(),
                    SubSiteSendTaxReturn = o.SubSiteSendTaxReturn,
                    SiteanMSOLocation = o.SiteanMSOLocation,
                    IsMainSiteTransmitTaxReturn = o.IsMainSiteTransmitTaxReturn,
                    NoofTaxProfessionals = o.NoofTaxProfessionals,
                    IsSoftwareOnNetwork = o.IsSoftwareOnNetwork,
                    NoofComputers = o.NoofComputers,
                    PreferredLanguage = o.PreferredLanguage,
                    CanSubSiteLoginToEmp = o.CanSubSiteLoginToEmp ?? false,
                    IsBusinessSoftware = o.HasBusinessSoftware,
                    IsSharingEFIN = o.IsSharingEFIN
                }).FirstOrDefaultAsync();

                if (data == null)
                {
                    if (EntityHierarchyDTOs.Count > 2)
                    {
                        SubSiteOfficeConfigDTO data1 = new SubSiteOfficeConfigDTO();
                        var subsiteconfig = (from ssc in db.SubSiteOfficeConfigs where ssc.RefId == parentId select ssc).FirstOrDefault();
                        //  data1.Id = "0";
                        if (subsiteconfig != null)
                            data1.SubSiteSendTaxReturn = subsiteconfig.SubSiteSendTaxReturn;
                        else
                            data1.SubSiteSendTaxReturn = true;
                        data1.IsBusinessSoftware = db.emp_CustomerInformation.Where(x => x.Id == userid).Select(x => x.QuoteSoftwarePackage).FirstOrDefault() == EMPConstants.EnterprisePackage;
                        return data1;
                    }
                    else
                    {
                        SubSiteOfficeConfigDTO data1 = new SubSiteOfficeConfigDTO();
                        var subsiteconfig = (from ssc in db.SubSiteConfigurations where ssc.emp_CustomerInformation_ID == parentId select ssc).FirstOrDefault();
                        //  data1.Id = "0";
                        if (subsiteconfig != null)
                            data1.iIsSubSiteSendTaxReturn = subsiteconfig.SubSiteTaxReturn ?? 0;
                        else
                            data1.iIsSubSiteSendTaxReturn = 0;
                        data1.IsBusinessSoftware = db.emp_CustomerInformation.Where(x => x.Id == userid).Select(x => x.QuoteSoftwarePackage).FirstOrDefault() == EMPConstants.EnterprisePackage;
                        return data1;
                    }
                }
                else
                {
                    if (EntityHierarchyDTOs.Count < 3)
                    {
                        var subsiteconfig = (from ssc in db.SubSiteConfigurations where ssc.emp_CustomerInformation_ID == parentId select ssc).FirstOrDefault();
                        if (subsiteconfig != null)
                        {
                            data.iIsSubSiteSendTaxReturn = subsiteconfig.SubSiteTaxReturn ?? 0;
                        }
                        else
                        {
                            data.iIsSubSiteSendTaxReturn = (data.SubSiteSendTaxReturn == true) ? 1 : 0;
                        }
                    }
                }
                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetSubSiteOfficeConfigById", Guid.Empty);
                return null;
            }
        }

        /// <summary>
        /// This method is used to save and update the Sub Site Office Configuration Details
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Guid SaveSubSiteOfficeConfigInfo(SubSiteOfficeConfigDTO dto)
        {
            db = new DatabaseEntities();
            Guid MyId = Guid.Empty;
            int entityState = 0;
            bool prevBS = false;
            SubSiteOfficeConfig model = new SubSiteOfficeConfig();
            if (dto != null)
            {
                Guid Id, refId;
                if (Guid.TryParse(dto.Id, out Id))
                {
                    model = db.SubSiteOfficeConfigs.Where(a => a.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        prevBS = model.HasBusinessSoftware;
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else { return Guid.Empty; }
                }
                else
                {
                    model.Id = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }

                bool IsRefId = Guid.TryParse(dto.RefId, out refId);
                if (!IsRefId)
                {
                    return Guid.Empty;
                }
                model.RefId = refId;
                model.EFINListedOtherOffice = dto.EFINListedOtherOffice ?? false;
                model.SiteOwnthisEFIN = dto.SiteOwnthisEFIN ?? false;
                model.EFINOwnerSite = dto.EFINOwnerSite;
                int SOorSSorEFIN = 0;
                if (int.TryParse(dto.SOorSSorEFIN, out SOorSSorEFIN))
                {
                    model.SOorSSorEFIN = SOorSSorEFIN;
                }
                else
                {
                    return Guid.Empty;
                }

                model.CanSubSiteLoginToEmp = true;
                model.SubSiteSendTaxReturn = dto.SubSiteSendTaxReturn ?? false;
                model.SiteanMSOLocation = dto.SiteanMSOLocation ?? false;
                model.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                model.LastUpdatedDate = System.DateTime.Now;

                ///// Main Site Data
                model.IsMainSiteTransmitTaxReturn = dto.IsMainSiteTransmitTaxReturn;
                model.NoofTaxProfessionals = dto.NoofTaxProfessionals;
                model.IsSoftwareOnNetwork = dto.IsSoftwareOnNetwork;
                model.NoofComputers = dto.NoofComputers;
                model.PreferredLanguage = dto.PreferredLanguage;
                model.HasBusinessSoftware = dto.IsBusinessSoftware;
                model.IsSharingEFIN = dto.IsSharingEFIN;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    model.CreatedBy = dto.UserId ?? Guid.Empty;
                    model.CreatedDate = System.DateTime.Now;
                    db.SubSiteOfficeConfigs.Add(model);
                }
                else
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }

                //var enrollconfig = db.EnrollmentOfficeConfigurations.Where(x => x.CustomerId == refId && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                //if (enrollconfig != null)
                //{
                //    enrollconfig.IsSoftwareOnNetwork = dto.IsSoftwareOnNetwork;
                //    enrollconfig.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                //    enrollconfig.LastUpdatedDate = DateTime.Now;
                //    enrollconfig.NoofComputers = dto.NoofComputers;
                //    enrollconfig.NoofTaxProfessionals = dto.NoofTaxProfessionals;
                //    enrollconfig.PreferredLanguage = dto.PreferredLanguage;
                //}

                //if (model.SOorSSorEFIN == 3 && model.LastUpdatedBy != Guid.Empty)
                //{
                //    var emp_Info = db.emp_CustomerInformation.Where(o => o.Id == dto.UserId).FirstOrDefault();
                //    if (emp_Info != null)
                //    {
                //        emp_Info.IsAdditionalEFINAllowed = true;
                //        db.Entry(emp_Info).State = System.Data.Entity.EntityState.Modified;
                //    }
                //}


                var emp_MyInfo = db.emp_CustomerInformation.Where(o => o.Id == refId).FirstOrDefault();
                if (emp_MyInfo != null)
                {

                    var emp_ParentInfo = db.emp_CustomerInformation.Where(o => o.Id == emp_MyInfo.ParentId).FirstOrDefault();
                    if (emp_ParentInfo != null)
                    {
                        MyId = emp_MyInfo.Id;

                        if (emp_ParentInfo.EntityId == (int)EMPConstants.Entity.MO)
                        {
                            if (model.SOorSSorEFIN == 1)
                            {
                                emp_MyInfo.EROType = "Multi Office - Single Office";
                                emp_MyInfo.EntityId = (int)EMPConstants.Entity.MO_SO;
                            }
                            else if (model.SOorSSorEFIN == 3)
                            {
                                emp_MyInfo.EROType = "Multi Office - Additional EFIN";
                                emp_MyInfo.EntityId = (int)EMPConstants.Entity.MO_AE;
                            }
                        }
                        else if (emp_ParentInfo.EntityId == (int)EMPConstants.Entity.SVB_MO)
                        {
                            if (model.SOorSSorEFIN == 1)
                            {
                                emp_MyInfo.EROType = "Service Bureau - Multi Office - Single Office";
                                emp_MyInfo.EntityId = (int)EMPConstants.Entity.SVB_MO_SO;
                            }
                            else if (model.SOorSSorEFIN == 3)
                            {
                                emp_MyInfo.EROType = "Service Bureau - Multi Office - Additional EFIN";
                                emp_MyInfo.EntityId = (int)EMPConstants.Entity.SVB_MO_AE;
                            }
                        }
                        else if (emp_ParentInfo.EntityId == (int)EMPConstants.Entity.SVB)
                        {
                            if (model.SOorSSorEFIN == 1)
                            {
                                emp_MyInfo.EROType = "Service Bureau - Single Office";
                                emp_MyInfo.EntityId = (int)EMPConstants.Entity.SVB_SO;
                            }
                            else if (model.SOorSSorEFIN == 2)
                            {
                                emp_MyInfo.EROType = "Service Bureau - Multi Office";
                                emp_MyInfo.EntityId = (int)EMPConstants.Entity.SVB_MO;
                            }
                            else if (model.SOorSSorEFIN == 3)
                            {
                                emp_MyInfo.EROType = "Service Bureau - Additional EFIN";
                                emp_MyInfo.EntityId = (int)EMPConstants.Entity.SVB_AE;
                            }
                        }

                        if (model.SOorSSorEFIN == 3)
                        {
                            emp_MyInfo.IsAdditionalEFINAllowed = true;
                        }

                        if (emp_MyInfo.EFINStatus == (int)EMPConstants.EFINStatus_ForSub.Sharing && model.SiteOwnthisEFIN == false)
                        {
                            var UserId = model.EFINOwnerSite;
                            if (!string.IsNullOrEmpty(UserId))
                            {
                                var UserMain = (from emp in db.emp_CustomerInformation
                                                join emplog in db.emp_CustomerLoginInformation
                                                on emp.Id equals emplog.CustomerOfficeId
                                                where emplog.EMPUserId == UserId
                                                select new { emp.EFIN }).FirstOrDefault();

                                if (UserMain != null)
                                {
                                    emp_MyInfo.EFIN = UserMain.EFIN ?? 0;
                                }

                            }
                        }

                        db.Entry(emp_MyInfo).State = System.Data.Entity.EntityState.Modified;

                    }

                    if (emp_MyInfo.IsActivationCompleted == 1)
                    {
                        if (prevBS != dto.IsBusinessSoftware && dto.IsBusinessSoftware)
                        {
                            var loginfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == refId).FirstOrDefault();

                            var sy = db.SalesYearMasters.Where(x => x.Id == emp_MyInfo.SalesYearID).Select(x => x.SalesYear).FirstOrDefault();
                            CustomerInformationService cis = new CustomerInformation.CustomerInformationService();
                            cis.SaveEmpCsrData(refId, "Business Software", emp_MyInfo.SalesforceAccountID, sy.Value.ToString());

                            EmailNotification _email = new EmailNotification();
                            _email.CreatedBy = dto.UserId ?? Guid.Empty;
                            _email.CreatedDate = DateTime.Now;
                            _email.EmailCC = "";
                            _email.EmailContent = "";
                            _email.EmailSubject = "Business Software";
                            _email.EmailTo = EMPConstants.SupportutaxEmail;
                            _email.EmailType = (int)EMPConstants.EmailTypes.BusinessSoftware;
                            _email.IsSent = false;
                            _email.Parameters = loginfo.EMPUserId + "$|$" + loginfo.MasterIdentifier + "$|$" + dto.IsBusinessSoftware;
                            db.EmailNotifications.Add(_email);
                        }
                    }
                    if (prevBS != dto.IsBusinessSoftware && dto.IsBusinessSoftware && emp_MyInfo.EntityId != (int)EMPConstants.Entity.SO)
                    {
                        var loginfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == refId).FirstOrDefault();
                        var parentinfo = db.emp_CustomerInformation.Where(x => x.Id == emp_MyInfo.ParentId).FirstOrDefault();
                        var parentsb = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == emp_MyInfo.ParentId).Select(x => x.HasBusinessSoftware).FirstOrDefault();
                        if (parentinfo != null)
                        {
                            if (parentinfo.QuoteSoftwarePackage != EMPConstants.EnterprisePackage && !parentsb)
                            {
                                EmailNotification _email = new EmailNotification();
                                _email.CreatedBy = dto.UserId ?? Guid.Empty;
                                _email.CreatedDate = DateTime.Now;
                                _email.EmailCC = "";
                                _email.EmailContent = "";
                                _email.EmailSubject = "Business Software";
                                _email.EmailTo = EMPConstants.accountutaxEmail;
                                _email.EmailType = (int)EMPConstants.EmailTypes.BusinessSoftware;
                                _email.IsSent = false;
                                _email.Parameters = loginfo.EMPUserId + "$|$" + loginfo.MasterIdentifier + "$|$" + dto.IsBusinessSoftware;
                                db.EmailNotifications.Add(_email);
                            }
                        }
                    }
                }
            }

            try
            {
                db.SaveChanges();
                db.Dispose();

                if (MyId != Guid.Empty)
                {
                    DropDownService ddService = new DropDownService();
                    var items = ddService.GetBottomToTopHierarchy(MyId);
                }

                return model.Id;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/SaveSubSiteOfficeConfigInfo", Guid.Empty);
                return Guid.Empty;
            }
        }
        #endregion

        #region "CUSTOMER NOTES"
        /// <summary>
        /// This Method is used to Get the Customer Notes details by ID
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<CustomerNotesDTO> GetCustomerNotesById(Guid userid)
        {
            try
            {
                db = new DatabaseEntities();
                var data = (from cu in db.CustomerNotes where cu.RefId == userid select cu);
                List<CustomerNotesDTO> lstsbcustnotes = new List<CustomerNotesDTO>();
                foreach (var itm in data)
                {
                    CustomerNotesDTO customernotesDTO = new CustomerNotesDTO();
                    customernotesDTO.Note = itm.Note;
                    customernotesDTO.RefName = db.emp_CustomerInformation.Where(o => o.Id == itm.RefId).Select(a => a.CompanyName).FirstOrDefault();
                    customernotesDTO.CreatedDate = itm.CreatedDate.ToString("MM/dd/yyyy hh:mm tt");
                    lstsbcustnotes.Add(customernotesDTO);
                }
                return lstsbcustnotes.OrderByDescending(a => a.CreatedDate).AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetCustomerNotesById", Guid.Empty);
                return null;
            }
        }

        /// <summary>
        /// This method is used to Save and Update the Customer Note Infomration Dtails 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Guid SaveCustomerNoteInfo(CustomerNotesDTO dto)
        {
            int entityState = 0;
            CustomerNote model = new CustomerNote();
            if (dto != null)
            {
                Guid refId;
                model.Id = Guid.NewGuid();
                entityState = (int)System.Data.Entity.EntityState.Added;

                bool IsRefId = Guid.TryParse(dto.RefId, out refId);
                if (!IsRefId)
                {
                    return Guid.Empty;
                }
                model.RefId = refId;
                model.Note = dto.Note;
                model.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                model.LastUpdatedDate = System.DateTime.Now;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    model.CreatedBy = dto.UserId ?? Guid.Empty;
                    model.CreatedDate = System.DateTime.Now;
                    db.CustomerNotes.Add(model);
                }
                else
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }
            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return model.Id;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/SaveCustomerNoteInfo", model.RefId);
                return Guid.Empty;
                throw;
            }
        }
        #endregion

        #region "FEE CONFIGURATION"
        /// <summary>
        /// This Method is used to Get the Sub Site Bank Fee Configuration details by ID
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<SubSiteBankFeeConfigDTO> GetSubSiteBankFeeById(Guid userid)
        {
            try
            {
                db = new DatabaseEntities();
                var data = (from cu in db.SubSiteBankFeesConfigs where cu.emp_CustomerInformation_ID == userid select cu).ToList();

                List<SubSiteBankFeeConfigDTO> lstsbsubsiteBankFee = new List<SubSiteBankFeeConfigDTO>();

                foreach (var itm in data)
                {
                    SubSiteBankFeeConfigDTO subsiteBFDTO = new SubSiteBankFeeConfigDTO();
                    subsiteBFDTO.RefId = itm.emp_CustomerInformation_ID.ToString();
                    subsiteBFDTO.BankID = itm.BankMaster_ID.ToString();
                    subsiteBFDTO.ServiceorTransmitter = itm.ServiceOrTransmitter ?? 0;
                    subsiteBFDTO.AmountDSK = itm.BankMaxFees;
                    subsiteBFDTO.AmountMSO = itm.BankMaxFees_MSO ?? 0;
                    subsiteBFDTO.QuestionID = itm.QuestionID.ToString();
                    lstsbsubsiteBankFee.Add(subsiteBFDTO);
                }
                return lstsbsubsiteBankFee.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetSubSiteBankFeeById", userid);
                return null;
            }
        }

        /// <summary>
        /// This method is used to save and update the sub site bank fee configuration
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int SaveSubSiteBankFeeConfigInfo(List<SubSiteBankFeeConfigDTO> dto)
        {
            int entityState = 0;
            Guid refId = Guid.Empty, bankId;
            int SvbOrTrans = 0;
            SubSiteBankFeesConfig model = new SubSiteBankFeesConfig();
            if (dto != null)
            {
                foreach (SubSiteBankFeeConfigDTO item in dto)
                {
                    if (item.ServiceorTransmitter != 0)
                    {
                        bool IsRefId = Guid.TryParse(item.RefId, out refId);
                        if (!IsRefId)
                        {
                            return -1;
                        }
                        bool Isbankid = Guid.TryParse(item.BankID, out bankId);
                        if (!Isbankid)
                        {
                            return -1;
                        }
                        SvbOrTrans = item.ServiceorTransmitter;
                        model = db.SubSiteBankFeesConfigs.Where(a => a.BankMaster_ID == bankId && a.emp_CustomerInformation_ID == refId && a.ServiceOrTransmitter == item.ServiceorTransmitter).FirstOrDefault();

                        if (model != null)
                        {
                            entityState = (int)System.Data.Entity.EntityState.Modified;
                        }
                        else
                        {
                            model = new SubSiteBankFeesConfig();
                            model.ID = Guid.NewGuid();
                            entityState = (int)System.Data.Entity.EntityState.Added;
                        }

                        model.emp_CustomerInformation_ID = refId;
                        model.BankMaster_ID = new Guid(item.BankID);
                        model.ServiceOrTransmitter = item.ServiceorTransmitter;
                        model.BankMaxFees = item.AmountDSK;
                        model.BankMaxFees_MSO = item.AmountMSO;
                        // model.AmountType = item.AmountType;
                        Guid Questionid;
                        if (!Guid.TryParse(item.QuestionID, out Questionid))
                        {
                            Questionid = Guid.Empty;
                        }
                        model.QuestionID = Questionid;
                        model.LastUpdatedBy = item.UserId ?? Guid.Empty;
                        model.LastUpdatedDate = System.DateTime.Now;

                        if (entityState == (int)System.Data.Entity.EntityState.Added)
                        {
                            model.CreatedBy = item.UserId ?? Guid.Empty;
                            model.CreatedDate = System.DateTime.Now;
                            db.SubSiteBankFeesConfigs.Add(model);
                        }
                        else
                        {
                            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }
            try
            {
                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    db.SaveChanges();
                }
                db.Dispose();

                return 1;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/SaveSubSiteBankFeeConfigInfo", Guid.Empty);
                return 0;
                throw;
            }
        }


        /// <summary>
        /// This method is used to save and update the sub site bank fee configuration
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int SaveSubSiteBankFeeConfig(List<SubSiteBankFeeConfigDTO> dto)
        {
            int entityState = 0;
            Guid refId = Guid.Empty, bankId;
            int SvbOrTrans = 0;
            SubSiteBankFeesConfig model = new SubSiteBankFeesConfig();
            int check = 0;
            if (dto != null)
            {
                foreach (SubSiteBankFeeConfigDTO item in dto)
                {
                    if (item.ServiceorTransmitter != 0)
                    {
                        bool IsRefId = Guid.TryParse(item.RefId, out refId);

                        if (!IsRefId)
                        {
                            return -1;
                        }

                        if (check == 0)
                        {
                            check = 1;
                            var result = db.SubSiteBankFeesConfigs.Where(a => a.emp_CustomerInformation_ID == refId).ToList();

                            if (result.Count() > 0)
                            {
                                db.SubSiteBankFeesConfigs.RemoveRange(result);
                                db.SaveChanges();
                            }
                        }

                        bool Isbankid = Guid.TryParse(item.BankID, out bankId);
                        if (!Isbankid)
                        {
                            return -1;
                        }

                        SvbOrTrans = item.ServiceorTransmitter;

                        // model = db.SubSiteBankFeesConfigs.Where(a => a.BankMaster_ID == bankId && a.emp_CustomerInformation_ID == refId && a.ServiceOrTransmitter == item.ServiceorTransmitter).FirstOrDefault();

                        //if (model != null)
                        //{
                        //    entityState = (int)System.Data.Entity.EntityState.Modified;
                        //}
                        //else
                        //{
                        model = new SubSiteBankFeesConfig();
                        model.ID = Guid.NewGuid();
                        entityState = (int)System.Data.Entity.EntityState.Added;
                        //}

                        model.emp_CustomerInformation_ID = refId;
                        model.BankMaster_ID = new Guid(item.BankID);
                        model.ServiceOrTransmitter = item.ServiceorTransmitter;
                        model.BankMaxFees = item.AmountDSK;
                        model.BankMaxFees_MSO = item.AmountMSO;
                        // model.AmountType = item.AmountType;
                        Guid Questionid;
                        if (!Guid.TryParse(item.QuestionID, out Questionid))
                        {
                            Questionid = Guid.Empty;
                        }
                        model.QuestionID = Questionid;
                        model.LastUpdatedBy = item.UserId ?? Guid.Empty;
                        model.LastUpdatedDate = System.DateTime.Now;

                        if (entityState == (int)System.Data.Entity.EntityState.Added)
                        {
                            model.CreatedBy = item.UserId ?? Guid.Empty;
                            model.CreatedDate = System.DateTime.Now;
                            db.SubSiteBankFeesConfigs.Add(model);
                        }
                        else
                        {
                            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                }
            }

            try
            {

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    db.SaveChanges();
                }
                db.Dispose();

                return 1;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/SaveSubSiteBankFeeConfig", Guid.Empty);
                return 0;
                throw;
            }
        }

        /// <summary>
        /// This Method is used to User Id valid or not
        /// </summary>
        /// <param name="OwnID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public bool GetValidUserID(string OwnID, string EFIN, Guid CustomerId)
        {
            try
            {
                bool iretval = false;
                db = new DatabaseEntities();

                bool canupdate = true;
                var user = (from s in db.emp_CustomerInformation
                            join l in db.emp_CustomerLoginInformation on s.Id equals l.CustomerOfficeId
                            where l.EMPUserId == EFIN && s.StatusCode == EMPConstants.Active
                            select s).FirstOrDefault();
                if (user != null)
                {
                    if (user.EntityId == (int)EMPConstants.Entity.MO || user.EntityId == (int)EMPConstants.Entity.SVB)
                    {
                        var isshare = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == user.Id).Select(x => x.IsSharingEfin).FirstOrDefault();
                        canupdate = isshare;
                    }
                    else
                    {
                        var isshare = db.SubSiteOfficeConfigs.Where(x => x.RefId == user.Id).Select(x => x.IsSharingEFIN).FirstOrDefault();
                        canupdate = isshare;
                    }
                }

                var masterid = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).Select(x => x.MasterIdentifier).FirstOrDefault();
                var list = (from s in db.emp_CustomerLoginInformation
                            where s.MasterIdentifier == masterid && s.CustomerOfficeId != CustomerId && s.EMPUserId == EFIN
                            select s).Count();
                if (list > 0 && canupdate)
                    return true;
                else
                    return false;

                //DropDownService ddService = new DropDownService();
                //List<Guid> hierarchy = ddService.GetHierarchyCustomerIds(CustomerId);

                //var result = (from cu in db.emp_CustomerLoginInformation
                //              join ci in db.emp_CustomerInformation on cu.CustomerOfficeId equals ci.Id
                //              where cu.EMPUserId == EFIN && hierarchy.Contains(ci.Id) && ci.Id != CustomerId// && cu.EMPUserId != OwnID
                //              select new
                //              {
                //                  ci.Id
                //              }).FirstOrDefault();

                //if (result != null)
                //{
                //    return true;
                //}
                //return iretval;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetValidUserID", Guid.Empty);
                return false;
            }

        }

        /// <summary>
        /// This Method is used to User Id valid or not
        /// </summary>
        /// <param name="OwnID"></param>
        /// <param name="ParentID"></param>
        /// <returns></returns>
        public bool GetValidUserIDFromuTax(string OwnID, string EFIN, Guid CustomerId)
        {
            try
            {
                bool iretval = false;
                db = new DatabaseEntities();

                bool canupdate = true;
                var user = (from s in db.emp_CustomerInformation
                            join l in db.emp_CustomerLoginInformation on s.Id equals l.CustomerOfficeId
                            where l.EMPUserId == EFIN && s.StatusCode == EMPConstants.Active
                            select s).FirstOrDefault();
                if (user != null)
                {
                    if (user.EntityId == (int)EMPConstants.Entity.MO || user.EntityId == (int)EMPConstants.Entity.SVB)
                    {
                        var isshare = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == user.Id).Select(x => x.IsSharingEfin).FirstOrDefault();
                        canupdate = isshare;
                    }
                    else
                    {
                        var isshare = db.SubSiteOfficeConfigs.Where(x => x.RefId == user.Id).Select(x => x.IsSharingEFIN).FirstOrDefault();
                        canupdate = isshare;
                    }
                }

                var masterid = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == CustomerId).Select(x => x.MasterIdentifier).FirstOrDefault();
                var list = (from s in db.emp_CustomerLoginInformation
                            where s.MasterIdentifier == masterid && s.CustomerOfficeId != CustomerId && s.EMPUserId == EFIN
                            select s).Count();
                if (list > 0 && canupdate)
                    return true;
                else
                    return false;

                //DropDownService ddService = new DropDownService();
                //List<Guid> hierarchy = ddService.GetHierarchyCustomerIds(CustomerId);

                //var result = (from cu in db.emp_CustomerLoginInformation
                //              join ci in db.emp_CustomerInformation on cu.CustomerOfficeId equals ci.Id
                //              where cu.EMPUserId == EFIN && hierarchy.Contains(ci.Id) && ci.Id != CustomerId //cu.EMPUserId == OwnID && ci.ParentId == null
                //              select new
                //              {
                //                  ci.Id
                //              }).FirstOrDefault();

                //if (result != null)
                //{
                //    return true;
                //}
                //return iretval;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetValidUserIDFromuTax", Guid.Empty);
                return false;
            }
        }

        public bool GetParentUserID(Guid ParentID, string ParentEFINID)
        {
            try
            {
                bool iretval = false;
                db = new DatabaseEntities();

                //List<Guid> listentitys = new List<Guid>();
                //listentitys.Add(new Guid("3F11AC17-C5AA-412B-9DCD-4FD8BA1FE404"));
                //listentitys.Add(new Guid("734C5E39-5C33-47E0-8D75-6852DA39F907"));

                List<int> listentitys = new List<int>();
                listentitys.Add((int)EMPConstants.Entity.MO);
                listentitys.Add((int)EMPConstants.Entity.SVB);

                return iretval = (from cu in db.emp_CustomerLoginInformation
                                  join ci in db.emp_CustomerInformation on cu.CustomerOfficeId equals ci.Id
                                  where ci.Id == ParentID &&
                                  cu.EMPUserId == ParentEFINID
                                   && listentitys.Contains(ci.EntityId.Value)
                                  select new { ci.EFIN }).Any();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/GetParentUserID", ParentID);
                return false;
            }
        }

        public IQueryable<SubSiteBankFeeConfigDTO> SubSiteOfficeBankFee(Guid UserId)
        {
            try
            {
                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(UserId);

                Guid ParentId = Guid.Empty;
                Guid FeeSourceParentId = Guid.Empty;

                if (EntityHierarchyDTOs.Count > 0)
                {
                    var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
                    ParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;
                }

                db = new DatabaseEntities();
                List<int> FeeFor = new List<int>();
                FeeFor.Add((int)EMPConstants.FeesFor.SVBFees);
                FeeFor.Add((int)EMPConstants.FeesFor.TransmissionFees);
                List<SubSiteBankFeeConfigDTO> ListDTO = new List<SubSiteBankFeeConfigDTO>();

                if (EntityHierarchyDTOs.Count <= 1)
                {
                    var data = (from fee in db.FeeMasters
                                    //join cust in db.CustomerAssociatedFees on fee.Id equals cust.FeeMaster_ID
                                where FeeFor.Contains(fee.FeesFor ?? 0) //&& cust.emp_CustomerInformation_ID == ParentId
                                select new { fee.FeesFor, fee.Amount }).ToList();

                    var dataa = (from s in data
                                 group s by s.FeesFor into g
                                 select new
                                 {
                                     feesfor = g.Key,
                                     Amount = g.Sum(x => x.Amount)
                                 }).ToList();



                    foreach (var itm in dataa.ToList())
                    {
                        SubSiteBankFeeConfigDTO SubSiteBankFeeConfig = new SubSiteBankFeeConfigDTO();
                        SubSiteBankFeeConfig.ServiceorTransmitter = itm.feesfor ?? 0;
                        SubSiteBankFeeConfig.AmountDSK = itm.Amount ?? 0;
                        ListDTO.Add(SubSiteBankFeeConfig);
                    }
                }
                else
                {

                    var data = (from fee in db.FeeMasters
                                join cust in db.CustomerAssociatedFees on fee.Id equals cust.FeeMaster_ID
                                where FeeFor.Contains(fee.FeesFor ?? 0) && cust.emp_CustomerInformation_ID == ParentId
                                select new { fee.FeesFor, cust.Amount }).ToList();

                    var dataa = (from s in data
                                 group s by s.FeesFor into g
                                 select new
                                 {
                                     feesfor = g.Key,
                                     Amount = g.Sum(x => x.Amount)
                                 }).ToList();



                    foreach (var itm in dataa.ToList())
                    {
                        SubSiteBankFeeConfigDTO SubSiteBankFeeConfig = new SubSiteBankFeeConfigDTO();
                        SubSiteBankFeeConfig.ServiceorTransmitter = itm.feesfor ?? 0;
                        SubSiteBankFeeConfig.AmountDSK = itm.Amount;
                        ListDTO.Add(SubSiteBankFeeConfig);
                    }
                }

                return ListDTO.AsQueryable();
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/SubSiteOfficeBankFee", UserId);
                return null;
            }
        }

        public List<SubSiteBankFeeConfigDTO> BankFee(Guid Id)
        {
            try
            {
                List<SubSiteBankFeeConfigDTO> SubSiteBankFeeConfigDTOList = new List<SubSiteBankFeeConfigDTO>();
                var SubSiteFeeConfig = db.SubSiteFeeConfigs.Where(o => o.emp_CustomerInformation_ID == Id).ToList();
                foreach (var item in SubSiteFeeConfig)
                {
                    SubSiteBankFeeConfigDTO SubSiteBankFeeConfig = new SubSiteBankFeeConfigDTO();
                    List<SubSiteBankFeesConfig> SubSiteBankFeeConfigList = new List<SubSiteBankFeesConfig>();
                    SubSiteBankFeeConfigList = db.SubSiteBankFeesConfigs.Where(o => o.emp_CustomerInformation_ID == Id && o.SubSiteFeeConfig_ID == item.ID).ToList();
                    foreach (var BankFees in SubSiteBankFeeConfigList)
                    {
                        SubSiteBankFeeConfig.AmountDSK = BankFees.BankMaxFees;
                    }
                    SubSiteBankFeeConfig.ServiceorTransmitter = item.ServiceorTransmission == 1 ? (int)EMPConstants.FeesFor.SVBFees : (int)EMPConstants.FeesFor.TransmissionFees;
                    SubSiteBankFeeConfigDTOList.Add(SubSiteBankFeeConfig);
                }
                return SubSiteBankFeeConfigDTOList;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "CustomerPaymentOptionsService/BankFee", Id);
                return new List<SubSiteBankFeeConfigDTO>();
            }
        }
        #endregion

        public List<ApprovedBankAndAddOnFeeDTO> GetSubSiteFeeConfigUpdateAfterApprove(Guid CustomerId)
        {
            List<ApprovedBankAndAddOnFeeDTO> ApprovedBankAndAddOnFeeList = new List<ApprovedBankAndAddOnFeeDTO>();

            var query = (from be in db.BankEnrollments
                         join ebs in db.EnrollmentBankSelections on be.BankId equals ebs.BankId
                         join bank in db.BankMasters on be.BankId equals bank.Id
                         where be.CustomerId == CustomerId &&
                         ebs.CustomerId == CustomerId &&
                         be.IsActive == true && ebs.StatusCode == EMPConstants.Active
                         select new
                         {
                             bank.BankName,
                             be.BankId,
                             be.StatusCode,
                             ebs.IsServiceBureauFee,
                             ebs.IsTransmissionFee,
                             ebs.TransmissionBankAmount,
                             ebs.ServiceBureauBankAmount
                         }).ToList();

            foreach (var item in query)
            {
                ApprovedBankAndAddOnFeeDTO ApprovedBankAndAddOnFee = new ApprovedBankAndAddOnFeeDTO();
                ApprovedBankAndAddOnFee.BankName = item.BankName;
                ApprovedBankAndAddOnFee.BankId = item.BankId ?? Guid.Empty;
                ApprovedBankAndAddOnFee.IsServiceBureauFee = item.IsServiceBureauFee ?? false;
                ApprovedBankAndAddOnFee.StatusCode = item.StatusCode;
                ApprovedBankAndAddOnFee.IsTransmissionFee = item.IsTransmissionFee ?? false;
                ApprovedBankAndAddOnFee.TransmissionBankAmount = item.TransmissionBankAmount ?? 0;
                ApprovedBankAndAddOnFee.ServiceBureauBankAmount = item.ServiceBureauBankAmount ?? 0;
                ApprovedBankAndAddOnFeeList.Add(ApprovedBankAndAddOnFee);
            }

            return ApprovedBankAndAddOnFeeList;
        }

        public string GetAccountId(Guid Id)
        {
            DropDownService ddService = new DropDownService();
            List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
            EntityHierarchyDTOs = ddService.GetEntityHierarchies(Id);

            var TopFromHierarchy = EntityHierarchyDTOs.OrderByDescending(o => o.Customer_Level).FirstOrDefault();
            int EntityId = TopFromHierarchy.EntityId??0;
            Guid ParentId = TopFromHierarchy.CustomerId ?? Guid.Empty;

            if (EntityId == (int)EMPConstants.Entity.SVB || EntityId == (int)EMPConstants.Entity.MO)
            {
                var custinfo = db.emp_CustomerLoginInformation.Where(x => x.CustomerOfficeId == ParentId).FirstOrDefault();
                if (custinfo != null)
                {
                    if (string.IsNullOrEmpty(custinfo.CLAccountId))
                    {
                        return custinfo.MasterIdentifier;
                    }
                    else
                        return custinfo.CLAccountId;
                }
                else
                    return "";
            }
            else
                return "";
        }

    }



    public class ApprovedBankAndAddOnFeeDTO
    {
        public string BankName { get; set; }
        public Nullable<Guid> BankId { get; set; }
        public string StatusCode { get; set; }
        public Nullable<bool> IsServiceBureauFee { get; set; }
        public Nullable<decimal> ServiceBureauBankAmount { get; set; }
        public Nullable<bool> IsTransmissionFee { get; set; }
        public Nullable<decimal> TransmissionBankAmount { get; set; }
    }
}