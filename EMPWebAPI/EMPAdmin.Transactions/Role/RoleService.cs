using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.Role.DTO;
using EMPAdmin.Transactions.SiteMapMaster.DTO;
using EMPAdmin.Transactions.SitePermissions.DTO;
using EMP.Core.Utilities;
using System.Data.Entity.Infrastructure;

namespace EMPAdmin.Transactions.Role
{
    public class RoleService : IRoleService, IDisposable
    {

        #region RoleMaster CRUD (CREAT, READ, UPDATE, DELETE)

        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<RoleDTO> GetAllRoles()
        {
            db = new DatabaseEntities();
            var role = db.RoleMasters.Where(o => o.IsVisible == false).Select(o => new RoleDTO
            {
                Id = o.Id,
                Name = o.Name,
                IsVisible = o.IsVisible,
                StatusCode = o.StatusCode
            }).Distinct();

            return role.OrderBy(o => o.Name);
        }

        public async Task<RoleDTO> GetRole(Guid roleId)
        {
            var role = await db.RoleMasters.Where(o => o.Id == roleId).Select(o => new RoleDTO
            {
                Id = o.Id,
                Name = o.Name,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();
            return role;
        }

        //public IQueryable<RoleDTO> GetRolesByUser(Guid userId)
        //{
        //    db = new DatabaseEntities();
        //    var query = (from sitemapMaster in db.SitemapMasters
        //                 join siteMapRolePermission in db.SiteMapRolePermissions on sitemapMaster.Id equals siteMapRolePermission.SiteMapId
        //                 join userRolesMap in db.UserRolesMaps on siteMapRolePermission.RoleId equals userRolesMap.RoleId
        //                 join userMaster in db.UserMasters on userRolesMap.UserId equals userMaster.Id
        //                 where userMaster.Id.Equals(userId)
        //                 select new RoleDTO
        //                 {
        //                     Id = sitemapMaster.Id,
        //                     Name = sitemapMaster.Name,
        //                     StatusCode = sitemapMaster.StatusCode
        //                 }).Distinct();
        //    return query;
        //}

        public IQueryable<RoleDTO> GetUserRoles(Guid userId)
        {
            var roleMaster = (from r in db.RoleMasters
                              join ur in db.UserRolesMaps on r.Id equals ur.RoleId
                              where ur.UserId == userId
                              select new RoleDTO
                              {
                                  Id = r.Id,
                                  Name = r.Name
                              }).Distinct();
            return roleMaster;
        }

        public async Task<int> Save(RoleDTO _Dto, Guid Id, int EntityState)
        {
            RoleMaster _role = new RoleMaster();

            if (_Dto != null)
            {
                _role.Id = Id;
                _role.Name = _Dto.Name;
                _role.IsVisible = false;
                _role.StatusCode = EMPConstants.Active;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                var ExistRole = db.RoleMasters.Where(o => o.Id != Id && o.Name == _Dto.Name).Any();
                if (ExistRole)
                    return -1;

                _role.CreatedBy = _Dto.UserId;
                _role.CreatedDate = DateTime.Now;
                _role.LastUpdatedBy = _Dto.UserId;
                _role.LastUpdatedDate = DateTime.Now;

                db.Entry(_role).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                var ExistRole = db.RoleMasters.Where(o => o.Name == _Dto.Name).Any();
                if (ExistRole)
                    return -1;

                _role.LastUpdatedBy = _Dto.UserId;
                _role.LastUpdatedDate = DateTime.Now;
                db.RoleMasters.Add(_role);
            }
            try
            {
                await db.SaveChangesAsync();
                return 1;
            }
            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
        }

        public async Task<Guid> SaveStatus(RoleDTO _Dto, Guid Id, int EntityState)
        {
            RoleMaster _RoleMaster = new RoleMaster();
            _RoleMaster = await db.RoleMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();

            if (_RoleMaster.StatusCode == EMPConstants.InActive)
            {
                _RoleMaster.StatusCode = EMPConstants.Active;
            }
            else if (_RoleMaster.StatusCode == EMPConstants.Active)
            {
                _RoleMaster.StatusCode = EMPConstants.InActive;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                _RoleMaster.LastUpdatedDate = DateTime.Now;
                _RoleMaster.LastUpdatedBy = _Dto.UserId;
                db.Entry(_RoleMaster).State = System.Data.Entity.EntityState.Modified;
            }

            try
            {
                await db.SaveChangesAsync();
                db.Dispose();
                return _RoleMaster.Id;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_RoleMaster.Id))
                {
                    return Guid.Empty;
                }
                else
                {
                    throw;
                }
            }
        }

        private bool IsExists(Guid id)
        {
            return db.RoleMasters.Count(e => e.Id == id) > 0;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        #endregion

        public async Task<RoleDetailDTO> ScreenPermission(Guid? roleId)
        {
            List<SitePermissionsDTO> sitePermissionsDTOList = new List<SitePermissions.DTO.SitePermissionsDTO>();
            RoleService roleService = new RoleService();

            RoleDetailDTO roledto = new RoleDetailDTO();
            roledto.Role = new DTO.RoleDTO();
            roledto.PermissionMasterList = new List<SitePermissionsDTO>();
            roledto.SiteMapRolePermissionList = new List<SiteMapRolePermission>();
            roledto.SitemapMasterList = new List<SitemapMaster>();

            roledto.Role = await roleService.GetRole(roleId ?? Guid.Empty);
            roledto.SitemapMasterList = await db.SitemapMasters.ToListAsync();

            var sitePermissionList = await (from permission in db.PermissionMasters
                                            join permisionSite in db.SitemapPermissionMaps on permission.Id equals permisionSite.PermissionId
                                            join sitemap in db.SitemapMasters on permisionSite.SiteMapId equals sitemap.Id
                                            select new
                                            {
                                                permission.Id,
                                                permission.Name,
                                                permission.DisplayOrder,
                                                permisionSite.SiteMapId,
                                                permisionSite.IsVisisble,

                                            }).Distinct().OrderBy(o => o.DisplayOrder).ToListAsync();
            foreach (var item in sitePermissionList)
            {
                SitePermissionsDTO sitePermissionsDTO = new SitePermissionsDTO();
                sitePermissionsDTO.Id = item.Id;
                sitePermissionsDTO.Name = item.Name;
                sitePermissionsDTO.IsVisisble = item.IsVisisble ?? false;
                sitePermissionsDTO.SiteMapId = item.SiteMapId ?? Guid.Empty;
                sitePermissionsDTOList.Add(sitePermissionsDTO);
            }

            roledto.PermissionMasterList = sitePermissionsDTOList;
            roledto.SiteMapRolePermissionList = await db.SiteMapRolePermissions.ToListAsync();
            return roledto;
        }

        public IQueryable<SitePermissionsDTO> ScreenPermission1(Guid roleId)
        {

            var sitePermissionList = (from permisionSite in db.SitemapPermissionMaps
                                          //join siteroleper in db.SiteMapRolePermissions on permisionSite.Id equals siteroleper.PermissionId
                                          //into outer
                                          //from siteroleper in outer.DefaultIfEmpty(null)
                                      join permission in db.PermissionMasters on permisionSite.PermissionId equals permission.Id
                                      join sitemap in db.SitemapMasters on permisionSite.SiteMapId equals sitemap.Id

                                      //  where (siteroleper.RoleId == null || siteroleper.RoleId == roleId)
                                      select new SitePermissionsDTO()
                                      {
                                          Id = permisionSite.Id,
                                          SiteMapId = permisionSite.SiteMapId,
                                          PermissionId = permisionSite.PermissionId,
                                          PermissionName = permission.Name,
                                          SiteName = sitemap.Name,
                                          IsVisisble = permisionSite.IsVisisble,
                                          DisplayOrder = permission.DisplayOrder ?? 0,
                                          SiteOrder = sitemap.DisplayOrder ?? 0,
                                          // RoleId = (siteroleper.RoleId == null) ? Guid.Empty : siteroleper.RoleId,
                                          IsPermitted = db.SiteMapRolePermissions.Where(o => o.SiteMapId == permisionSite.SiteMapId && o.PermissionId == permisionSite.Id && o.RoleId == roleId).Any(),// (siteroleper.RoleId == null) ? false : siteroleper.RoleId.Equals(roleId),
                                          ParentName = db.SitemapMasters.Where(o => o.Id == sitemap.ParentId).Select(o => o.Name).FirstOrDefault()
                                      }).Distinct().DefaultIfEmpty();


            return sitePermissionList.OrderBy(x=>x.ParentName).ThenBy(a => a.SiteOrder).ThenBy(a => a.DisplayOrder);
        }

        public string ScreenPermission(FormCollection fcollection, Guid userId)
        {
            string sResult = this.SaveScreenPermissionDetails(Guid.Parse(fcollection["hdnRoleID"].ToString()), userId, fcollection);
            return sResult;
        }

        /// <summary>
        /// This method is used to Save the screen permission details
        /// </summary>
        /// <param name="ScreenPermissions"></param>
        /// <param name="RoleID"></param>
        /// <param name="CreatedBy"></param>
        /// <returns></returns>
        public string SaveScreenPermissionDetails(Guid RoleID, Guid CreatedBy, FormCollection fCollection)
        {
            try
            {
                SiteMapRolePermission SiteMapRolePermissionModel;
                using (var DatabaseEntities = new DatabaseEntities())
                {
                    var sitemapRolePermissions = DatabaseEntities.SiteMapRolePermissions.Where(y => y.RoleId == RoleID);
                    foreach (var item in sitemapRolePermissions)
                    {
                        DatabaseEntities.SiteMapRolePermissions.Remove(item);
                    }
                    DatabaseEntities.SaveChanges();

                    foreach (var item in fCollection.Keys)
                    {
                        if (item.ToString() != "hdnRoleID")
                        {
                            if (fCollection[item.ToString()] == "on")
                            {
                                string[] sitemapRolePermission = item.ToString().Split('_');
                                if (sitemapRolePermission != null)
                                {
                                    SiteMapRolePermissionModel = new SiteMapRolePermission();
                                    Guid Id = Guid.NewGuid();
                                    SiteMapRolePermissionModel.Id = Id;
                                    if (sitemapRolePermission.Count() > 0)
                                    {
                                        if (!string.IsNullOrWhiteSpace(sitemapRolePermission[0]))
                                        {
                                            SiteMapRolePermissionModel.SiteMapId = Guid.Parse(sitemapRolePermission[0]);
                                        }
                                    }
                                    if (sitemapRolePermission.Count() > 1)
                                    {
                                        if (!string.IsNullOrWhiteSpace(sitemapRolePermission[1]))
                                        {
                                            SiteMapRolePermissionModel.RoleId = Guid.Parse(sitemapRolePermission[1]);
                                        }
                                    }
                                    if (sitemapRolePermission.Count() > 2)
                                    {
                                        if (!string.IsNullOrWhiteSpace(sitemapRolePermission[2]))
                                        {
                                            SiteMapRolePermissionModel.PermissionId = Guid.Parse(sitemapRolePermission[2]);
                                        }
                                    }
                                    // SiteMapRolePermissionAPI.PutSiteMapRolePermission(Id, SiteMapRolePermissionModel);
                                    DatabaseEntities.SiteMapRolePermissions.Add(SiteMapRolePermissionModel);
                                    DatabaseEntities.SaveChanges();
                                }
                            }
                        }
                    }
                }
                return "Permissions assigned successfully";
            }
            catch (Exception ex)
            {
                // ExceptionTrans.SaveException(ex.ToString(), 1, ex.Message, "SaveScreenPermissionDetails");
                return ex.Message;
            }
        }

        public IQueryable<SitePermissionsDTO> ScreenPermission()
        {
            var sitePermissionList = (from permission in db.PermissionMasters
                                      join permisionSite in db.SitemapPermissionMaps on permission.Id equals permisionSite.PermissionId
                                      join sitemap in db.SitemapMasters on permisionSite.SiteMapId equals sitemap.Id
                                      select new SitePermissionsDTO
                                      {
                                          Id = permission.Id,
                                          Name = permission.Name,
                                          DisplayOrder = permission.DisplayOrder,
                                          SiteMapId = permisionSite.SiteMapId,
                                          SiteName = sitemap.Name,
                                          IsVisisble = permisionSite.IsVisisble,
                                      }).Distinct().OrderBy(o => o.SiteMapId).ThenBy(o => o.DisplayOrder).AsQueryable();

            return sitePermissionList;
        }

        //public async Task<RoleDTO> Create(RoleDTO role)
        //{
        //    RoleMaster roleMaster = new RoleMaster();
        //    roleMaster.Id = Guid.NewGuid();
        //    roleMaster.IsVisible = role.IsVisible;
        //    db.RoleMasters.Add(roleMaster);
        //    await db.SaveChangesAsync();
        //    return role;
        //}

        //public async Task<int> Edit(RoleDTO role)
        //{
        //    RoleMaster rm = new RoleMaster();
        //    rm.Id = role.Id;
        //    rm.IsVisible = role.IsVisible;
        //    rm.Name = role.Name;
        //    db.RoleMasters.Add(rm);
        //    db.Entry(rm).State = EntityState.Modified;
        //    int result = await db.SaveChangesAsync();
        //    return result;
        //}


        public async Task<bool> SaveSitePermission(SiteRolePermissionsDTO _dto)
        {
            try
            {
                GroupMaster groupMaster = new GroupMaster();
                if (_dto != null)
                {

                    if (_dto.SiteRolePermissions.ToList().Count > 0)
                    {
                        var SiteMapRolePermission = db.SiteMapRolePermissions.Where(o => o.RoleId == _dto.RoleId).ToList();
                        if (SiteMapRolePermission.ToList().Count > 0)
                        {
                            db.SiteMapRolePermissions.RemoveRange(SiteMapRolePermission);
                        }

                        List<SiteMapRolePermission> _SiteMapRolePermissionList = new List<SiteMapRolePermission>();
                        foreach (SiteRolePermissionDetailDTO item in _dto.SiteRolePermissions)
                        {
                            SiteMapRolePermission _SiteMapRolePermission = new SiteMapRolePermission();
                            _SiteMapRolePermission.Id = Guid.NewGuid();
                            _SiteMapRolePermission.RoleId = _dto.RoleId ?? Guid.Empty;
                            _SiteMapRolePermission.SiteMapId = item.SiteMapId ?? Guid.Empty;
                            _SiteMapRolePermission.PermissionId = item.PermissionId ?? Guid.Empty;
                            _SiteMapRolePermissionList.Add(_SiteMapRolePermission);
                        }

                        db.SiteMapRolePermissions.AddRange(_SiteMapRolePermissionList);
                    }

                    await db.SaveChangesAsync();
                    db.Dispose();

                    return true;
                }

                return false;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        
    }
}
