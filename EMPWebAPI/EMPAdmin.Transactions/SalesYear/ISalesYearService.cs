using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.SalesYear.DTO;

namespace EMPAdmin.Transactions.SalesYear
{
  public interface ISalesYearService
    {
         IQueryable<SalesYearDTO> GetAll();
        Task<SalesYearDTO> GetById(Guid Id);
        Guid Save(SalesYearDTO _Dto, Guid Id, int State);
        //Task<bool> Delete(Guid Id);
    }
}
