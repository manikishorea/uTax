using EMPPortal.Transactions.EnrollmentOfficeConfig.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollmentOfficeConfig
{
    interface IEnrollmentOfficeConfigService
    {
        EnrollmentOfficeConfigDTO GetEnrollmentOfficeConfig(Guid UserId);
        int SaveEnrollmentOfficeConfig(EnrollmentOfficeConfigDTO dto);
    }
}
