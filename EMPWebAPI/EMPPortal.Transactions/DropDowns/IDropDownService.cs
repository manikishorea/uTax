using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPPortal.Transactions.DropDowns.DTO;

namespace EMPPortal.Transactions.DropDowns
{
  public interface IDropDownService
    {
        IQueryable<DropDownDTO> GetPhoneTypes();
        IQueryable<DropDownDTO> GetTitles();
        IQueryable<FeeEntityDTO> GetFeeMaster(int entityid, Guid userid);
        IQueryable<BankQuestionDTO> GetSubSiteBankAndQuestions(Guid Userid);
    }
}
