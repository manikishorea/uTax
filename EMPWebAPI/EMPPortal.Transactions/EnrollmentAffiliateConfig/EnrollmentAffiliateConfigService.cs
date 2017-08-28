using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.EnrollmentAffiliateConfig.DTO;
using System.Data.Entity;
using EMP.Core.Utilities;

namespace EMPPortal.Transactions.EnrollmentAffiliateConfig
{
    public class EnrollmentAffiliateConfigService : IEnrollmentAffiliateConfigService
    {
        private DatabaseEntities db = new DatabaseEntities();

        #region "Enrollment Affiliate Configuration Detail"
        /// <summary>
        /// This Method is used to Get Enrollment Affiliate Configuration Detail
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<EnrollmentAffiliateConfigDTO> GetEnrollmentAffiProgConfig(Guid UserId)
        {
            try
            {
                db = new DatabaseEntities();
                var data = db.EnrollmentAffiliateConfigurations.Where(o => o.CustomerId == UserId).Select(o => new EnrollmentAffiliateConfigDTO
                {
                    Id = o.Id.ToString(),
                    CustomerId = o.CustomerId.ToString(),
                    AffiliateProgramId = o.AffiliateProgramId.ToString(),
                    AffiliateProgramCharge = o.AffiliateProgramCharge,
                }).DefaultIfEmpty();

                if (data.FirstOrDefault() == null)
                {
                    var customer = db.emp_CustomerInformation.Where(x => x.Id == UserId).FirstOrDefault();
                    if (customer.ParentId == null || customer.ParentId == Guid.Empty)
                        return null;
                    var isAuto = db.SubSiteConfigurations.Where(x => x.emp_CustomerInformation_ID == customer.ParentId.Value && x.StatusCode == EMPConstants.Active).FirstOrDefault();
                    if (isAuto != null)
                    {
                        bool _isauto = isAuto.IsAutoEnrollAffiliateProgram.HasValue ? isAuto.IsAutoEnrollAffiliateProgram.Value : false;
                        if (!_isauto)
                        {
                            //return null;
                            return GetEnrollmentAffiProgMainConfig(customer.ParentId.Value);
                        }
                        else
                        {
                            return GetEnrollmentAffiProgMainConfig(customer.ParentId.Value);
                        }
                    }
                    else
                        return null;
                }
                return data;
            }
            catch (Exception ex)
            {
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentAffiliateConfigService/GetEnrollmentAffiProgConfig", UserId);
                return null;
            }
        }

        /// <summary>
        /// This method is used to save and update the Enrollment Affiliate Configuration Detail
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public int SaveEnrollmentAffiProgConfig(EnrollmentAffiliateConfigDetailDTO dto)
        {
            //int entityState = 0;

            Guid refId = Guid.Empty, UserId, AffiliateProgId;

            bool IsUserId = Guid.TryParse(dto.UserId.ToString(), out UserId);
            if (!IsUserId)
            {
                return -1;
            }

            if (dto != null)
            {


               // entityState = (int)System.Data.Entity.EntityState.Added;

                foreach (EnrollmentAffiliateConfigDTO item in dto.Affiliates)
                {
                    EnrollmentAffiliateConfiguration model = new EnrollmentAffiliateConfiguration();

                    if (item != null)
                    {
                        bool IsRefId = Guid.TryParse(item.CustomerId.ToString(), out refId);
                        if (!IsRefId)
                        {
                            return -1;
                        }

                        bool IsAffiliateProgId = Guid.TryParse(item.AffiliateProgramId, out AffiliateProgId);
                        if (!IsAffiliateProgId)
                        {
                            return -1;
                        }

                        decimal AffiliateProgramCharge;
                        bool IsAffiliateProgramCharge = decimal.TryParse(item.AffiliateProgramCharge.ToString(), out AffiliateProgramCharge);
                        if (!IsAffiliateProgramCharge)
                        {
                            AffiliateProgramCharge = 0;
                        }

                        model.Id = Guid.NewGuid();
                        model.CustomerId = refId;
                        model.AffiliateProgramId = AffiliateProgId;
                        model.AffiliateProgramCharge = AffiliateProgramCharge;
                        model.StatusCode = EMPConstants.Active;

                        model.LastUpdatedBy = UserId;
                        model.LastUpdatedDate = System.DateTime.Now;
                        model.CreatedBy = UserId;
                        model.CreatedDate = System.DateTime.Now;

                        db.EnrollmentAffiliateConfigurations.Add(model);

                        //if (entityState == (int)System.Data.Entity.EntityState.Added)
                        //{
                        //    Guid SiteMapId;
                        //    Guid.TryParse("2f7d1b90-78aa-4a93-85ec-81cd8b10a545", out SiteMapId);
                        //    CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                        //    ConfigStatusModel.Id = Guid.NewGuid();
                        //    ConfigStatusModel.CustomerId = dto.UserId ?? Guid.Empty;
                        //    ConfigStatusModel.SitemapId = SiteMapId;
                        //    ConfigStatusModel.StatusCode = "done";
                        //    ConfigStatusModel.UpdatedBy = dto.UserId ?? Guid.Empty;
                        //    ConfigStatusModel.UpdatedDate = DateTime.Now;
                        //    db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                        //}
                    }
                }

                var _AffiliateList = db.EnrollmentAffiliateConfigurations.Where(a => a.CustomerId == refId).ToList();
                if (_AffiliateList.ToList().Count > 0)
                {
                    db.EnrollmentAffiliateConfigurations.RemoveRange(_AffiliateList);
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
                EMPPortal.Core.Utilities.ExceptionLogger.LogException(ex.ToString(), "EnrollmentAffiliateConfigService/SaveEnrollmentAffiProgConfig", Guid.Empty);
                return -1;
                throw;
            }
        }


        /// <summary>
        /// This Method is used to Get Enrollment Affiliate Configuration Detail
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<EnrollmentAffiliateConfigDTO> GetEnrollmentAffiProgMainConfig(Guid ParentId)
        {
            db = new DatabaseEntities();
            var data = db.SubSiteAffiliateProgramConfigs.Where(o => o.emp_CustomerInformation_ID == ParentId).Select(o => new EnrollmentAffiliateConfigDTO
            {
                Id = o.ID.ToString(),
                CustomerId = o.emp_CustomerInformation_ID.ToString(),
                AffiliateProgramId = o.AffiliateProgramMaster_ID.ToString(),
                AffiliateProgramCharge = 0,
                IsAutoEnrollAffiliateProgram = db.SubSiteConfigurations.Where(a => a.emp_CustomerInformation_ID == ParentId && a.ID == o.SubSiteConfiguration_ID).Select(a => a.IsAutoEnrollAffiliateProgram).FirstOrDefault() ?? false
            }).DefaultIfEmpty();

            return data;
        }

        #endregion

    }
}