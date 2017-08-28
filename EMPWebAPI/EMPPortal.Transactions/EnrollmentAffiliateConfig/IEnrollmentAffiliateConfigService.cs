using EMPPortal.Transactions.EnrollmentAffiliateConfig.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollmentAffiliateConfig
{
    interface IEnrollmentAffiliateConfigService
    {
        IQueryable<EnrollmentAffiliateConfigDTO> GetEnrollmentAffiProgConfig(Guid UserId);
        int SaveEnrollmentAffiProgConfig(EnrollmentAffiliateConfigDetailDTO dto);
    }
}
