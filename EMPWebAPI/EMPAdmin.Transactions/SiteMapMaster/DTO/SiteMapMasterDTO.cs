using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMP.Core.DTO;
namespace EMPAdmin.Transactions.SiteMapMaster.DTO
{
   public class SiteMapMasterDTO : CoreModel
    {
        public System.Guid Id { get; set; }
        public Nullable<System.Guid> ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string URL { get; set; }
        public Nullable<int> DisplayOrder { get; set; }
    }
}
