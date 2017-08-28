namespace EMPAdmin.Transactions.Group.DTO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using EMPAdmin.Transactions.Role.DTO;

    using EMP.Core.DTO;

    public class GroupRoleDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid GroupId { get; set; }
        public System.Guid RoleId { get; set; }
        public string GroupName { get; set; }
        public string RoleName { get; set; }
        public List<RoleDTO> Roles { get; set; }

        public System.Guid UserId { get; set; }
        public string StatusCode { get; set; }
    }
}