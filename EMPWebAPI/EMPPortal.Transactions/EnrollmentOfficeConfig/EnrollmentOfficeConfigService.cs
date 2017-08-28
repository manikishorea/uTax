using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.EnrollmentOfficeConfig.DTO;
using System.Data.Entity;
using EMP.Core.Utilities;
using EMPPortal.Transactions.DropDowns;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.EnrollmentOfficeConfig
{
    public class EnrollmentOfficeConfigService : IEnrollmentOfficeConfigService
    {
        DatabaseEntities db = new DatabaseEntities();

        #region "Enrollment Affiliate Configuration Detail"
        /// <summary>
        /// This Method is used to Get Enrollment Affiliate Configuration Detail
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public EnrollmentOfficeConfigDTO GetEnrollmentOfficeConfig(Guid UserId)
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.EnrollmentOfficeConfigurations.Where(o => o.CustomerId == UserId).Select(o => new EnrollmentOfficeConfigDTO
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    IsMainSiteTransmitTaxReturn = o.IsMainSiteTransmitTaxReturn,
                    NoofComputers = o.NoofComputers,
                    NoofTaxProfessionals = o.NoofTaxProfessionals,
                    PreferredLanguage = o.PreferredLanguage,
                    IsSoftwareOnNetwork = o.IsSoftwareOnNetwork,
                    StatusCode = o.StatusCode,
                }).FirstOrDefault();

                if (data == null)
                {
                    var accountdata = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == UserId && x.StatusCode == EMPConstants.Active).Select(x => new EnrollmentOfficeConfigDTO
                    {
                        Id = null,
                        CustomerId = x.emp_CustomerInformation_ID,
                        IsMainSiteTransmitTaxReturn = null,
                        NoofComputers = x.ComputerswillruninSoftware,
                        NoofTaxProfessionals = x.TaxProfessionals,
                        PreferredLanguage = x.PreferredSupportLanguage,
                        IsSoftwareOnNetwork = x.IsSoftwarebeInstalledNetwork,
                        StatusCode = x.StatusCode,
                    }).FirstOrDefault();

                    return accountdata;
                }

                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentOfficeConfigService/GetEnrollmentOfficeConfig", UserId);
                return null;
            }
        }

        /// <summary>
        /// This method is used to save and update the Enrollment Affiliate Configuration Detail
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int SaveEnrollmentOfficeConfig(EnrollmentOfficeConfigDTO dto)
        {
            // int entityState = 0;

            EnrollmentOfficeConfiguration model = new EnrollmentOfficeConfiguration();
            if (dto != null)
            {
                Guid Id, UserId, CustomerId;
                int entityState = (int)System.Data.Entity.EntityState.Added;
                if (!Guid.TryParse(dto.CustomerId.ToString(), out CustomerId))
                {
                    return -1;
                }
                if (!Guid.TryParse(dto.UserId.ToString(), out UserId))
                {
                    return -1;
                }
                if (dto.Id != null)
                {
                    if (!Guid.TryParse(dto.Id.ToString(), out Id))
                    {
                        return -1;
                    }

                    entityState = (int)System.Data.Entity.EntityState.Modified;

                    if (Id == Guid.Empty)
                    {
                        Id = Guid.NewGuid();
                        model.Id = Id;
                    }
                }
                else
                {
                    Id = Guid.NewGuid();
                    model.Id = Id;
                }

                if (entityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    model = db.EnrollmentOfficeConfigurations.Where(a => a.Id == dto.Id).FirstOrDefault();
                    if (model == null)
                    {
                        return -1;
                    }
                }
                model.CustomerId = CustomerId;
                model.IsMainSiteTransmitTaxReturn = dto.IsMainSiteTransmitTaxReturn;
                model.NoofComputers = dto.NoofComputers;
                model.NoofTaxProfessionals = dto.NoofTaxProfessionals;
                model.NoofTaxProfessionals = dto.NoofTaxProfessionals;
                model.PreferredLanguage = dto.PreferredLanguage;
                model.IsSoftwareOnNetwork = dto.IsSoftwareOnNetwork;
                model.StatusCode = EMPConstants.Active;
                model.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                model.LastUpdatedDate = System.DateTime.Now;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    model.CreatedBy = dto.UserId ?? Guid.Empty;
                    model.CreatedDate = System.DateTime.Now;
                    db.EnrollmentOfficeConfigurations.Add(model);
                }
                else
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }

                var custInfo = db.emp_CustomerInformation.Where(x => x.Id == CustomerId).FirstOrDefault();
                if (custInfo.EntityId == (int)EMPConstants.Entity.MO || custInfo.EntityId == (int)EMPConstants.Entity.SVB)
                {
                    var accountconfig = db.MainOfficeConfigurations.Where(x => x.emp_CustomerInformation_ID == CustomerId && x.StatusCode==EMPConstants.Active).FirstOrDefault();
                    if (accountconfig != null)
                    {
                        accountconfig.ComputerswillruninSoftware = dto.NoofComputers ?? 0;
                        accountconfig.IsSoftwarebeInstalledNetwork = dto.IsSoftwareOnNetwork??false;
                        accountconfig.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                        accountconfig.LastUpdatedDate = DateTime.Now;
                        accountconfig.PreferredSupportLanguage = dto.PreferredLanguage ?? 0;
                        accountconfig.TaxProfessionals = dto.NoofTaxProfessionals ?? 0;                        
                    }
                }
            }
            try
            {
                db.SaveChanges();
                db.Dispose();
                return 1;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentOfficeConfigService/SaveEnrollmentOfficeConfig", Guid.Empty);
                return -1;
            }
        }


        /// <summary>
        /// This Method is used to Get Enrollment Affiliate Configuration Detail
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public EnrollmentOfficeConfigDTO GetEnrollmentOfficeMainConfig(Guid ParentId)
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.MainOfficeConfigurations.Where(o => o.emp_CustomerInformation_ID == ParentId).Select(o => new EnrollmentOfficeConfigDTO
                {
                    Id = o.Id,
                    CustomerId = o.emp_CustomerInformation_ID,
                    IsMainSiteTransmitTaxReturn = o.IsSiteTransmitTaxReturns,
                    NoofComputers = o.ComputerswillruninSoftware,
                    NoofTaxProfessionals = o.TaxProfessionals,
                    PreferredLanguage = o.PreferredSupportLanguage,
                    IsSoftwareOnNetwork = o.IsSoftwarebeInstalledNetwork,
                    StatusCode = o.StatusCode,
                }).FirstOrDefault();

                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentOfficeConfigService/GetEnrollmentOfficeMainConfig", ParentId);
                return null;
            }
        }

        #endregion

    }
}