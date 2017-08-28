using EMPPortal.Transactions.EnrollmentBankSelectionInfo.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMPPortal.Transactions.EnrollmentBankSelectionInfo
{
    public interface IEnrollmentBankSelectionService
    {
        IQueryable<EnrollmentBankSelectionDTO> GetBankandFeesInfo(Guid entityid, Guid userid);

        bool EnrollmentBankSelectSave(EnrollmentBankSelectionDTO dto);

        IQueryable<EnrollmentBankSelectionDTO> GetEnrollmentBankSelection(Guid userid);
    }
}
