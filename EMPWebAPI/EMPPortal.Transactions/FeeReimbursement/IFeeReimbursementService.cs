using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.FeeReimbursement.DTO;
namespace EMPPortal.Transactions.FeeReimbursement
{
    interface IFeeReimbursementService
    {
        Task<FeeReimbursementDTO> GetFeeReimbursementById(Guid strguid);
        bool Save(FeeReimbursementDTO dto);
    }
}
