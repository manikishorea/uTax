using EMP.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollmentAffiliateConfig.DTO
{
    public class EnrollmentAffiliateConfigDetailDTO : CoreModel
    {
        public List<EnrollmentAffiliateConfigDTO> Affiliates { get; set; }
    }

    public class EnrollmentAffiliateConfigDTO
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string AffiliateProgramId { get; set; }
        public Nullable<decimal> AffiliateProgramCharge { get; set; }
        public bool IsAutoEnrollAffiliateProgram { get; set; }
    }
}
