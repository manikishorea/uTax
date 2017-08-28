using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.APIIntegrationMaster.DTO;

namespace EMPAdmin.Transactions.APIIntegrationMaster
{
  public interface IAPIIntegrationService
    {
        IQueryable<APIIntegrationDTO> GetAll();
        Task<APIIntegrationDTO> GetById(Guid Id);
        Task<Guid> Save(APIIntegrationDTO _Dto, Guid Id, int State);
        Task<bool> Delete(Guid Id);
    }
}
