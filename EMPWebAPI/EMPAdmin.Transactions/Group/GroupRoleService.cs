using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Group.DTO;
using EMPAdmin.Transactions.Role.DTO;
using EMPEntityFramework.Edmx;
using System.Data.Entity.Infrastructure;
using EMP.Core.Utilities;
using System.Data.Entity;

namespace EMPAdmin.Transactions.Group
{
    public class GroupRoleService : IGroupRoleService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<GroupRoleDTO> GetAllGroupRole()
        {
            var result = (from i in db.GroupRoleMaps

                          let k = new
                          {
                              i.GroupId,
                              i.GroupMaster.Name,
                              i.GroupMaster.StatusCode,
                          }
                          group i by k into t
                          select new GroupRoleDTO
                          {
                              Id = t.Key.GroupId,
                              GroupId = t.Key.GroupId,
                              GroupName = t.Key.Name,
                              StatusCode = t.Key.StatusCode,
                              Roles = t.Select(s => new RoleDTO() { Id = s.RoleId, Name = s.RoleMaster.Name }).ToList()
                          }).DefaultIfEmpty();
            return result.OrderBy(o => o.GroupName);
        }

        public GroupRoleDTO GetGroupRolesByGroupId(Guid GroupId)
        {
            var result = (from i in db.GroupRoleMaps

                          let k = new
                          {
                              i.GroupId,
                              i.GroupMaster.Name,
                              i.GroupMaster.StatusCode,
                          }
                          where (i.GroupId == GroupId)
                          group i by k into t
                          select new GroupRoleDTO
                          {
                              Id = t.Key.GroupId,
                              GroupId = t.Key.GroupId,
                              GroupName = t.Key.Name,
                              StatusCode = t.Key.StatusCode,
                              Roles = t.Select(s => new RoleDTO() { Id = s.RoleId, Name = s.RoleMaster.Name }).ToList()
                          }).FirstOrDefault();


            return result;
        }

        public async Task<int> Save(GroupRoleDTO _dto, Guid Id, int EntityState)
        {
            try
            {
                GroupMaster groupMaster = new GroupMaster();
                if (_dto != null)
                {

                    groupMaster.Id = Id;
                    groupMaster.Name = _dto.GroupName;
                    groupMaster.StatusCode = EMPConstants.Active;
                }

                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    var ExistGroup = db.GroupMasters.Where(o => o.Id != Id && o.Name == _dto.GroupName).Any();
                    if (ExistGroup)
                        return -1;

                    groupMaster.LastUpdatedBy = _dto.UserId;
                    groupMaster.LastUpdatedDate = DateTime.Now;
                    db.Entry(groupMaster).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    var ExistGroup = db.GroupMasters.Where(o => o.Name == _dto.GroupName).Any();
                    if (ExistGroup)
                        return -1;

                    groupMaster.CreatedBy = _dto.UserId;
                    groupMaster.CreatedDate = DateTime.Now;
                    groupMaster.LastUpdatedBy = _dto.UserId;
                    groupMaster.LastUpdatedDate = DateTime.Now;
                    db.GroupMasters.Add(groupMaster);
                }

                if (_dto.Roles.ToList().Count > 0)
                {
                    if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                    {
                        var GroupRoleMapDel = db.GroupRoleMaps.Where(o => o.GroupId == Id).DefaultIfEmpty();
                        if (GroupRoleMapDel != null)
                            db.GroupRoleMaps.RemoveRange(GroupRoleMapDel);
                    }

                    List<GroupRoleMap> _GroupRoleMapList = new List<GroupRoleMap>();
                    foreach (RoleDTO item in _dto.Roles)
                    {
                        GroupRoleMap _GroupRoleMap = new GroupRoleMap();
                        _GroupRoleMap.Id = Guid.NewGuid();
                        _GroupRoleMap.GroupId = groupMaster.Id;
                        _GroupRoleMap.RoleId = item.Id;
                        _GroupRoleMapList.Add(_GroupRoleMap);
                    }

                    db.GroupRoleMaps.AddRange(_GroupRoleMapList);
                }

                await db.SaveChangesAsync();
                db.Dispose();

                return 1;
            }
            catch (DbUpdateConcurrencyException)
            {
                return 0;
            }
        }

        public async Task<int> SaveStatus(GroupDTO _Dto, Guid Id, int EntityState)
        {
            UserGroupMap objUserGroupMap = new UserGroupMap();
            bool IsUserGroupExist = await db.UserGroupMaps.Where(o => o.GroupId == Id).AnyAsync();

            if (!IsUserGroupExist)
            {
                GroupMaster groupMaster = new GroupMaster();
                groupMaster = await db.GroupMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();
                if (groupMaster.StatusCode == EMPConstants.InActive)
                {
                    groupMaster.StatusCode = EMPConstants.Active;
                }
                else if (groupMaster.StatusCode == EMPConstants.Active)
                {
                    groupMaster.StatusCode = EMPConstants.InActive;
                }
                if (EntityState == (int)System.Data.Entity.EntityState.Modified)
                {
                    groupMaster.LastUpdatedDate = DateTime.Now;
                    groupMaster.LastUpdatedBy = _Dto.UserId;
                    db.Entry(groupMaster).State = System.Data.Entity.EntityState.Modified;
                }
                try
                {
                    await db.SaveChangesAsync();
                    db.Dispose();
                    return (int)HttpStatusCode.OK;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return (int)HttpStatusCode.InternalServerError;
                }
            }
            else {
                return (int)HttpStatusCode.NotModified;
            }
        }

        private bool IsExists(Guid id)
        {
            return db.GroupMasters.Count(e => e.Id == id) > 0;
        }

        public async Task<bool> Delete(Guid Id)
        {
            // GroupRoleMap _GroupRoleMap = new GroupRoleMap();
            try
            {
                var GroupRoleMapRem = db.GroupRoleMaps.Where(o => o.Id == Id).FirstOrDefault();
                if (GroupRoleMapRem != null)
                {
                    db.GroupRoleMaps.Remove(GroupRoleMapRem);
                    await db.SaveChangesAsync();
                    db.Dispose();
                }
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        public bool GroupRoleMapExists(Guid id)
        {
            return db.GroupRoleMaps.Count(e => e.Id == id) > 0;
        }

        public bool GroupExists(Guid id)
        {
            return db.GroupMasters.Count(e => e.Id == id) > 0;
        }

        //public async Task<Guid> SaveStatus(GroupDTO _Dto, Guid Id, int EntityState)
        //{
        //    GroupMaster Group = new GroupMaster();
        //    Group = await db.GroupMasters.Where(o => o.Id == Id).FirstOrDefaultAsync();

        //    if (Group.StatusCode == EMPConstants.InActive)
        //    {
        //        Group.StatusCode = EMPConstants.Active;
        //    }
        //    else if (Group.StatusCode == EMPConstants.Active)
        //    {
        //        Group.StatusCode = EMPConstants.InActive;
        //    }

        //    if (EntityState == (int)System.Data.Entity.EntityState.Modified)
        //    {
        //        Group.LastUpdatedDate = DateTime.Now;
        //        Group.LastUpdatedBy = _Dto.UserId;
        //        db.Entry(Group).State = System.Data.Entity.EntityState.Modified;
        //    }

        //    try
        //    {
        //        await db.SaveChangesAsync();
        //        db.Dispose();
        //        return Group.Id;
        //    }

        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!GroupExists(Group.Id))
        //        {
        //            return Guid.Empty;
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}