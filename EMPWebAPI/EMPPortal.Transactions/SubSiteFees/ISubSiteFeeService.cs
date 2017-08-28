using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.SubSiteFees.DTO;

namespace EMPPortal.Transactions.SubSiteFees
{
    interface ISubSiteFeeService
    {
        IQueryable<SubSiteFeeDTO> GetSubSiteFeeById(Guid strguid);
        int Save(SubSiteFeesDTO dto);
        bool SaveCustomerAssociatedFees(TransmitterFeeDTO tmDto);
       bool UpdateCustomerAssociatedFees(TransmitterFeeDTO tmDto);
    }
}
