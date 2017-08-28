using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
using EMPEntityFramework.Edmx;
using EMPAdmin.Transactions.SitePermissions.DTO;

namespace EMPAdmin.Transactions.Role.DTO
{
    public class RoleDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public bool IsVisible { get; set; }
    }
}
