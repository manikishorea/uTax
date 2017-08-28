using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

using System.Threading.Tasks;
using EMPEntityFramework.Edmx;

using System.Data.Entity;
using EMPAdmin.Transactions.User.DTO;
using System.Data.Entity.Infrastructure;
using EMPAdmin.Transactions.Role.DTO;
using EMP.Core.Utilities;
using System.Globalization;

namespace EMPAdmin.Transactions.User
{
    public class UserRoleMapService : IUserRoleMapService
    {
        public DatabaseEntities db = new DatabaseEntities();
        public UserRoleMapDTO userrolemap = new UserRoleMapDTO();

        //public UserService(DatabaseEntities _db, UserDetailDTO _user)
        //{
        //    db = _db;
        //    user = _user;
        //}

        public IQueryable<UserRoleMapDTO> GetAll()
        {
            db = new DatabaseEntities();

            var result = db.UserRolesMaps.Select(o => new UserRoleMapDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                RoleId = o.RoleId,
              //  UserName = o.UserMaster.FirstName + " " + o.UserMaster.LastName,
                RoleName = o.RoleMaster.Name,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            return result;
        }

        public IQueryable<UserRoleMapDTO> GetAllByUser()
        {
            db = new DatabaseEntities();

            var result = db.UserRolesMaps.Select(o => new UserRoleMapDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                RoleId = o.RoleId,
                //UserName = o.UserMaster.FirstName + " " + o.UserMaster.LastName,
                RoleName = o.RoleMaster.Name,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            var newresult = (from p in result
                             let k = new
                             {
                                 p.UserId,
                                 Name = p.UserName,
                                 p.StatusCode
                             }
                             group p by k into t
                             select new UserRoleMapDTO
                             {
                                 UserId = t.Key.UserId,
                                 UserName = t.Key.Name,
                                 StatusCode = t.Key.StatusCode,
                                 RoleName = string.Join("", t.Select(s => s.RoleName))
                             }).DefaultIfEmpty();

            return newresult;
        }

        public async Task<UserRoleMapDTO> GetById(Guid Id)
        {
            var result = await db.UserRolesMaps.Where(o => o.Id == Id).Select(o => new UserRoleMapDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                RoleId = o.RoleId,
              //  UserName = o.UserMaster.FirstName + " " + o.UserMaster.LastName,
                RoleName = o.RoleMaster.Name,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return result;
        }

        public IQueryable<UserRoleMapDTO> GetByUserId(Guid Id)
        {
            var result = db.UserRolesMaps.Where(o => o.UserId == Id).Select(o => new UserRoleMapDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                RoleId = o.RoleId,
              //  UserName = o.UserMaster.FirstName + " " + o.UserMaster.LastName,
                RoleName = o.RoleMaster.Name,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            return result;
        }

        public async Task<bool> Save(UserRoleMapDTO _dto, Guid Id, int EntityState)
        {
            UserRolesMap _userRolesMap = new UserRolesMap();

            if (_dto != null)
            {
                //usermaster.Id = "";
                _userRolesMap.Id = Id;
                _userRolesMap.UserId = _dto.UserId;
                _userRolesMap.RoleId = _dto.RoleId;
                _userRolesMap.StatusCode = EMPConstants.Active;
            }

            if (EntityState == (int)System.Data.Entity.EntityState.Modified)
            {
                db.Entry(_userRolesMap).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                db.UserRolesMaps.Add(_userRolesMap);
            }

            try
            {
                await db.SaveChangesAsync();
                return true;
            }

            catch (DbUpdateConcurrencyException)
            {
                if (!IsExists(_userRolesMap.Id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public bool IsExists(Guid id)
        {
            return db.UserRolesMaps.Count(e => e.Id == id) > 0;
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //          db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}


        //public IQueryable<UserRoleMapDTO> UserRoleByUserId(Guid id)
        //{
        //    var UserRole = db.UserRolesMaps.Where(o => o.UserId == id).Select(o => new UserRoleMapDTO
        //    {
        //        Id = o.Id,
        //        RoleId = o.RoleId,
        //        UserId = o.UserId,
        //        StatusCode = o.StatusCode
        //    }).DefaultIfEmpty();

        //    return UserRole;
        //}

        public bool UserRoleDeleteByUserId(Guid id)
        {
            try
            {
                var UserRole1 = db.UserRolesMaps.Where(o => o.UserId == id).DefaultIfEmpty();
                db.UserRolesMaps.RemoveRange(UserRole1);
                db.SaveChangesAsync();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
