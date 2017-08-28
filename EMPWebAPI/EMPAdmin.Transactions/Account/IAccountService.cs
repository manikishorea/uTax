using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Account.DTO;

namespace EMPAdmin.Transactions.Account
{
    public interface IAccountService
    {
        Task<AccountDTO> IsUsernameExist(string userName);
        AccountDTO IsUserExist(string userName, string password);
        bool LastLoginUpdate(string userName);
        bool ResetPassword(string passwordHash, string userName);
        bool ResetPassword(string passwordHash, Guid UserId);
    }
}
