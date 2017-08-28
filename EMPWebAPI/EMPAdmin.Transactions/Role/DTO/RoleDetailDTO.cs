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
    public class RoleDetailDTO : CoreModel
    {
        public RoleDTO Role { get; set; }
        public List<RoleDTO> ManageRoleList { get; set; }
        public List<SitePermissionsDTO> PermissionMasterList { get; set; }
        public List<SitemapMaster> SitemapMasterList { get; set; }
        public List<SiteMapRolePermission> SiteMapRolePermissionList { get; set; }

    }
}
