using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Role.DTO;
using EMPEntityFramework.Edmx;
namespace EMPAdmin.Transactions.User.DTO
{
    public class UserRoleMapSaveDTO
    {
        public System.Guid Id { get; set; }
        public List<UserRolesMap> UserRolesMapList { get; set; }
    }
}
