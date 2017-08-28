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
   public class SitePermissionsDTO : CoreModel
    {
        public Nullable<System.Guid> RoleId { get; set; }

        public string ParentName { get; set; }
        public string SiteName { get; set; }
        public Nullable<int> SiteOrder { get; set; }
        public string PermissionName { get; set; }

        public string Name { get; set; }
        public Nullable<int> DisplayOrder { get; set; }

        public System.Guid Id { get; set; }
        public Nullable<System.Guid> SiteMapId { get; set; }
        public Nullable<System.Guid> PermissionId { get; set; }
        public Nullable<bool> IsVisisble { get; set; }
        public Nullable<bool> IsPermitted { get; set; }

        public SiteMapMasterDTO SiteMapMaster  { get; set; }
        public PermissionMasterDTO PermissionMaster { get; set; }
    }
}
