using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.User.DTO;

namespace EMPAdmin.Transactions.User
{
    public interface IUserGroupMapService
    {
        IQueryable<UserGroupMapDTO> GetAll();
        Task<UserGroupMapDTO> GetByUserId(Guid Id);
       // Task<bool> Save(UserGroupMapDTO _userrolemap, Guid Id, int EntityState);
    }
}
