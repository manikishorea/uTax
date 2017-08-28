using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.AffiliateProgram.DTO;

namespace EMPAdmin.Transactions.AffiliateProgram
{
  public interface IAffiliateProgramService
    {
        IQueryable<AffiliateProgramDTO> GetAll();
        Task<AffiliateProgramDTO> GetById(Guid Id);
        int Save(AffiliateProgramDTO _Dto, Guid Id, int State);
        bool Delete(Guid Id);
    }
}
