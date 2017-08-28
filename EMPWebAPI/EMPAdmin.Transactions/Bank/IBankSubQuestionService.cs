using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EMPAdmin.Transactions.Bank.DTO;

namespace EMPAdmin.Transactions.Bank
{
    public interface IBankSubQuestionService
    {
        IQueryable<BankSubQuestionDTO> GetAll();
        Task<BankSubQuestionDTO> GetById(Guid Id);
        Task<Guid> Save(BankSubQuestionDTO _Dto, Guid Id, int State);
        Task<bool> Delete(Guid Id);
    }
}
