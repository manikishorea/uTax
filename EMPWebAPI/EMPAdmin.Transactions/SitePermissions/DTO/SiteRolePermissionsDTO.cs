using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
using EMPAdmin.Transactions.SiteMapMaster.DTO;
using EMPAdmin.Transactions.PermissionMaster.DTO;

namespace EMPAdmin.Transactions.SitePermissions.DTO
{
    public class SiteRolePermissionsDTO
    {
        public Nullable<System.Guid> UserId { get; set; }
        public Nullable<System.Guid> RoleId { get; set; }
        public List<SiteRolePermissionDetailDTO> SiteRolePermissions { get; set; }
    }

    public class SiteRolePermissionDetailDTO
    {
        public Nullable<System.Guid> Id { get; set; }
        public Nullable<System.Guid> SiteMapId { get; set; }
        public Nullable<System.Guid> PermissionId { get; set; }
    }


}
