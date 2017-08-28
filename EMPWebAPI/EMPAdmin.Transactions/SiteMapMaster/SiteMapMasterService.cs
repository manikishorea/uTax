using System;
using System.Linq;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.SiteMapMaster.DTO;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using EMP.Core.Utilities;
namespace EMPAdmin.Transactions.SiteMapMaster
{
    public class SiteMapMasterService : IDisposable
    {
        public DatabaseEntities db = new DatabaseEntities();

        public async Task<List<SiteMapMasterDTO>> GetSitemapMaster(Guid userId)
        {
            //SitemapMaster sitemapMaster = await db.SitemapMasters.FindAsync(id);
            int SitemapTypeId = Convert.ToInt32((int)EMPConstants.SitemapType.Admin);
            db = new DatabaseEntities();
            var sitemap = await (from sitemapMaster in db.SitemapMasters
                                 join siteMapRolePermission in db.SiteMapRolePermissions on sitemapMaster.Id equals siteMapRolePermission.SiteMapId
                                 join userRolesMap in db.UserRolesMaps on siteMapRolePermission.RoleId equals userRolesMap.RoleId
                                 join userMaster in db.UserMasters on userRolesMap.UserId equals userMaster.Id
                                 where userMaster.Id.Equals(userId) && sitemapMaster.SitemapTypeID == SitemapTypeId && sitemapMaster.IsVisible == true
                                 select new SiteMapMasterDTO
                                 {
                                     Id = sitemapMaster.Id,
                                     Name = sitemapMaster.Name,
                                     URL = sitemapMaster.URL,
                                     Description = sitemapMaster.Description,
                                     StatusCode = sitemapMaster.StatusCode,
                                     DisplayOrder = sitemapMaster.DisplayOrder
                                 }).Distinct().OrderBy(o => o.DisplayOrder).ToListAsync();
            return sitemap;
        }

        public async Task<List<SiteMapMasterDTO>> GetSitemapMaster_Admin()
        {
          
            int SitemapTypeId = Convert.ToInt32((int)EMPConstants.SitemapType.Admin);
            db = new DatabaseEntities();
            var sitemap = await (from sitemapMaster in db.SitemapMasters
                                 where sitemapMaster.SitemapTypeID == SitemapTypeId && sitemapMaster.IsVisible == true
                                 select new SiteMapMasterDTO
                                 {
                                     Id = sitemapMaster.Id,
                                     Name = sitemapMaster.Name,
                                     URL = sitemapMaster.URL,
                                     Description = sitemapMaster.Description,
                                     StatusCode = sitemapMaster.StatusCode,
                                     DisplayOrder = sitemapMaster.DisplayOrder
                                 }).Distinct().OrderBy(o => o.DisplayOrder).ToListAsync();
            return sitemap;
        }

        #region IDisposable Support
        public void Dispose()
        {
            db.Dispose();
            throw new NotImplementedException();
        }
        #endregion
    }
}
