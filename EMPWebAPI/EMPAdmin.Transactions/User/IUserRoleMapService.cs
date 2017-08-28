using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.User.DTO;

namespace EMPAdmin.Transactions.User
{
    public interface IUserRoleMapService
    {
        IQueryable<UserRoleMapDTO> GetAll();
        Task<UserRoleMapDTO> GetById(Guid Id);
        Task<bool> Save(UserRoleMapDTO _userrolemap, Guid Id, int EntityState);
        bool UserRoleDeleteByUserId(Guid id);
    }
}
