using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.SecurityQuestion.DTO;

namespace EMPAdmin.Transactions.SecurityQuestion
{
  public interface ISecurityQuestionService
    {
        IQueryable<SecurityQuestionDTO> GetAll();
        Task<SecurityQuestionDTO> GetById(Guid Id);
        Task<Guid> Save(SecurityQuestionDTO _Dto, Guid Id, int State);
        Task<bool> Delete(Guid Id);
    }
}
