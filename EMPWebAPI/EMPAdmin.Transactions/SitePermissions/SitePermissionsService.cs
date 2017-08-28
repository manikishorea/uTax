using System;
using System.Linq;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.SiteMapMaster.DTO;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using EMPAdmin.Transactions.SitePermissions.DTO;
namespace EMPAdmin.Transactions.SitePermissions
{
    public class SitePermissionsService : IDisposable
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<SitePermissionsDTO> GetSitePermissions(Guid id)
        {
            var SitePermissions = db.SitemapPermissionMaps.Where(o => o.SiteMapId == id).Select(o => new SitePermissionsDTO
            {
                Id = o.Id,
                SiteMapId = o.SiteMapId,
                PermissionId = o.PermissionId,
                IsVisisble = o.IsVisisble,
                SiteName = o.SitemapMaster.Name,
                Name = o.PermissionMaster.Name,
                DisplayOrder = o.PermissionMaster.DisplayOrder

            }).Distinct().DefaultIfEmpty();

            return SitePermissions;
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
