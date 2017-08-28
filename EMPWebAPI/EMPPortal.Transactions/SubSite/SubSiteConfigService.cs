using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPPortal.Transactions.SubSite.DTO;
using EMP.Core.Utilities;
using System.Data.Entity;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.SubSite
{
    public class SubSiteConfigService : ISubSiteConfigService
    {
        DatabaseEntities db = new DatabaseEntities();
        DropDownService ddService = new DropDownService();
        /// <summary>
        /// This Method is used to Get the Main Office Configuration Details
        /// </summary>
        /// <returns></returns>
        public async Task<SubSiteDTO> GetById(Guid userid)
        {
            try
            {
                DropDownService ddService = new DropDownService();
                List<EntityHierarchyDTO> EntityHierarchyDTOs = new List<EntityHierarchyDTO>();
                EntityHierarchyDTOs = ddService.GetEntityHierarchies(userid);

                Guid ParentId = Guid.Empty;
                int Level = EntityHierarchyDTOs.Count;
                if (EntityHierarchyDTOs.Count == 1)
                {
                    var LevelOne = EntityHierarchyDTOs.Where(o => o.Customer_Level == 0).FirstOrDefault();
                    if (LevelOne != null)
                    {
                        ParentId = LevelOne.CustomerId ?? Guid.Empty;
                    }
                }
                else
                {
                    var LevelOne = EntityHierarchyDTOs.Where(o => o.Customer_Level == 1).FirstOrDefault();
                    if (LevelOne != null)
                    {
                        ParentId = LevelOne.CustomerId ?? Guid.Empty;
                    }
                }

                userid = ParentId;

                db = new DatabaseEntities();
                var data = await db.SubSiteConfigurations.Where(o => (o.StatusCode == EMPConstants.Active || o.StatusCode == EMPConstants.Pending) && o.emp_CustomerInformation_ID == userid).Select(o => new SubSiteDTO
                {
                    Id = o.ID.ToString(),
                    refId = o.emp_CustomerInformation_ID.ToString(),
                    IsuTaxManageingEnrolling = o.IsuTaxManageingEnrolling,
                    IsuTaxPortalEnrollment = o.IsuTaxPortalEnrollment,
                    IsuTaxManageOnboarding = o.IsuTaxManageOnboarding,
                    EnrollmentEmails = o.EnrollmentEmails??false,
                    IsuTaxCustomerSupport = o.IsuTaxCustomerSupport,
                    NoofSupportStaff = o.NoofSupportStaff,
                    NoofDays = o.NoofDays,
                    OpenHours = o.OpenHours.Value.ToString(),// "hh:mm tt"),// "hh:mm tt"),
                    CloseHours = o.CloseHours.Value.ToString(),//"hh:mm tt"),//"hh:mm tt"),
                    TimeZone = o.TimeZone.ToString(),
                    SubSiteTaxReturn = o.SubSiteTaxReturn,
                    IsAutoEnrollAffiliateProgram = o.IsAutoEnrollAffiliateProgram,
                    IsSubSiteEFINAllow = o.IsSubSiteEFINAllow,
                    CanSubSiteLoginToEmp = o.CanSubSiteLoginToEmp,
                    Affiliates = o.SubSiteAffiliateProgramConfigs.Select(s => new SubSiteAffiliateProgramDTO() { AffiliateProgramId = s.AffiliateProgramMaster_ID }).ToList(),
                    SubSiteBankQuestions = o.SubSiteBankConfigs.Select(s => new SubSiteBankQuestionDTO() { BankId = s.BankMaster_ID, QuestionId = s.SubQuestion_ID ?? Guid.Empty }).ToList()
                }).FirstOrDefaultAsync();

                if (data != null)
                {
                    DateTime dt = new DateTime();
                    bool res = DateTime.TryParse(data.OpenHours, out dt);
                    string ddt = new DateTime().Add(dt.TimeOfDay).ToString("hh:mm tt");
                    data.OpenHours = ddt;

                    DateTime dt1 = new DateTime();
                    bool res1 = DateTime.TryParse(data.CloseHours, out dt1);
                    string ddt1 = new DateTime().Add(dt1.TimeOfDay).ToString("hh:mm tt");
                    data.CloseHours = ddt1;
                }
                else
                {
                    if (Level > 2)
                    {
                        data = new DTO.SubSiteDTO();
                        var SubSiteOfficeCon = db.SubSiteOfficeConfigs.Where(o => o.RefId == ParentId).FirstOrDefault();

                        if (SubSiteOfficeCon != null)
                        {
                            if (SubSiteOfficeCon.SOorSSorEFIN == 2 || SubSiteOfficeCon.SOorSSorEFIN == 3)
                                data.IsSubSiteEFINAllow = true;
                        }
                    }

                    // OpenHours = new DateTime().Add(o.OpenHours.Value).ToString("hh:mm tt"),// "hh:mm tt"),
                    //     CloseHours = new DateTime().Add(o.CloseHours.Value).ToString("hh:mm tt"),//"hh:mm tt"),
                }

                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteConfigService/GetById", userid);
                return null;
            }
        }

        /// <summary>
        /// This method is used to Save the main office details 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Guid BankServiceSave(SubSiteBankServiceDTO dto)
        {
            int entityState = 0;
            SubSiteConfiguration model = new SubSiteConfiguration();
            if (dto != null)
            {
                Guid Id, refId;

                bool IsRefId = Guid.TryParse(dto.refId, out refId);
                if (!IsRefId)
                {
                    return Guid.Empty;
                }

                if (Guid.TryParse(dto.Id, out Id))
                {
                    model = db.SubSiteConfigurations.Where(a => a.ID == Id).FirstOrDefault();
                    if (model.IsuTaxManageOnboarding != null && model.IsuTaxCustomerSupport != null)
                    {
                        Guid SiteMapId;
                        int EntityType = 0;
                        if (Guid.TryParse("68882c05-5914-4fdb-b284-e33d6c029f5a", out SiteMapId))
                        {
                            CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                            ConfigStatusModel = db.CustomerConfigurationStatus.Where(o => o.CustomerId == refId && o.SitemapId == SiteMapId).FirstOrDefault();
                            if (ConfigStatusModel == null)
                            {
                                ConfigStatusModel = new CustomerConfigurationStatu();
                                ConfigStatusModel.Id = Guid.NewGuid();
                            }
                            else
                            {
                                EntityType = (int)System.Data.Entity.EntityState.Modified;
                            }

                            ConfigStatusModel.CustomerId = dto.UserId ?? Guid.Empty;
                            ConfigStatusModel.SitemapId = SiteMapId;
                            ConfigStatusModel.StatusCode = "done";
                            ConfigStatusModel.UpdatedBy = dto.UserId ?? Guid.Empty;
                            ConfigStatusModel.UpdatedDate = DateTime.Now;

                            if (EntityType == (int)System.Data.Entity.EntityState.Modified)
                            {
                                db.Entry(ConfigStatusModel).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                            }
                        }
                    }

                    if (model != null)
                    {
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else { return Guid.Empty; }
                }
                else
                {
                    model.ID = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }



                model.emp_CustomerInformation_ID = refId; // newguid;

                if (dto.IsuTaxManageingEnrolling != null)
                {
                    model.IsuTaxManageingEnrolling = dto.IsuTaxManageingEnrolling.Value;
                }

                if (dto.IsuTaxPortalEnrollment != null)
                {
                    model.IsuTaxPortalEnrollment = dto.IsuTaxPortalEnrollment.Value;
                }


                if (dto.IsSubSiteEFINAllow != null)
                {
                    model.IsSubSiteEFINAllow = dto.IsSubSiteEFINAllow.Value;
                }
                else
                {
                    model.IsSubSiteEFINAllow = false;
                }


                model.EnrollmentEmails = dto.EnrollmentEmails;
                model.CanSubSiteLoginToEmp = true;

                model.StatusCode = EMPConstants.Active;
                model.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                model.LastUpdatedDate = System.DateTime.Now;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    model.CreatedBy = dto.UserId ?? Guid.Empty;
                    model.CreatedDate = System.DateTime.Now;
                    db.SubSiteConfigurations.Add(model);

                }
                else
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;

                    var Bank = db.SubSiteBankConfigs.Where(o => o.SubSiteConfiguration_ID == model.ID).ToList();

                    if (Bank.ToList().Count > 0)
                    {
                        db.SubSiteBankConfigs.RemoveRange(Bank);
                    }
                }

                List<SubSiteBankConfig> SubSiteBankConfigs = new List<SubSiteBankConfig>();

                foreach (var item in dto.SubSiteBankQuestions)
                {
                    SubSiteBankConfig SubSiteBank = new SubSiteBankConfig();
                    SubSiteBank.ID = Guid.NewGuid();
                    SubSiteBank.emp_CustomerInformation_ID = model.emp_CustomerInformation_ID;
                    SubSiteBank.BankMaster_ID = item.BankId;
                    SubSiteBank.SubSiteConfiguration_ID = model.ID;
                    SubSiteBank.SubQuestion_ID = item.QuestionId;

                    SubSiteBank.CreatedBy = dto.UserId ?? Guid.Empty;
                    SubSiteBank.CreatedDate = DateTime.Now;
                    SubSiteBank.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                    SubSiteBank.LastUpdatedDate = DateTime.Now;

                    SubSiteBankConfigs.Add(SubSiteBank);
                }

                if (dto.SubSiteBankQuestions.ToList().Count > 0)
                {
                    db.SubSiteBankConfigs.AddRange(SubSiteBankConfigs);
                }

                if (dto.IsuTaxManageingEnrolling ?? false)
                {
                    var custinfo = (from s in db.emp_CustomerInformation
                                    join sy in db.SalesYearMasters on s.SalesYearID equals sy.Id
                                    where s.Id == refId
                                    select new
                                    {
                                        s.IsMSOUser,
                                        s.SalesforceAccountID,
                                        sy.SalesYear
                                    }
                                ).FirstOrDefault();

                    CustomerInformation.CustomerInformationService objCIS = new CustomerInformation.CustomerInformationService();
                    string desc = (custinfo.IsMSOUser ?? false) ? "Automated MSO " + custinfo.SalesYear ?? 0 + " Enrollment Case " : "Automated Desktop " + custinfo.SalesYear ?? 0 + " Enrollment Case";
                    objCIS.SaveCSRCase(custinfo.SalesforceAccountID, (custinfo.SalesYear ?? 0).ToString(), true, desc, false);
                }
            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return model.ID;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteConfigService/BankServiceSave", model.ID);
                return Guid.Empty;
                throw;
            }
        }

        /// <summary>
        /// This method is used to Save the main office details 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Guid OnBoardingServiceSave(SubSiteOnBoardingDTO dto)
        {
            int entityState = 0;
            SubSiteConfiguration model = new SubSiteConfiguration();
            if (dto != null)
            {
                Guid Id, refId;
                bool IsRefId = Guid.TryParse(dto.refId, out refId);
                if (!IsRefId)
                {
                    return Guid.Empty;
                }

                if (Guid.TryParse(dto.Id, out Id))
                {
                    model = db.SubSiteConfigurations.Where(a => a.ID == Id).FirstOrDefault();
                    if (model.IsuTaxManageingEnrolling != null && model.IsuTaxCustomerSupport != null)
                    {
                        Guid SiteMapId; int EntityType = 0;
                        if (Guid.TryParse("68882c05-5914-4fdb-b284-e33d6c029f5a", out SiteMapId))
                        {
                            CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                            ConfigStatusModel = db.CustomerConfigurationStatus.Where(o => o.CustomerId == refId && o.SitemapId == SiteMapId).FirstOrDefault();
                            if (ConfigStatusModel == null)
                            {
                                ConfigStatusModel = new CustomerConfigurationStatu();
                                ConfigStatusModel.Id = Guid.NewGuid();
                            }
                            else
                            {
                                EntityType = (int)System.Data.Entity.EntityState.Modified;
                            }

                            ConfigStatusModel.CustomerId = refId;
                            ConfigStatusModel.SitemapId = SiteMapId;
                            ConfigStatusModel.StatusCode = "done";
                            ConfigStatusModel.UpdatedBy = dto.UserId ?? Guid.Empty;
                            ConfigStatusModel.UpdatedDate = DateTime.Now;

                            if (EntityType == (int)System.Data.Entity.EntityState.Modified)
                            {
                                db.Entry(ConfigStatusModel).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                            }
                        }
                    }

                    if (model != null)
                    {
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else { return Guid.Empty; }
                }
                else
                {
                    model.ID = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }
                model.emp_CustomerInformation_ID = refId; // newguid;
                if (dto.IsuTaxManageOnboarding != null)
                {
                    model.IsuTaxManageOnboarding = dto.IsuTaxManageOnboarding.Value;
                }
                model.StatusCode = EMPConstants.Active;
                model.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                model.LastUpdatedDate = System.DateTime.Now;
                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    model.CreatedBy = dto.UserId ?? Guid.Empty;
                    model.CreatedDate = System.DateTime.Now;
                    db.SubSiteConfigurations.Add(model);
                }
                else
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }


                var custinfo = (from s in db.emp_CustomerInformation
                                join sy in db.SalesYearMasters on s.SalesYearID equals sy.Id
                                where s.Id == refId
                                select new
                                {
                                    s.IsMSOUser,
                                    s.SalesforceAccountID,
                                    sy.SalesYear
                                }).FirstOrDefault();

                CustomerInformation.CustomerInformationService objCIS = new CustomerInformation.CustomerInformationService();
                string desc = (custinfo.IsMSOUser ?? false) ? "Automated MSO " + custinfo.SalesYear ?? 0 + " Onboarding Case " : "Automated Desktop " + custinfo.SalesYear ?? 0 + " Onboarding Case";
                var caseId = objCIS.SaveCSRCase(custinfo.SalesforceAccountID, (custinfo.SalesYear ?? 0).ToString(), dto.IsuTaxManageOnboarding ?? false, desc);

                var cinfo = db.emp_CustomerInformation.Where(x => x.Id == refId).FirstOrDefault();
                cinfo.OnBoardPrimaryKey = caseId;
                                
            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return model.ID;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteConfigService/OnBoardingServiceSave", Guid.Empty);
                return Guid.Empty;
                throw;
            }
        }

        /// <summary>
        /// This method is used to Save the main office details 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public Guid SupportServiceSave(SubSiteSupportDTO dto)
        {
            int entityState = 0;
            SubSiteConfiguration model = new SubSiteConfiguration();
            if (dto != null)
            {
                Guid Id, refId;
                bool IsRefId = Guid.TryParse(dto.refId, out refId);
                if (!IsRefId)
                {
                    return Guid.Empty;
                }
                if (Guid.TryParse(dto.Id, out Id))
                {
                    model = db.SubSiteConfigurations.Where(a => a.ID == Id).FirstOrDefault();
                    if (model.IsuTaxManageingEnrolling != null && model.IsuTaxManageOnboarding != null)
                    {
                        Guid SiteMapId;
                        int EntityType = 0;
                        if (Guid.TryParse("68882c05-5914-4fdb-b284-e33d6c029f5a", out SiteMapId))
                        {
                            CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                            ConfigStatusModel = db.CustomerConfigurationStatus.Where(o => o.CustomerId == refId && o.SitemapId == SiteMapId).FirstOrDefault();
                            if (ConfigStatusModel == null)
                            {
                                ConfigStatusModel = new CustomerConfigurationStatu();
                                ConfigStatusModel.Id = Guid.NewGuid();
                            }
                            else
                            {
                                EntityType = (int)System.Data.Entity.EntityState.Modified;
                            }

                            ConfigStatusModel.CustomerId = refId;
                            ConfigStatusModel.SitemapId = SiteMapId;
                            ConfigStatusModel.StatusCode = "done";
                            ConfigStatusModel.UpdatedBy = dto.UserId ?? Guid.Empty;
                            ConfigStatusModel.UpdatedDate = DateTime.Now;

                            if (EntityType == (int)System.Data.Entity.EntityState.Modified)
                            {
                                db.Entry(ConfigStatusModel).State = System.Data.Entity.EntityState.Modified;
                            }
                            else
                            {
                                db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                            }
                        }
                    }
                    if (model != null)
                    {
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else { return Guid.Empty; }
                }
                else
                {
                    model.ID = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }
                model.emp_CustomerInformation_ID = refId; // newguid;
                if (dto.IsuTaxCustomerSupport != null)
                {
                    model.IsuTaxCustomerSupport = dto.IsuTaxCustomerSupport.Value;
                }
                if (dto.NoofSupportStaff != null)
                {
                    model.NoofSupportStaff = dto.NoofSupportStaff.Value;
                }

                if (!string.IsNullOrEmpty(dto.NoofDays))
                {
                    model.NoofDays = dto.NoofDays;
                }

                if (dto.OpenHours != null)
                {
                    DateTime dt;
                    bool res = DateTime.TryParse(dto.OpenHours, out dt);
                    if (!res) { return Guid.Empty; }
                    model.OpenHours = dt.TimeOfDay;
                }

                if (dto.CloseHours != null)
                {
                    DateTime dt;
                    bool res = DateTime.TryParse(dto.CloseHours, out dt);

                    if (!res)
                    {
                        return Guid.Empty;
                    }
                    model.CloseHours = dt.TimeOfDay;
                }

                if (!string.IsNullOrEmpty(dto.TimeZone))
                {
                    model.TimeZone = dto.TimeZone;
                }

                if (dto.IsAutoEnrollAffiliateProgram != null)
                {
                    model.IsAutoEnrollAffiliateProgram = dto.IsAutoEnrollAffiliateProgram.Value;
                }

                if (dto.SubSiteTaxReturn != null)
                {
                    model.SubSiteTaxReturn = dto.SubSiteTaxReturn.Value;
                }

                model.StatusCode = EMPConstants.Active;
                model.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                model.LastUpdatedDate = System.DateTime.Now;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    model.CreatedBy = dto.UserId ?? Guid.Empty;
                    model.CreatedDate = System.DateTime.Now;
                    db.SubSiteConfigurations.Add(model);
                }
                else
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;

                    var Affiliate = db.SubSiteAffiliateProgramConfigs.Where(o => o.SubSiteConfiguration_ID == model.ID).ToList();

                    if (Affiliate.ToList().Count > 0)
                    {
                        db.SubSiteAffiliateProgramConfigs.RemoveRange(Affiliate);
                    }
                }
                SaveSubOfficeDetails(refId, dto.SubSiteTaxReturn.Value);
                List<SubSiteAffiliateProgramConfig> SubSiteAffiliatePrograms = new List<SubSiteAffiliateProgramConfig>();

                foreach (var item in dto.Affiliates)
                {
                    SubSiteAffiliateProgramConfig SubSiteAffiliateProgram = new SubSiteAffiliateProgramConfig();
                    SubSiteAffiliateProgram.ID = Guid.NewGuid();
                    SubSiteAffiliateProgram.emp_CustomerInformation_ID = model.emp_CustomerInformation_ID;
                    SubSiteAffiliateProgram.AffiliateProgramMaster_ID = item.AffiliateProgramId;
                    SubSiteAffiliateProgram.SubSiteConfiguration_ID = model.ID;

                    SubSiteAffiliateProgram.CreatedBy = dto.UserId ?? Guid.Empty;
                    SubSiteAffiliateProgram.CreatedDate = DateTime.Now;
                    SubSiteAffiliateProgram.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                    SubSiteAffiliateProgram.LastUpdatedDate = DateTime.Now;

                    SubSiteAffiliatePrograms.Add(SubSiteAffiliateProgram);
                }

                if (dto.Affiliates.ToList().Count > 0)
                {
                    db.SubSiteAffiliateProgramConfigs.AddRange(SubSiteAffiliatePrograms);
                }
            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return model.ID;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteConfigService/SupportServiceSave", Guid.Empty);
                return Guid.Empty;
                throw;
            }
        }

        public bool SaveSubOfficeDetails(Guid CustID, int iSubSiteTaxReturn)
        {
            try
            {
                var dbcust = (from cu in db.emp_CustomerInformation where cu.ParentId == CustID select cu);
                foreach (var cid in dbcust)
                {
                    DatabaseEntities _db = new DatabaseEntities();
                    SubSiteOfficeConfig oSubSiteOfficeConfigurations = new SubSiteOfficeConfig();
                    oSubSiteOfficeConfigurations = _db.SubSiteOfficeConfigs.Where(a => a.RefId == cid.Id).FirstOrDefault();
                    if (oSubSiteOfficeConfigurations != null)
                    {
                        if (iSubSiteTaxReturn == 1)
                            oSubSiteOfficeConfigurations.SubSiteSendTaxReturn = true;
                        else
                            oSubSiteOfficeConfigurations.SubSiteSendTaxReturn = false;
                        _db.SaveChanges();
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteConfigService/SaveSubOfficeDetails", CustID);
                return false;
            }
        }

        /// <summary>
        /// This Method is used to Get the Main Office Configuration Details
        /// </summary>
        /// <returns></returns>
        public async Task<SubSiteDTO> GetOnlyConfigById(Guid userid)
        {
            try
            {
                db = new DatabaseEntities();
                var data = await db.SubSiteConfigurations.Where(o => (o.StatusCode == EMPConstants.Active || o.StatusCode == EMPConstants.Pending) && o.emp_CustomerInformation_ID == userid).Select(o => new SubSiteDTO
                {
                    Id = o.ID.ToString(),
                    refId = o.emp_CustomerInformation_ID.ToString(),
                    IsuTaxManageingEnrolling = o.IsuTaxManageingEnrolling,
                    IsuTaxPortalEnrollment = o.IsuTaxPortalEnrollment,
                    IsuTaxManageOnboarding = o.IsuTaxManageOnboarding,
                    IsuTaxCustomerSupport = o.IsuTaxCustomerSupport,
                    NoofSupportStaff = o.NoofSupportStaff,
                    NoofDays = o.NoofDays,
                    OpenHours = o.OpenHours.Value.ToString(),// "hh:mm tt"),// "hh:mm tt"),
                    CloseHours = o.CloseHours.Value.ToString(),//"hh:mm tt"),//"hh:mm tt"),
                    TimeZone = o.TimeZone.ToString(),
                    SubSiteTaxReturn = o.SubSiteTaxReturn,
                    IsAutoEnrollAffiliateProgram = o.IsAutoEnrollAffiliateProgram,
                    IsSubSiteEFINAllow = o.IsSubSiteEFINAllow,
                }).FirstOrDefaultAsync();

                if (data != null)
                {
                    DateTime dt = new DateTime();
                    bool res = DateTime.TryParse(data.OpenHours, out dt);
                    string ddt = new DateTime().Add(dt.TimeOfDay).ToString("hh:mm tt");
                    data.OpenHours = ddt;

                    DateTime dt1 = new DateTime();
                    bool res1 = DateTime.TryParse(data.CloseHours, out dt1);
                    string ddt1 = new DateTime().Add(dt1.TimeOfDay).ToString("hh:mm tt");
                    data.CloseHours = ddt1;
                }
                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "SubSiteConfigService/GetOnlyConfigById", userid);
                return null;
            }
        }
    }
}
