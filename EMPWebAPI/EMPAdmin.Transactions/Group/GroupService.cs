using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Group.DTO;
using EMPAdmin.Transactions.Role.DTO;
using EMPEntityFramework.Edmx;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

using System.Data.Entity;
using EMP.Core.Utilities;

namespace EMPAdmin.Transactions.Group
{
    public class GroupService : IGroupService
    {
        public DatabaseEntities db = new DatabaseEntities();

        public IQueryable<GroupDTO> GetAllGroup()
        {
            var data = db.GroupMasters.Where(o => o.StatusCode == EMPConstants.Active).Select(o => new GroupDTO
            {
                Id = o.Id,
                Name = o.Name,
                StatusCode = o.StatusCode
            }).DefaultIfEmpty();

            return data;
        }

        public async Task<GroupDTO> GetGroupById(Guid Id)
        {
            var data = await db.GroupMasters.Where(o => o.Id == Id).Select(o => new GroupDTO
            {
                Id = o.Id,
                Name = o.Name,
                StatusCode = o.StatusCode
            }).FirstOrDefaultAsync();

            return data;
        }
    }
}