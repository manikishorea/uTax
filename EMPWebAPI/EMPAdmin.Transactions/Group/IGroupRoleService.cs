using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Group.DTO;
namespace EMPAdmin.Transactions.Group
{
   public interface IGroupRoleService
    {
        IQueryable<GroupRoleDTO> GetAllGroupRole();
        GroupRoleDTO GetGroupRolesByGroupId(Guid GroupId);
        Task<int> Save(GroupRoleDTO _dto,Guid Id, int EntityState);
        Task<bool> Delete(Guid Id);
    }
}
