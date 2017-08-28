using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.ContactPersonTitle.DTO;

namespace EMPPortal.Transactions.ContactPersonTitle
{
  public interface IContactPersonTitleService
    {
        IQueryable<ContactPersonTitleDTO> GetAll();
    }
}
