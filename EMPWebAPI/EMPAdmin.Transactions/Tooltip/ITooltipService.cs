using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Tooltip.DTO;

namespace EMPAdmin.Transactions.Tooltip
{
  public interface ITooltipService
    {
        IQueryable<TooltipDTO> GetAll();
        Task<TooltipDTO> GetById(Guid Id);
        Task<Guid> Save(TooltipDTO _Dto, Guid Id, int State);
        Task<bool> Delete(Guid Id);
    }
}
