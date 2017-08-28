using EMPEntityFramework.Edmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.EnrollmentBankConfig.DTO;
using System.Data.Entity;
using EMP.Core.Utilities;

namespace EMPPortal.Transactions.EnrollmentBankConfig
{
    public class EnrollmentBankTPGConfigService : IEnrollmentBankTPGConfigService
    {
        DatabaseEntities db = new DatabaseEntities();

    }
}