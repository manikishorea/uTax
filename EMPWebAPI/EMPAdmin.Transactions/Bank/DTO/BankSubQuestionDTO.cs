using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using EMP.Core.DTO;
namespace EMPAdmin.Transactions.Bank.DTO
{
   public class BankSubQuestionDTO:CoreModel
    {
        public System.Guid Id { get; set; }
        public System.Guid BankId { get; set; }
        public string BankName { get; set; }
        public string Questions { get; set; }
        public Nullable<System.DateTime> ActivatedDate { get; set; }
        public Nullable<System.DateTime> DeActivatedDate { get; set; }
        public string Description { get; set; }
    }
}
