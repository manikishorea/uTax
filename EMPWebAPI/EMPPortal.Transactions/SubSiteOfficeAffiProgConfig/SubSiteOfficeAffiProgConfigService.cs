using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.SubSiteOfficeAffiProgConfig.DTO;
using System.Data.Entity;
using EMP.Core.Utilities;

namespace EMPPortal.Transactions.SubSiteOfficeAffiProgConfig
{
    public class SubSiteOfficeAffiProgConfigService : ISubSiteOfficeAffiProgConfigService
    {
        DatabaseEntities db = new DatabaseEntities();

        #region "SUB-SITE OFFICE CONFIGURATION"
        /// <summary>
        /// This Method is used to Get SubSite Office Configuration Details
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public IQueryable<SubSiteOfficeAffiliateProgramConfigDTO> SubSiteOfficeAffiProgConfig(Guid UserId)
        {
            db = new DatabaseEntities();
            var data = db.SubSiteOfficeAffiliateProgramConfigs.Where(o => o.CustomerInformation_Id == UserId).Select(o => new SubSiteOfficeAffiliateProgramConfigDTO
            {
                Id = o.Id,
                CustomerInformation_Id = o.CustomerInformation_Id.ToString(),
                AffiliateProgramId = o.AffiliateProgramId.ToString(),
                AffiliateProgramCharge = o.AffiliateProgramCharge,
            }).DefaultIfEmpty();

            return data;
        }

        /// <summary>
        /// This method is used to save and update the Sub Site Office Configuration Details
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        public async Task<int> SaveSubSiteOfficeAffiProgConfig(SubSiteOfficeAffiliateProgramConfigDTO dto)
        {
            int entityState = 0;

            SubSiteOfficeAffiliateProgramConfig model = new SubSiteOfficeAffiliateProgramConfig();

            if (dto != null)
            {
                int Id;
                Guid refId, AffiliateProgId;
                if (int.TryParse(dto.Id.ToString(), out Id))
                {
                    model = db.SubSiteOfficeAffiliateProgramConfigs.Where(a => a.Id == Id).FirstOrDefault();
                    if (model != null)
                    {
                        entityState = (int)System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        return -1;
                    }
                }
                else
                {
                    //model.Id = Guid.NewGuid();
                    entityState = (int)System.Data.Entity.EntityState.Added;
                }

                bool IsRefId = Guid.TryParse(dto.CustomerInformation_Id, out refId);
                if (!IsRefId)
                {
                    return -1;
                }

                bool IsAffiliateProgId = Guid.TryParse(dto.AffiliateProgramId, out AffiliateProgId);
                if (!IsAffiliateProgId)
                {
                    return -1;
                }

                model.CustomerInformation_Id = refId;
                model.AffiliateProgramId = AffiliateProgId;
                model.AffiliateProgramCharge = dto.AffiliateProgramCharge;
                model.StatusCode = EMPConstants.Active;

                if (entityState == (int)System.Data.Entity.EntityState.Added)
                {
                    model.CreatedBy = dto.UserId ?? Guid.Empty;
                    model.CreatedDate = System.DateTime.Now;
                    db.SubSiteOfficeAffiliateProgramConfigs.Add(model);

                    //Guid SiteMapId;
                    //Guid.TryParse("2639fb0a-0caa-47cf-b315-587e7ce86aef", out SiteMapId);
                    //CustomerConfigurationStatu ConfigStatusModel = new CustomerConfigurationStatu();
                    //ConfigStatusModel.Id = Guid.NewGuid();
                    //ConfigStatusModel.CustomerId = dto.UserId ?? Guid.Empty;
                    //ConfigStatusModel.SitemapId = SiteMapId;
                    //ConfigStatusModel.StatusCode = "done";
                    //ConfigStatusModel.UpdatedBy = dto.UserId ?? Guid.Empty;
                    //ConfigStatusModel.UpdatedDate = DateTime.Now;
                    //db.CustomerConfigurationStatus.Add(ConfigStatusModel);
                }
                else
                {
                    model.LastUpdatedBy = dto.UserId ?? Guid.Empty;
                    model.LastUpdatedDate = System.DateTime.Now;

                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                }
            }
            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return model.Id;
            }
            catch (Exception ex)
            {
                return -1;
                throw;
            }
        }
        #endregion

    }
}