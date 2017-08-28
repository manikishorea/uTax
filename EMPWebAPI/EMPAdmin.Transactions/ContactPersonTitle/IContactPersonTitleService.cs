using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.ContactPersonTitle.DTO;

namespace EMPAdmin.Transactions.ContactPersonTitle
{
  public interface IContactPersonTitleService
    {
        IQueryable<ContactPersonTitleDTO> GetAll();
        Task<ContactPersonTitleDTO> GetById(Guid Id);
        Task<int> Save(ContactPersonTitleDTO _Dto, Guid Id, int EntityState);
        Task<bool> Delete(Guid Id);
    }
}
