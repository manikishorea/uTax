using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.PhoneType.DTO;

namespace EMPAdmin.Transactions.PhoneType
{
  public interface IPhoneTypeService
    {
        IQueryable<PhoneTypeDTO> GetAll();
        Task<PhoneTypeDTO> GetById(Guid Id);
        Task<int> Save(PhoneTypeDTO _Dto, Guid Id, int State);
        Task<bool> Delete(Guid Id);
    }
}
