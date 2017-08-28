using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;

namespace EMPAdmin.Transactions.PermissionMaster.DTO
{
   public class PermissionMasterDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
    }
}
