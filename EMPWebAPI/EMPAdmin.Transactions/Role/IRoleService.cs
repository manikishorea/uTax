using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Role.DTO;

namespace EMPAdmin.Transactions.Role
{
  public interface IRoleService
    {
        IQueryable<RoleDTO> GetAllRoles();
        Task<RoleDTO> GetRole(Guid roleId);
    }
}
