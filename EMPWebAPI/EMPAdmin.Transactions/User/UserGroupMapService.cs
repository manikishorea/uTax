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

namespace EMPAdmin.Transactions.User
{
    public class UserGroupMapService : IUserGroupMapService
    {
        public DatabaseEntities db = new DatabaseEntities();
        public UserRoleMapDTO userrolemap = new UserRoleMapDTO();

        //public UserService(DatabaseEntities _db, UserDetailDTO _user)
        //{
        //    db = _db;
        //    user = _user;
        //}

        public IQueryable<UserGroupMapDTO> GetAll()
        {
            db = new DatabaseEntities();

            var result = db.UserGroupMaps.Select(o => new UserGroupMapDTO
            {
                Id = o.Id,
                UserId = o.UserId,
                GroupId = o.GroupId,
                GroupName = o.GroupMaster.Name,
                UserName = o.UserMaster.FirstName,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            return result;
        }

        //public IQueryable<UserGroupMapDTO> GetByUserId(Guid Id)
        //{
        //    db = new DatabaseEntities();

        //    var result = db.UserGroupMaps.Where(o=>o.UserId==Id).Select(o => new UserGroupMapDTO
        //    {
        //        Id = o.Id,
        //        UserId = o.UserId,
        //        GroupId = o.GroupId,
        //        GroupName = o.GroupMaster.Name,
        //        UserName = o.UserMaster.FirstName
        //        //StatusCode = o.StatusCodeMaster.StatusCodes
        //    }).DefaultIfEmpty();

        //    return result;
        //}

        public async Task<UserGroupMapDTO> GetByUserId(Guid id)
        {
            var UserGroup = await db.UserGroupMaps.Where(o => o.UserId == id).Select(o => new UserGroupMapDTO
            {
                Id = o.Id,
                GroupId = o.GroupId,
                UserId = o.UserId,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return UserGroup;
        }

        public bool IsExists(Guid id)
        {
            return db.UserGroupMaps.Count(e => e.Id == id) > 0;
        }

    }
}
