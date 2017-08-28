using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Group.DTO;

namespace EMPAdmin.Transactions.Group
{
   public interface IGroupService
    {
        IQueryable<GroupDTO> GetAllGroup();
        Task<GroupDTO> GetGroupById(Guid Id);
    }
}
