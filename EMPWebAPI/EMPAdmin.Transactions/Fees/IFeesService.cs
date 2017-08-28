using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Fees.DTO;

namespace EMPAdmin.Transactions.Fees
{
  public interface IFeesService
    {
        IQueryable<FeesDTO> GetAll();
        Task<FeesDTO> GetById(Guid Id);
        Task<int> Save(FeesDTO _Dto, Guid Id, int State);
        Task<bool> Delete(Guid Id);
    }
}
