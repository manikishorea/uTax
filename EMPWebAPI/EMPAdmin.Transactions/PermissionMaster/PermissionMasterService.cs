using System;
using System.Linq;
using System.Threading.Tasks;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.SiteMapMaster.DTO;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using EMPAdmin.Transactions.PermissionMaster.DTO;
using EMPAdmin.Transactions.User.DTO;
using EMPAdmin.Transactions.SitePermissions.DTO;
using EMP.Core.Utilities;
namespace EMPAdmin.Transactions.PermissionMaster
{
    public class PermissionMasterService : IDisposable
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<PermissionMasterDTO> GetPermission()
        {
            var SitePermissions = db.PermissionMasters.Select(o => new PermissionMasterDTO
            {
                Id = o.Id,
                DisplayOrder = o.DisplayOrder,
                Name = o.Name,
                StatusCode = o.StatusCode,
                UserId = o.CreatedBy

            }).Distinct().DefaultIfEmpty();

            return SitePermissions;
        }

        public async Task<List<PermissionMasterDTO>> GetPermissionByUserAndRole(Guid userId, string roleId)
        {
            string[] roleIds = roleId.Split('_');
            List<Guid> roleIdList = new List<Guid>();
            if (roleIds.Length > 0)
            {
                foreach (var item in roleIds)
                {
                    roleIdList.Add(Guid.Parse(item));
                }
            }
            //PermissionMaster permissionMaster = await db.PermissionMasters.FindAsync(id);
            var permissionMaster = await (from permMaster in db.PermissionMasters
                                          join siteMapRolePermission in db.SiteMapRolePermissions on permMaster.Id equals siteMapRolePermission.PermissionId
                                          join userRolesMap in db.UserRolesMaps on siteMapRolePermission.RoleId equals userRolesMap.RoleId
                                          join userMaster in db.UserMasters on userRolesMap.UserId equals userMaster.Id
                                          where userMaster.Id == userId && roleIdList.Contains(userRolesMap.RoleId)
                                          select new PermissionMasterDTO
                                          {
                                              Id = permMaster.Id,
                                              Name = permMaster.Name
                                          }).Distinct().ToListAsync();

            return permissionMaster;
        }

        public IQueryable<SitePermissionsDTO> GetPermissionByUserAndRole(UserRolesDTO userRoles)
        {
            List<Guid> Roles = userRoles.Roles.Select(o => o.Id).ToList();
            ////PermissionMaster permissionMaster = await db.PermissionMasters.FindAsync(id);
            //var permissionMaster = await (from permMaster in db.PermissionMasters
            //                              join siteMapRolePermission in db.SiteMapRolePermissions on permMaster.Id equals siteMapRolePermission.PermissionId
            //                              join userRolesMap in db.UserRolesMaps on siteMapRolePermission.RoleId equals userRolesMap.RoleId
            //                              join userMaster in db.UserMasters on userRolesMap.UserId equals userMaster.Id
            //                              where userMaster.Id.Equals(userRoles.UserId) && Roles.Contains(userRolesMap.RoleId)
            //                              select new PermissionMasterDTO
            //                              {
            //                                  Id = permMaster.Id,
            //                                  Name = permMaster.Name
            //                              }).Distinct().ToListAsync();
            int SiteMapTypeId = (int)EMPConstants.SitemapType.Admin;
            var sitePermissionList = (from siteroleper in db.SiteMapRolePermissions
                                      join permisionSite in db.SitemapPermissionMaps on siteroleper.PermissionId equals permisionSite.Id
                                      join permission in db.PermissionMasters on permisionSite.PermissionId equals permission.Id
                                      join sitemap in db.SitemapMasters on siteroleper.SiteMapId equals sitemap.Id
                                      where Roles.Contains(siteroleper.RoleId) && sitemap.IsVisible == true && sitemap.SitemapTypeID == SiteMapTypeId
                                      select new SitePermissionsDTO()
                                      {
                                          // Id = siteroleper.Id,
                                          SiteMapId = siteroleper.SiteMapId,
                                          // SiteName = sitemap.Name,
                                          PermissionId = permisionSite.PermissionId,
                                          PermissionName = permission.Name

                                      }).Distinct().DefaultIfEmpty();

            return sitePermissionList;
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
