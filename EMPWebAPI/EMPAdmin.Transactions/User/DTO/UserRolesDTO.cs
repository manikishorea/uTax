using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Role.DTO;

namespace EMPAdmin.Transactions.User.DTO
{
    public class UserRolesDTO
    {
        public System.Guid UserId { get; set; }
        public List<RoleDTO> Roles { get; set; }
    }
}
