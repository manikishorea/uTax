using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Role.DTO;
namespace EMPAdmin.Transactions.User.DTO
{
    public class UserRoleMapDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid UserId { get; set; }
        public System.Guid RoleId { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; }

        public string StatusCode { get; set; }
        //public RoleDTO[] RoleList { get; set; }
    }
}
