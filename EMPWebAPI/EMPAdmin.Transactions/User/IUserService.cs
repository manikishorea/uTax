using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.User.DTO;

namespace EMPAdmin.Transactions.User
{
    public interface IUserService
    {
        IQueryable<UserDTO> GetAllUser();
        Task<UserDetailDTO> GetUser(Guid Id);
        Task<int> SaveUser(UserDetailDTO _user, Guid Id, int EntityState);
    }
}
