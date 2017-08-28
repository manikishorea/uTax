using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.SubSite.DTO
{
   public class SubSiteAffiliateProgramDTO
    {
        public System.Guid Id { get; set; }
        public System.Guid refId { get; set; }
        public System.Guid AffiliateProgramId { get; set; }
        public System.Guid SubSiteConfigurationId { get; set; }
    }
}
