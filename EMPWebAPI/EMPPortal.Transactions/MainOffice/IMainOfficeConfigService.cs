using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.MainOffice.DTO;
namespace EMPPortal.Transactions.MainOffice
{
    interface IMainOfficeConfigService
    {
        Task<MainOfficeDTO> GetMainOfficeById(Guid strguid);
        bool Save(MainOfficeDTO dto);
    }
}
