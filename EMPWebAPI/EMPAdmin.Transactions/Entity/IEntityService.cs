using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Entity.DTO;

namespace EMPAdmin.Transactions.Entity
{
   public interface IEntityService
    {
       IQueryable<EntityDTO> GetAllEntity();
       Task<EntityDTO> GetEntity(int Id);
       Task<EntityDetailDTO> GetEntityDetail(int Id);
    }
}
