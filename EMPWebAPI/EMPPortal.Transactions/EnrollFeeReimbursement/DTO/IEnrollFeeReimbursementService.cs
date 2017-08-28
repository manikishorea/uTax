using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollFeeReimbursement.DTO
{
    interface IEnrollFeeReimbursementService
    {
        Task<EnrollFeeReimbursementDTO> GetEnrollFeeReimbursementById(Guid strguid);
        Task<bool> Save(EnrollFeeReimbursementDTO dto);
    }
}
