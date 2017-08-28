using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.SubSiteOfficeAffiProgConfig.DTO
{
    public class SubSiteOfficeAffiliateProgramConfigDTO : CoreModel
    {
        public int Id { get; set; }
        public string SubSiteOfficeConfig_Id { get; set; }
        public string CustomerInformation_Id { get; set; }
        public string AffiliateProgramId { get; set; }
        public Nullable<decimal> AffiliateProgramCharge { get; set; }
    }
}
